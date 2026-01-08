# Automated Exam System (AES) with AI / RAG

## Description
This project is a university-level automated exam system that integrates **ASP.NET Core** with a **Python AI microservice**.  
The system uses **Retrieval-Augmented Generation (RAG)** and **OpenAI models** to automatically generate exam questions from lecture materials.

The Python backend is responsible for AI processing, while ASP.NET Core handles the main application logic and user interface.

---

## Features

- Upload lecture PDFs
- Extract text from PDFs
- Perform OCR on scanned pages
- Chunk lecture content
- Generate embeddings
- Store embeddings in ChromaDB
- Perform semantic search on lecture content
- Automatically generate exam questions (MCQ & True/False)
- REST API built with FastAPI
- Designed to integrate with ASP.NET Core backend

---

## Technologies & Dependencies

Python dependencies (see `requirements.txt`):

- **fastapi** – Web framework for building APIs
- **uvicorn** – ASGI server to run FastAPI
- **pydantic** – Data validation and schemas
- **pydantic-settings** – Environment-based configuration
- **python-dotenv** – Load environment variables from `.env`
- **python-multipart** – Handle file uploads
- **openai** – OpenAI API for question generation
- **chromadb** – Vector database for embeddings
- **pymupdf** – Extract text from PDF files
- **pytesseract** – OCR engine for scanned PDFs
- **pillow** – Image processing for OCR
- **requests** – HTTP requests
- **numpy** – Numerical computations
- **scikit-learn** – Similarity calculations
- **sentence-transformers** – Text embedding models
- **tqdm** – Progress bars for processing tasks

---

## Project Structure

