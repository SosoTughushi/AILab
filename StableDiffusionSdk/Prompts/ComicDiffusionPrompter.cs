using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StableDiffusionSdk.Infrastructure;
using StableDiffusionSdk.Integrations.OpenAiGptApi;
using StableDiffusionSdk.Integrations.StableDiffusionWebUiApi;
using StableDiffusionSdk.Modules.Images;

namespace StableDiffusionSdk.Prompts
{
    public class ComicDiffusionPrompter : IPrompter
    {
        private readonly GptApi _gptApi;
        private readonly StableDiffusionApi _stableDiffusionApi;

        public ComicDiffusionPrompter(GptApi gptApi, StableDiffusionApi stableDiffusionApi)
        {
            _gptApi = gptApi;
            _stableDiffusionApi = stableDiffusionApi;
        }
        public async Task<string> GetPrompt(ImageDomainModel image)
        {
            var styles = new[]
            {
                "charliebo artstyle",
                "holliemengert artstyle",
                "marioalberti artstyle",
                "pepelarraz artstyle",
                "andreasrocha artstyle",
                "jamesdaly artstyle",
                "comicmay artsyle"
            };
            var randomStyle = styles.PickRandom();
            var gptPrompter = new GptPrompter(_gptApi, _stableDiffusionApi, $"trigger word [{randomStyle}]");
            return await gptPrompter.GetPrompt(image);
        }
    }
}
