
import os
from dotenv import load_dotenv

load_dotenv()

UPLOAD_DIR = "uploads"
os.makedirs(UPLOAD_DIR, exist_ok=True)

CHROMA_DB_DIR = "chroma_db"
os.makedirs(CHROMA_DB_DIR, exist_ok=True)

TESSERACT_PATH = r"C:\Program Files\Tesseract-OCR\tesseract.exe"
