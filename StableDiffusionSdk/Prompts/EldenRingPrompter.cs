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
    public class EldenRingPrompter : IPrompter
    {
        private readonly GptApi _gptApi;
        private readonly StableDiffusionApi _stableDiffusionApi;
        private readonly string _prompt;

        public EldenRingPrompter(GptApi gptApi, StableDiffusionApi stableDiffusionApi, string prompt)
        {
            _gptApi = gptApi;
            _stableDiffusionApi = stableDiffusionApi;
            _prompt = prompt;
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
            var gptPrompter = new GptPrompter(_gptApi, _stableDiffusionApi, $"{_prompt}, trigger word: [elden ring style]");
            return await gptPrompter.GetPrompt(image);
        }
    }
}
