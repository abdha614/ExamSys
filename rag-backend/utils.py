
import re

def sanitize_name(name: str) -> str:
    """Clean folder/file names — FIXES Windows file system errors."""
    clean = "".join(c if c.isalnum() or c in " _-" else "_" for c in name)
    clean = clean.strip()               # remove leading/trailing spaces
    clean = re.sub(r"\s+", "_", clean)  # normalize spaces
    return clean
