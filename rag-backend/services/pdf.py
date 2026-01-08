
import os
import fitz  # PyMuPDF
import pytesseract
from PIL import Image
from core.logging import process_logger
from core.config import TESSERACT_PATH

if os.path.exists(TESSERACT_PATH):
    pytesseract.pytesseract.tesseract_cmd = TESSERACT_PATH

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
