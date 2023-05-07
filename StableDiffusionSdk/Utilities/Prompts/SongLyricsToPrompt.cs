using Newtonsoft.Json;
using StableDiffusionTools.Integrations.OpenAi;

namespace StableDiffusionSdk.Utilities.Prompts
{
    public static class SongLyricsToPrompt
    {
        private static readonly string Template = @"
Please generate standalone image prompts based on the following lyrics. Each prompt should describe a scene in detail, including characters, their appearance, 
clothing, facial expressions, background environment, and objects in the scene. The images will change every 5 seconds in the video, and they should reflect 
the content of the lyrics. Generate prompt per verse of lyrics.
Note that the prompts are independent, and no information will carry from one prompt to another. Please provide your response as a 
JSON array of strings so that the software can parse it accurately.

Lyrics:

{0}

Guidelines:

1. Characters: Include descriptions of clothing, hair, skin, facial expressions, shoes, body posture, lighting, colors, and the part of the image each character 
occupies. Repeat the character descriptions in each prompt, as the prompts are independent of each other and will not carry information from one to another.
2. Background: Describe the environment and any relevant objects in the scene.
3. Specificity: Be as precise as possible regarding colors, characters, environment, and the positioning of each object.
4. Consistency: Maintain consistent character descriptions throughout all prompts, including clothing, hair, and other details.

Also, make it ""safe"": no violence
";

        public static async Task<string[]> SongLyricsToPrompts(this GptApi api, string lyrics)
        {
            var gptMessage = string.Format(Template, lyrics);
            var responseText = await api.GenerateTextAsync(gptMessage);
            var result = JsonConvert.DeserializeObject<string[]>(responseText);
            return result!;
        }
    }
}
