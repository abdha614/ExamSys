
import json
import time
from typing import Dict
from fastapi import HTTPException
from core.logging import process_logger
from core.ai import openai_client

def chunk_text_with_openai(full_text: str) -> list[Dict]:
    system_prompt = (
        "You are a text chunker."
        "- DO NOT translate any text.\n"
        "- DO NOT rewrite, summarize, correct, or alter any text.\n"
        " Split the given lecture text into coherent chunks "
        "of about 800-1200 words each. Each chunk must have:\n"
        "- chunk_index\n"
        "- title\n"
        "- text\n\n"
        "Output strictly in JSON array."
    )

    response = openai_client.chat.completions.create(
        model="gpt-4.1",  # NOTE: Assuming gpt-4.1 is valid alias or user has access. Kept from original.
        temperature=0.1,
        messages=[
            {"role": "system", "content": system_prompt},
            {"role": "user", "content": full_text}
        ]
    )

    ai_output = response.choices[0].message.content.strip()

    process_logger.info(
        f"Chunking Token Usage → prompt={response.usage.prompt_tokens}, "
        f"completion={response.usage.completion_tokens}, "
        f"total={response.usage.total_tokens}"
    )

    # 🔥 SAFE PARSE (avoids broken formatting)
    try:
        # Fix attempts: remove backticks or markdown wrappers
        ai_output = ai_output.replace("```json", "").replace("```", "").strip()
        return json.loads(ai_output)
    except Exception as e:
        process_logger.error("Invalid JSON returned by chunking model.")
        raise HTTPException(status_code=500, detail=f"Failed to parse chunks JSON: {e}")


def generate_embeddings_for_texts(texts: list[str], model="text-embedding-3-large", batch_size=40):
    embeddings = []
    total = len(texts)

    for i in range(0, total, batch_size):
        batch = texts[i:i+batch_size]
        resp = openai_client.embeddings.create(model=model, input=batch)

        process_logger.info(
            f"Embedding Token Usage → total={resp.usage.total_tokens}"
        )

        for item in resp.data:
            embeddings.append(item.embedding)

        time.sleep(0.1)

    return embeddings

def llm_relevance_check(query: str, chunk: str) -> bool:
    response = openai_client.chat.completions.create(
        model="gpt-4.1",
        temperature=0,
        messages=[
            {
                "role": "system",
                "content": (
                    "You are a strict relevance judge. "
                    "Answer ONLY with YES or NO. "
                    "NO explanations."
                )
            },
            {
                "role": "user",
                "content": f"""
Semantic Query:
{query}

Retrieved Text:
{chunk}

Is the retrieved text directly related to the semantic query?
"""
            }
        ]
    )

    answer = response.choices[0].message.content.strip().upper()
    return answer == "YES"
