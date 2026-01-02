# Automated Exam System (AES) with AI / RAG

## Description
This project is a university-level automated exam system that uses AI (OpenAI) and RAG (Retrieval-Augmented Generation) for:

- Uploading lecture PDFs
- Extracting text (from PDFs and images using OCR)
- Chunking lecture content
- Generating embeddings
- Performing semantic search
- Automatically generating exam questions (MCQ and True/False) based on lecture content

---

## Features

- Upload PDF lectures and extract text & images
- Perform OCR on scanned pages
- Chunk text using GPT
- Store embeddings in ChromaDB
- Semantic search across lectures
- Generate exam questions (MCQ & True/False) with difficulty levels
- FastAPI backend with REST endpoints

---

## Technologies & Dependencies

Python packages (see `requirements.txt`):

- `fastapi` : API framework  
- `uvicorn` : Server to run FastAPI  
- `pydantic` & `pydantic-settings` : Data validation and configuration  
- `python-dotenv` : Load environment variables  
- `python-multipart` : Handle file uploads  
- `openai` : GPT API calls  
- `chromadb` : Vector database for embeddings  
- `pymupdf` : PDF text extraction  
- `pytesseract` + `pillow` : OCR for scanned images  
- `requests` : HTTP requests  
- `numpy` : Numerical computations  
- `scikit-learn` : Similarity calculations  


---

## Project Structure

