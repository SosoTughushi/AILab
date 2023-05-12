using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accord.Imaging;
using StableDiffusionSdk.Utilities.Videos;
using StableDiffusionTools.ImageUtilities;
using StableDiffusionTools.Integrations.OpenAi;
using StableDiffusionTools.Integrations.StableDiffusionWebUi;

namespace StableDiffusionSdk.Workflows
{
    public class YoutubeSubtitleGenerator
    {
        private static readonly string Template = "";


        private readonly GptApi _gptApi;
        private readonly StableDiffusionApi _stableDiffusionApi;

        public YoutubeSubtitleGenerator(GptApi gptApi, StableDiffusionApi stableDiffusionApi)
        {
            _gptApi = gptApi;
            _stableDiffusionApi = stableDiffusionApi;
        }

        public async Task<string> Run(string videoFile, int everyXthSecond)
        {
            var count = 0;
            var frameDescription = new List<(string, int)>();
            foreach (var image in VideoProcessor.DisassembleVideoToFrames(videoFile, 30 * everyXthSecond))
            {
                var second = everyXthSecond * count;

                var interrogated =
                    await _stableDiffusionApi.Interrogate(new InterrogateRequest(await image.ReadImage(), InterrogationModel.Clip));

                frameDescription.Add((interrogated, second));

                count++;
            }

            var result = await _gptApi.GenerateTextAsync("");
            return result;
        }
    }
}
