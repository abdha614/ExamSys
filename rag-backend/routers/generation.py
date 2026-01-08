
import math
import json
from typing import List, Dict, Optional
from fastapi import APIRouter, Body, HTTPException
from fastapi.responses import JSONResponse

from core.logging import logger
from core.ai import openai_client
from core.db import collection
from services.llm import llm_relevance_check

router = APIRouter()

@router.post("/generate-questions/")
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
