# Put this near the top of your file (with your existing imports)
from fastapi import FastAPI, Body, UploadFile, Form, HTTPException
from fastapi.responses import JSONResponse
import os, shutil, json, math, time, re
from dotenv import load_dotenv

from typing import List, Dict, Optional

# PDF / OCR
import fitz  # PyMuPDF
import pytesseract
from PIL import Image

# OpenAI & Chroma
from openai import OpenAI
import chromadb

import logging
import sys


# ============================================================
# GLOBAL LOGGER  (app.log)
# ============================================================
logging.basicConfig(
    level=logging.INFO,
    format="%(asctime)s [%(levelname)s] %(message)s",
    handlers=[
        logging.FileHandler("app.log", encoding="utf-8"),
        logging.StreamHandler(sys.stdout)
    ]
)
logger = logging.getLogger("app")


# ============================================================
# SEPARATE LOGGER FOR /process-lecture (process_lecture.log)
# ============================================================
process_logger = logging.getLogger("process_lecture")
process_logger.setLevel(logging.INFO)

process_handler = logging.FileHandler("process_lecture.log", encoding="utf-8")
process_handler.setFormatter(logging.Formatter("%(asctime)s [%(levelname)s] %(message)s"))

process_logger.addHandler(process_handler)
process_logger.propagate = False  # avoid duplication


# ============================================================
# CORE SETUP
# ============================================================
load_dotenv()
app = FastAPI()
UPLOAD_DIR = "uploads"
os.makedirs(UPLOAD_DIR, exist_ok=True)

openai_client = OpenAI()

CHROMA_DB_DIR = "chroma_db"
os.makedirs(CHROMA_DB_DIR, exist_ok=True)

chroma_client = chromadb.PersistentClient(path=CHROMA_DB_DIR)
collection = chroma_client.get_or_create_collection(
    name="lecture_embeddings",
    metadata={"hnsw:space": "cosine"}
)


# Tesseract
TESSERACT_PATH = r"C:\Program Files\Tesseract-OCR\tesseract.exe"
if os.path.exists(TESSERACT_PATH):
    pytesseract.pytesseract.tesseract_cmd = TESSERACT_PATH


# ============================================================
# UTILITIES
# ============================================================
def sanitize_name(name: str) -> str:
    """Clean folder/file names — FIXES Windows file system errors."""
    clean = "".join(c if c.isalnum() or c in " _-" else "_" for c in name)
    clean = clean.strip()               # remove leading/trailing spaces
    clean = re.sub(r"\s+", "_", clean)  # normalize spaces
    return clean


def extract_images_from_pdf(pdf_path: str, images_folder: str) -> list[str]:
    os.makedirs(images_folder, exist_ok=True)
    image_paths = []
    pdf_doc = fitz.open(pdf_path)

    for page_num in range(len(pdf_doc)):
        for img_index, img in enumerate(pdf_doc[page_num].get_images(full=True)):
            xref = img[0]
            base_image = pdf_doc.extract_image(xref)
            image_bytes = base_image["image"]

            image_path = os.path.join(images_folder, f"page{page_num+1}_img{img_index+1}.png")
            with open(image_path, "wb") as f:
                f.write(image_bytes)

            image_paths.append(image_path)

    return image_paths


def ocr_images(image_paths: list[str]) -> str:
    ocr_texts = []
    for img_path in image_paths:
        try:
            img = Image.open(img_path)
            text = pytesseract.image_to_string(img, lang="ara+eng")
            ocr_texts.append(text)
        except Exception as e:
            process_logger.error(f"OCR FAILED → {img_path}: {e}")

    return "\n\n".join(ocr_texts)


def extract_text_from_pdf(pdf_path: str) -> str:
    pdf_doc = fitz.open(pdf_path)
    full_text = ""
    for page in pdf_doc:
        full_text += page.get_text("text") + "\n\n"
    return full_text


# ============================================================
# CHUNK TEXT (Token usage included)
# ============================================================
def chunk_text_with_openai(full_text: str) -> list[Dict]:
    system_prompt = (
        "You are a text chunker."
        "- DO NOT translate any text.\n"
        "- DO NOT rewrite, summarize, correct, or alter any text.\n"
        " Split the given lecture text into coherent chunks "
        "of about 800-1200 words each. Each chunk must have:\n"
        "- chunk_index\n"
        "- title\n"
        "- text\n\n"
        "Output strictly in JSON array."
    )

    response = openai_client.chat.completions.create(
        model="gpt-4.1",
        temperature=0.1,
        messages=[
            {"role": "system", "content": system_prompt},
            {"role": "user", "content": full_text}
        ]
    )

    ai_output = response.choices[0].message.content.strip()

    process_logger.info(
        f"Chunking Token Usage → prompt={response.usage.prompt_tokens}, "
        f"completion={response.usage.completion_tokens}, "
        f"total={response.usage.total_tokens}"
    )

    # 🔥 SAFE PARSE (avoids broken formatting)
    try:
        # Fix attempts: remove backticks or markdown wrappers
        ai_output = ai_output.replace("```json", "").replace("```", "").strip()
        return json.loads(ai_output)
    except Exception as e:
        process_logger.error("Invalid JSON returned by chunking model.")
        raise HTTPException(status_code=500, detail=f"Failed to parse chunks JSON: {e}")


# ============================================================
# GENERATE EMBEDDINGS (Token usage included)
# ============================================================
def generate_embeddings_for_texts(texts: list[str], model="text-embedding-3-large", batch_size=40):
    embeddings = []
    total = len(texts)

    for i in range(0, total, batch_size):
        batch = texts[i:i+batch_size]
        resp = openai_client.embeddings.create(model=model, input=batch)

        process_logger.info(
            f"Embedding Token Usage → total={resp.usage.total_tokens}"
        )

        for item in resp.data:
            embeddings.append(item.embedding)

        time.sleep(0.1)

    return embeddings

def llm_relevance_check(query: str, chunk: str) -> bool:
    response = openai_client.chat.completions.create(
        model="gpt-4.1",
        temperature=0,
        messages=[
            {
                "role": "system",
                "content": (
                    "You are a strict relevance judge. "
                    "Answer ONLY with YES or NO. "
                    "NO explanations."
                )
            },
            {
                "role": "user",
                "content": f"""
Semantic Query:
{query}

Retrieved Text:
{chunk}

Is the retrieved text directly related to the semantic query?
"""
            }
        ]
    )

    answer = response.choices[0].message.content.strip().upper()
    return answer == "YES"

# ============================================================
# MAIN ENDPOINT: /process-lecture
# ============================================================
@app.post("/process-lecture/")
async def process_lecture(
    file: UploadFile,
    lecture_id: int = Form(...),
    course_name: str = Form(...),
    lecture_name: str = Form(...)
):

    process_logger.info("=== NEW REQUEST /process-lecture ===")
    process_logger.info(
        f"Incoming → id={lecture_id}, course='{course_name}', lecture='{lecture_name}', file='{file.filename}'"
    )

    # --- Create folders safely ---
    course_folder = os.path.join(UPLOAD_DIR, sanitize_name(course_name))
    lecture_folder = os.path.join(course_folder, sanitize_name(lecture_name), str(lecture_id))
    os.makedirs(lecture_folder, exist_ok=True)

    # --- Save PDF ---
    file_path = os.path.join(lecture_folder, file.filename)
    with open(file_path, "wb") as buffer:
        shutil.copyfileobj(file.file, buffer)

    # --- Processing steps ---
    process_logger.info("Step 1: Extracting standard PDF text...")
    pdf_text = extract_text_from_pdf(file_path)

    process_logger.info("Step 2: Extracting images for OCR...")
    images_folder = os.path.join(lecture_folder, "images")
    image_paths = extract_images_from_pdf(file_path, images_folder)
    process_logger.info(f"Found {len(image_paths)} images.")

    process_logger.info("Step 3: OCR on extracted images...")
    ocr_text = ocr_images(image_paths) if image_paths else ""

    process_logger.info("Step 4: Merge PDF text + OCR text...")
    combined_text = (pdf_text or "") + "\n\n" + (ocr_text or "")

    # Save extracted text
    text_path = os.path.join(lecture_folder, "extracted_text.txt")
    with open(text_path, "w", encoding="utf-8") as f:
        f.write(combined_text)

    process_logger.info("Step 5: Semantic chunking with OpenAI...")
    chunks_json = chunk_text_with_openai(combined_text)
    process_logger.info(f"Chunking complete → {len(chunks_json)} chunks")

    chunks_path = os.path.join(lecture_folder, "embedding_chunks.json")
    with open(chunks_path, "w", encoding="utf-8") as f:
        json.dump(chunks_json, f, ensure_ascii=False, indent=2)

    # --- Embeddings ---
    process_logger.info("Step 6: Generating embeddings...")
    try:
        texts = [c["text"] for c in chunks_json]
        embedding_vectors = generate_embeddings_for_texts(texts)

        ids, documents, metadatas = [], [], []

        for c, emb in zip(chunks_json, embedding_vectors):
            cid = f"{lecture_id}_{c['chunk_index']}"
            ids.append(cid)
            documents.append(c["text"])
            metadatas.append({
                "lecture_id": lecture_id,
                "course_name": course_name,
                "lecture_name": lecture_name,
                "chunk_index": c["chunk_index"],
                "title": c["title"],
                "filename": file.filename
            })

        collection.add(
            ids=ids,
            embeddings=embedding_vectors,
            documents=documents,
            metadatas=metadatas
        )

    except Exception as e:
        process_logger.error(f"Embedding/Chroma error: {e}")
        raise HTTPException(status_code=500, detail=f"Embedding/Chroma error: {e}")

    process_logger.info("=== /process-lecture COMPLETED SUCCESSFULLY ===\n")

    return JSONResponse({
        "status": "ok",
        "message": "Processed, chunked, embedded, and stored in ChromaDB.",
        "lecture_folder": lecture_folder,
        "chunks": len(chunks_json)
    })

# ============================================================
# ENDPOINT: /show-database
# ============================================================
@app.get("/show-database/")
async def show_database(limit: int = 20):
    """
    Show a snapshot of what's stored in the ChromaDB collection.
    Default limit = 20 items.
    """
    try:
        results = collection.get(
            include=["metadatas", "documents"],
            limit=limit
        )

        # Build a clean response
        items = []
        for i in range(len(results["ids"])):
            items.append({
                "id": results["ids"][i],
                "metadata": results["metadatas"][i],
                "document_preview": results["documents"][i][:200]  # first 200 chars
            })

        return JSONResponse({
            "status": "ok",
            "count": len(items),
            "items": items
        })

    except Exception as e:
        logger.error(f"Error showing database: {e}")
        raise HTTPException(status_code=500, detail=f"Error showing database: {e}")

# --------------------------
# Simple semantic search endpoint
# --------------------------
@app.get("/semantic-search/")
async def semantic_search(query: str, top_k: int = 5, lecture_id: int | None = None):
    if not query:
        raise HTTPException(status_code=400, detail="query parameter required")

    # 1️⃣ Embed the query
    q_emb = openai_client.embeddings.create(
        model="text-embedding-3-large",
        input=query
    ).data[0].embedding

    # 2️⃣ Build filter if lecture_id is provided
    filters = {}
    if lecture_id is not None:
        filters["lecture_id"] = lecture_id

    # 3️⃣ Query Chroma with optional filter
    results = collection.query(
        query_embeddings=[q_emb],
        n_results=top_k,
        include=["metadatas", "documents", "distances"],
        where=filters  # 👈 restricts results to that lecture_id
    )

    # 4️⃣ Extra logic: fetch next chunk if sequentially related
    extra_docs = []
    extra_metas = []

    for meta in results["metadatas"][0]:
        idx = meta["chunk_index"]
        lec_id = meta["lecture_id"]

        # Only fetch sequential chunks for the same lecture
        if lecture_id is None or lec_id == lecture_id:
            next_id = f"{lec_id}_{idx+1}"
            try:
                next_chunk = collection.get(ids=[next_id], include=["documents", "metadatas"])
                if next_chunk["ids"]:
                    extra_docs.append(next_chunk["documents"][0])
                    extra_metas.append(next_chunk["metadatas"][0])
            except Exception as e:
                print(f"Sequential fetch failed for {next_id}: {e}")

    # 5️⃣ Merge original results with sequential extras
    if extra_docs:
        results["documents"][0].extend(extra_docs)
        results["metadatas"][0].extend(extra_metas)

    return JSONResponse(results)

# ============================================================
# ENDPOINT: /show-courses
# ============================================================
@app.get("/show-courses/")
async def show_courses():
    """
    Return all course_name with their lectures grouped,
    but keep repeated lectures separate (using lecture_id or filename).
    """
    try:
        results = collection.get(include=["metadatas"])

        courses_dict = {}
        for meta in results["metadatas"]:
            if meta:
                course = meta.get("course_name", "").strip()
                lecture = meta.get("lecture_name")
                lecture_id = meta.get("lecture_id")
                filename = meta.get("filename")  # if you stored it earlier

                if course not in courses_dict:
                    courses_dict[course] = {}

                # use lecture_id (or filename) as part of the key to avoid merging
                key = f"{lecture}__{lecture_id}"  # or f"{lecture}__{filename}"
                if key not in courses_dict[course]:
                    courses_dict[course][key] = {
                        "lecture_name": lecture,
                        "lecture_id": lecture_id,
                        "filename": filename,
                        "total_chunks": 0
                    }

                courses_dict[course][key]["total_chunks"] += 1

        # Convert to list of dicts for JSON
        items = []
        for course, lectures in courses_dict.items():
            items.append({
                "course_name": course,
                "lectures": list(lectures.values())
            })

        return JSONResponse({
            "status": "ok",
            "count": len(items),
            "courses": items
        })

    except Exception as e:
        logger.error(f"Error showing courses: {e}")
        raise HTTPException(status_code=500, detail=f"Error showing courses: {e}")




@app.get("/list-collections/")
async def list_collections():
    collections = chroma_client.list_collections()
    # Convert each Collection object to a dict
    collections_info = [
        {
            "name": c.name,
            "metadata": c.metadata
        }
        for c in collections
    ]
    return JSONResponse({"collections": collections_info})

@app.get("/dump-collection/")
async def dump_collection(name: str = "lecture_embeddings", limit: int = 10):
    try:
        collection = chroma_client.get_collection(name)
        # Fetch all items (be careful if you have thousands!)
        results = collection.get(
            include=["documents", "metadatas"],
            limit=limit  # safety: only show first N
        )

        # Format for JSONResponse
        items = []
        for i, doc in enumerate(results["documents"]):
            items.append({
                "id": results["ids"][i],
                "document": doc[:200] + "..." if len(doc) > 200 else doc,  # preview
                "metadata": results["metadatas"][i]
            })

        return JSONResponse({"collection": name, "items": items})

    except Exception as e:
        raise HTTPException(status_code=500, detail=f"Error dumping collection: {e}")


@app.get("/get-item/")
async def get_item(item_id: str, include_vectors: bool = False):
    try:
        collection = chroma_client.get_collection("lecture_embeddings")

        results = collection.get(
            ids=[item_id],
            include=["documents", "metadatas"] + (["embeddings"] if include_vectors else [])
        )

        if not results["ids"]:
            raise HTTPException(status_code=404, detail=f"Item {item_id} not found")

        # Build response
        item = {
            "id": results["ids"][0],
            "document": results["documents"][0],
            "metadata": results["metadatas"][0]
        }

        if include_vectors:
            # Convert ndarray → list
            emb = results["embeddings"][0]
            item["embedding_preview"] = emb.tolist()[:10]  # first 10 numbers for readability

        return JSONResponse(item)

    except Exception as e:
        raise HTTPException(status_code=500, detail=f"Error fetching item: {e}")
    

    
    
@app.post("/generate-questions/")
async def generate_questions(payload: dict = Body(...)):
    try:
        logger.info("=== NEW REQUEST /generate-questions ===")
        logger.info(f"Incoming payload: {payload}")

        # -------------------------
        # Step 0: Normalize payload keys
        # -------------------------
        lecture_ids: List[int] = (
            payload.get("LecturesToProcess")
            or payload.get("lecturesToProcess")
            or []
        )
        distribution: Dict[str, int] = (
            payload.get("Distribution")
            or payload.get("distribution")
            or {}
        )

        semantic_queries = (
            payload.get("SemanticQueries")
            or payload.get("semanticQueries")
            or payload.get("SemanticQuery")
            or payload.get("semanticQuery")
            or []
        )

        # Auto-wrap single string
        if isinstance(semantic_queries, str):
            semantic_queries = [semantic_queries]

        # Split comma string
        if len(semantic_queries) == 1 and "," in semantic_queries[0]:
            semantic_queries = [q.strip() for q in semantic_queries[0].split(",") if q.strip()]

        logger.info(f"Normalized lecture_ids={lecture_ids}")
        logger.info(f"Normalized distribution={distribution}")
        logger.info(f"Normalized semantic_queries={semantic_queries}")

        if not lecture_ids:
            raise HTTPException(status_code=400, detail="No lectures provided")

        # -----------------------------------------
        # PRECOMPUTE QUERY EMBEDDINGS
        # -----------------------------------------
        query_embeddings_cache = {}
        if semantic_queries:
            logger.info("Precomputing semantic query embeddings...")
            for q in semantic_queries:
                emb = openai_client.embeddings.create(
                    model="text-embedding-3-large",
                    input=q
                ).data[0].embedding

                query_embeddings_cache[q] = emb
                logger.info(f"[EMBED] Precomputed embedding for query '{q}'")

        # ============================================================
        # 🔥🔥 NEW GLOBAL RETRIEVAL LOGIC (Best chunk per query)
        # ============================================================
        grouped_text = []
        processed_chunk_ids = set()

        if semantic_queries:
            logger.info("SemanticQueries provided → GLOBAL BEST chunk per query across ALL lectures")

            global_docs = []
            global_metas = []
            global_ids = []

            for q in semantic_queries:
                logger.info(f"[QUERY] Running GLOBAL search for: '{q}'")

                emb = query_embeddings_cache[q]

                results = collection.query(
                    query_embeddings=[emb],
                    where={"lecture_id": {"$in": lecture_ids}},
                    n_results=3,
                    include=["documents", "metadatas", "distances"]
                )

                logger.info(f"[QUERY] Global search results for '{q}': {results}")

                docs = results.get("documents", [[]])[0]
                metas = results.get("metadatas", [[]])[0]
                ids = results.get("ids", [[]])[0]

                if docs:
                    chunk_text = docs[0]

                    is_relevant = llm_relevance_check(q, chunk_text)

                    if not is_relevant:
                        logger.warning(
                            f"[LLM-REJECTED] Query '{q}' is NOT related to retrieved chunk"
                        )
                        continue
                    
                    global_docs.extend(docs)
                    global_metas.extend(metas)
                    global_ids.extend(ids)

            if not global_docs:
                raise HTTPException(status_code=404, detail="No semantic matches found")

            # Format the global block
            final_text = "\n".join(global_docs)
            lecture_name = global_metas[0].get("lecture_name", "") if global_metas else ""

            grouped_text.append(
                f"### GLOBAL BEST MATCHES (FROM LECTURES={lecture_ids}, LECTURE_NAME_SAMPLE={lecture_name})\n"
                f"{final_text}\n"
            )

            processed_chunk_ids.update(global_ids)

        # ============================================================
        # NEW BEHAVIOR: If NO semantic queries, distribute chunks per lecture
        # ============================================================
        else:
            logger.info("No semantic queries → distribute chunks based on question distribution")

            total_questions = sum(distribution.values())
            questions_per_chunk = 5  # configurable constant
            chunks_needed = math.ceil(total_questions / questions_per_chunk)

            chunks_per_lecture = {}

            if total_questions < questions_per_chunk:
                # Rule: if less than 5 questions, load 1 chunk from each lecture
                for lec_id in lecture_ids:
                    chunks_per_lecture[lec_id] = 1
            else:
                # Fair distribution with minimum 1 chunk per lecture
                base = chunks_needed // len(lecture_ids)
                remainder = chunks_needed % len(lecture_ids)

                chunks_per_lecture = {lec_id: max(1, base) for lec_id in lecture_ids}
                for i in range(remainder):
                    lec_id = lecture_ids[i]
                    chunks_per_lecture[lec_id] += 1

            for lec_id in lecture_ids:
                num_chunks = chunks_per_lecture[lec_id]
                logger.info(f"Processing lecture_id={lec_id} with {num_chunks} chunks")

                neutral_embedding = openai_client.embeddings.create(
                    model="text-embedding-3-large",
                    input=" "
                ).data[0].embedding

                results = collection.query(
                    query_embeddings=[neutral_embedding],
                    where={"lecture_id": lec_id},
                    n_results=num_chunks,
                    include=["documents", "metadatas"]
                )

                docs = results.get("documents", [[]])[0]
                metas = results.get("metadatas", [[]])[0]
                ids = results.get("ids", [[]])[0]

                if not docs:
                    continue

                combined = list(zip(docs, metas, ids))
                combined_sorted = sorted(combined, key=lambda x: x[1].get("chunk_index", 0))

                lecture_text = "\n".join([d for d, _, _ in combined_sorted])
                lecture_name = combined_sorted[0][1].get("lecture_name", "")

                grouped_text.append(
                    f"### LECTURE_ID={lec_id} (LECTURE_NAME={lecture_name})\n{lecture_text}\n"
                )

                processed_chunk_ids.update([i for _, _, i in combined_sorted])

        # ============================================================
        # FINAL PREPARATION
        # ============================================================
        if not grouped_text:
            raise HTTPException(status_code=404, detail="No RAG content found for these lectures")

        rag_context = "\n\n".join(grouped_text)

        logger.info(f"Chunk IDs contributing to LLM context: {sorted(list(processed_chunk_ids))}")
        logger.info(f"RAG grouped context size: {len(rag_context)} characters")
        logger.info(f"LLM will receive {len(semantic_queries)} semantic queries: {semantic_queries}")

        # -------------------------
        # Step 2: Build prompts
        # -------------------------
        system_prompt = (
            "You generate exam questions in EXACT plain text format.\n"
            "STRICT RULE: DO NOT return JSON, markdown, lists, code blocks, or quotes.\n"
            "Output MUST be raw text exactly following the required categories.\n\n"
            "HEADERS MUST APPEAR EXACTLY AS:\n"
            "mcqEasy:\n"
            "mcqMedium:\n"
            "mcqHard:\n"
            "tfEasy:\n"
            "tfMedium:\n"
            "tfHard:\n\n"
            "Format rules:\n"
            "- No braces, no brackets, no JSON.\n"
            "- No markdown symbols like ``` or **.\n"
            "- Blank line between each question.\n"
            "- MCQ must use A), B), C), D) exactly.\n"
            "- True/False must use 'Answer: True' or 'Answer: False'.\n"
            "- Do NOT add or remove extra sections.\n\n"
            "DIFFICULTY DEFINITIONS (MANDATORY):\n"
            "Easy:\n"
            "- Tests basic definitions, terminology, or direct facts.\n"
            "- Answer must be obvious from a single sentence in the lecture.\n"
            "- No reasoning, no comparisons, no trick wording.\n"
            "Medium:\n"
            "- Tests understanding and application of concepts.\n"
            "- May require short reasoning or combining two facts from the lecture.\n"
            "Hard:\n"
            "- Tests deep understanding, analysis, or edge cases.\n"
            "- Requires multi-step reasoning or recognizing subtle differences.\n"
            "- Avoid purely memorized facts.\n\n"
            "COGNITIVE VERBS BY DIFFICULTY:\n"
            "Easy verbs: define, identify, recognize, state\n"
            "Medium verbs: explain, compare, apply, classify\n"
            "Hard verbs: analyze, evaluate, infer, deduce, justify\n\n"
            "MCQ RULES:\n"
            "- Only ONE correct answer.\n"
            "- Distractors must be plausible and related to the lecture.\n"
            "- Do NOT use obviously wrong options.\n\n"
            "RULE CHECK:\n"
            "Before writing each question, verify internally that it matches the assigned difficulty level.\n"
            "If a question does not match its difficulty, regenerate it.\n"
        )

        user_prompt = f"""
Generate questions EXACTLY in the required structure.

⚠️ NEW IMPORTANT RULES:
1. For every question you generate, add two final lines:
(lectureId: X)
(lectureName: Y)

Where:
- X is the LECTURE_ID used
- Y is the LECTURE_NAME shown in the RAG context

2. When generating questions for each category (mcqEasy, tfEasy, etc.), you MUST distribute them across the lectures provided. 
   - Example: If mcqEasy requires 3 questions and there are 3 lectures, generate 1 question from each lecture.
   - Do NOT put all questions from the same lecture unless the distribution explicitly requires it.

Example output format:

mcqEasy:
Q1) Example question?
A) Option A
B) Option B
C) Option C
D) Option D
correct answer : A
(lectureId: 66)
(lectureName: Lecture 8)

Q2) Another question?
A) Option A
B) Option B
C) Option C
D) Option D
correct answer : B
(lectureId: 77)
(lectureName: Lecture 5)

Q3) Another question?
A) Option A
B) Option B
C) Option C
D) Option D
correct answer : C
(lectureId: 78)
(lectureName: Lecture 6)


Number of questions required per category:
{json.dumps(distribution, indent=4)}

RAG CONTEXT (GROUPED BY LECTURE):
{rag_context}

Semantic Queries: {semantic_queries}

FOLLOW THE FORMAT EXACTLY.
"""

        logger.info("=== SYSTEM PROMPT SENT TO LLM ===")
        logger.info(system_prompt)

        logger.info("=== USER PROMPT SENT TO LLM ===")
        logger.info(user_prompt)

        # -------------------------
        # Step 3: Call OpenAI LLM
        # -------------------------
        response = openai_client.chat.completions.create(
            model="gpt-4.1",
            temperature=0.1,
            messages=[
                {"role": "system", "content": system_prompt},
                {"role": "user", "content": user_prompt}
            ]
        )

        ai_output = response.choices[0].message.content.strip()

        logger.info("=== RAW LLM OUTPUT ===")
        logger.info(ai_output)

        if hasattr(response, "usage") and response.usage:
            logger.info("=== TOKEN USAGE ===")
            logger.info(f"Prompt tokens: {response.usage.prompt_tokens}")
            logger.info(f"Completion tokens: {response.usage.completion_tokens}")
            logger.info(f"Total tokens: {response.usage.total_tokens}")
        else:
            logger.warning("LLM returned no token usage.")

        return JSONResponse(content={"questions": ai_output})

    except Exception as e:
        logger.exception("Error generating questions.")
        raise HTTPException(status_code=500, detail=f"Error generating questions: {str(e)}")




