
from fastapi import FastAPI
from dotenv import load_dotenv

# Import Routers
from routers import processing,generation

# Load Env (just in case, though core.config does it too)
load_dotenv()

app = FastAPI()

# Include Routers
app.include_router(processing.router)
app.include_router(generation.router)

@app.get("/")
def root():
    return {"message": "RAG Backend is running. Access docs at /docs"}
