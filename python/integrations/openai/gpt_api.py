import os
import httpx

class GptApi:
    def __init__(self, openai_api_key):
        self.api_key = openai_api_key
        self.endpoint = "https://api.openai.com/v1/chat/completions"

    async def generate_text_async(self, prompt: str, model: str = "gpt-3.5-turbo", max_tokens: int = 50, temperature: float = 1.0):
        headers = {
            "Content-Type": "application/json",
            "Authorization": f"Bearer {self.api_key}"
        }

        data = {
            "model": model,
            "messages": [
                {
                    "role": "system",
                    "content": "You are a helpful assistant."
                },
                {
                    "role": "user",
                    "content": prompt
                }
            ],
            "max_tokens": max_tokens,
            "temperature": temperature
        }

        async with httpx.AsyncClient() as client:
            response = await client.post(self.endpoint, json=data, headers=headers)

        if response.status_code != 200:
            raise Exception(f"Failed to generate text: {response.text}")

        result = response.json()
        generated_text = result['choices'][0]['message']['content']
        return generated_text.strip()
