
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
