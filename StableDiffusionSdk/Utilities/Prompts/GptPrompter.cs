using StableDiffusionSdk.Jobs;
using StableDiffusionTools.Domain;
using StableDiffusionTools.Integrations.OpenAi;

namespace StableDiffusionSdk.Utilities.Prompts
{
    public class GptPrompter : IJob<(ImageDomainModel, string), string>
    {
        private readonly GptApi _api;

        private static string Template =
            @"
Guide of how to make good prompts:

// start of guide

Stable Diffusion is not like Midjourney or other popular image generation software, you can't just ask it what you want. You have to be specific. Very specific.
Most people have found a prompt that works for them and they swear by it, often recommended by other people. I will show you my own personal example of a prompt and negative prompt:

Anime

2d, masterpiece, best quality, anime, highly detailed face, highly detailed background, perfect lighting
EasyNegative, worst quality, low quality, 3d, realistic, photorealistic, (loli, child, teen, baby face), zombie, animal, multiple views, text, watermark, signature, artist name, artist logo, censored
Photorealism

best quality, 4k, 8k, ultra highres, raw photo in hdr, sharp focus, intricate texture, skin imperfections, photograph of
EasyNegative, worst quality, low quality, normal quality, child, painting, drawing, sketch, cartoon, anime, render, 3d, blurry, deformed, disfigured, morbid, mutated, bad anatomy, bad art

// end of guide

Now, create stable diffusion prompt out of following description:

{0}
";


        public GptPrompter(
            GptApi api)
        {
            _api = api;
        }
        public async Task<string> Run((ImageDomainModel, string) parameters)
        {

            var gptMessage = string.Format(Template, parameters.Item2);
            var consolidated = await _api.GenerateTextAsync(gptMessage);

            Console.WriteLine();
            Console.WriteLine(consolidated);
            Console.WriteLine();

            return consolidated;
        }
    }
}
