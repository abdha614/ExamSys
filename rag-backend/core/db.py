
import chromadb
from .config import CHROMA_DB_DIR

chroma_client = chromadb.PersistentClient(path=CHROMA_DB_DIR)
collection = chroma_client.get_or_create_collection(
    name="lecture_embeddings",
    metadata={"hnsw:space": "cosine"}
)
