
import os
import shutil
import json
from fastapi import APIRouter, UploadFile, Form, HTTPException
from fastapi.responses import JSONResponse

from core.logging import process_logger
from core.config import UPLOAD_DIR
from core.db import collection
from utils import sanitize_name
from services.pdf import extract_text_from_pdf, extract_images_from_pdf, ocr_images
from services.llm import chunk_text_with_openai, generate_embeddings_for_texts

router = APIRouter()

@router.post("/process-lecture/")
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
