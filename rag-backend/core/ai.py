
from openai import OpenAI
from dotenv import load_dotenv

# Ensure env vars are loaded if this is imported independently
load_dotenv()

openai_client = OpenAI()
