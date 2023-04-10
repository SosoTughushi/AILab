using Newtonsoft.Json;
using StableDiffusionSdk.DomainModels;
using StableDiffusionSdk.Integrations.OpenAiGptApi;
using StableDiffusionSdk.Jobs.Interrogators;

namespace StableDiffusionSdk.Jobs.Prompters
{
    public static class GptPrompterWithInterrogation
    {
        private static string Template = @"
You are used within stable diffusion software to generate prompts for images. 
Prompts should be a small sentence that highlights the features of the image/painting that we are making stable diffusion to produce.
Prompts should be short and contain keywords describing the desired style of the image.

Examples of good prompts:
1) Space
Prompt: astronaut looking at a nebula , digital art , trending on artstation , hyperdetailed , matte painting , CGSociety

2) Universe
Prompt: Entering the Fifth Dimension. Photorealistic. Masterpiece.

3) Magic
Prompt: modern street magician holding playing cards, realistic, modern, intricate, elegant, highly detailed, digital painting, 
artstation, concept art, addiction, chains, smooth, sharp focus, illustration, art by ilja repin

4) Fantasy
Prompt: Hedgehog magus, gaia, nature, fairy, forest background, magic the gathering artwork, D&D, fantasy, cinematic lighting, centered, 
symmetrical, highly detailed, digital painting, artstation, concept art, smooth, sharp focus, illustration, volumetric lighting, epic Composition, 8k, art by 
Akihiko Yoshida and Greg Rutkowski and Craig Mullins, oil painting, cgsociety

5) Sci-Fi
Prompt: Futuristic cyberpunk cityscape, neon lights, rain, dark atmosphere, reflective surfaces, detailed, Blade Runner inspired

6) Nature
Prompt: Serene mountain landscape with a crystal-clear lake, vibrant colors, early morning light, peaceful, Bob Ross style

7) Steampunk
Prompt: Steampunk airship soaring above Victorian-era city, intricate machinery, brass and copper details, gears and cogs, concept art

8) Surrealism
Prompt: Surreal floating island with distorted structures, dreamlike, vivid colors, Dali-inspired, imaginative, otherworldly

9) Portrait
Prompt: Expressive portrait of a young woman, oil painting, Rembrandt style, chiaroscuro lighting, strong contrast, fine details

10) Architecture
Prompt: Majestic Gothic cathedral with intricate details, soaring towers, stained glass windows, historical, architectural masterpiece

You are given an image interrogation result and user instructions.
Your goal is to produce a prompt based on the interrogation result while strictly following the user instructions.


Follow the user instructions:
a) User might just specify thematics like Atchitecture, Space... Or user might give you situation: Dog standing on a balconee with a cigarette. 
b) If the user instruction contains 'Replace every object with [something]', look at the interrogation result and replace every keyword from it with the specified 'something'.
c) If the user instruction contains a trigger word, paste it unchanged in the response.

Reply with only the prompt! Nothing else.

Interrogated Image result: [{0}]
UserInput: [{1}]
";

        /// <summary>
        /// Consolidates the prompt and image interrogation result using the GPT API.
        /// The method follows user instructions provided in the image interrogation result. The rules are as follows:
        /// a) If user instruction contains "Choose randomly from those prompts: [prompt1], [prompt2], [prompt3]", the method will return one of the given options.
        /// b) If user instruction contains "Replace every object with [something]", the method will replace every keyword from the interrogation result with the specified "something".
        /// c) If user instruction contains a trigger word, the method will paste it unchanged in the result.
        /// </summary>
        /// <param name="api">The GPT API instance.</param>
        /// <param name="prompt">The prompt for image interrogation.</param>
        /// <param name="clipResult">The image interrogation result.</param>
        /// <returns>A Task that returns the consolidated string.</returns>
        public static async Task<string> Consolidate(this GptApi api, string prompt, string clipResult)
        {
            Console.WriteLine("==========================================");
            Console.WriteLine(prompt);
            Console.WriteLine();
            Console.WriteLine(clipResult);
            Console.WriteLine();
            var gptMessage = Template.Replace("{0}", clipResult).Replace("{1}", prompt);
            var consolidated = await api.GenerateTextAsync(gptMessage);

            Console.WriteLine(consolidated);

            return consolidated;
        }
    }
}
