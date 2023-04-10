from integrations.openai.gpt_api import GptApi
import asyncio

api_key = " "
gpt_api = GptApi(api_key)

prompt = "Translate the following English text to French: 'Hello, how are you?'"



async def main():
    response = await gpt_api.generate_text_async(prompt)
    print(response)

asyncio.run(main())