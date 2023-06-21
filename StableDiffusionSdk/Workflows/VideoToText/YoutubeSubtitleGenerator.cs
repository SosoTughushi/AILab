using StableDiffusionSdk.Utilities.Videos;
using StableDiffusionTools.Domain;
using StableDiffusionTools.ImageUtilities;
using StableDiffusionTools.Integrations.OpenAi;
using StableDiffusionTools.Integrations.StableDiffusionWebUi;

namespace StableDiffusionSdk.Workflows.VideoToText
{
    public class YoutubeSubtitleGenerator
    {
        private static readonly string Template = "todo: fill me";

        
        private readonly GptApi _gptApi;
        private readonly StableDiffusionApi _stableDiffusionApi;

        public YoutubeSubtitleGenerator(GptApi gptApi, StableDiffusionApi stableDiffusionApi)
        {
            _gptApi = gptApi;
            _stableDiffusionApi = stableDiffusionApi;
        }

        public async Task Run(string videoFile, int everyXthSecond, int resolution)
        {
            var count = 0;
            var interrogationResults = new List<(string, int)>();
            foreach (var image in videoFile.DisassembleVideoToFrames(30 * everyXthSecond))
            {
                var second = everyXthSecond * count;

                var interrogated =
                    await _stableDiffusionApi.Interrogate(new InterrogateRequest(await image.ReadImage(), InterrogationModel.Clip));

                interrogationResults.Add((interrogated, second));

                count++;
            }

            var persistor = new ImagePersister(Path.Combine(Path.GetDirectoryName(videoFile)!,
                Path.GetFileNameWithoutExtension(videoFile)));

            foreach (var interrogationResult in interrogationResults)
            {
                var prompt = await _gptApi.GenerateTextAsync(
                    string.Format(
                        Template,
                        interrogationResult.Item1,
                        interrogationResult.Item2)
                    );

                var generated = await _stableDiffusionApi.TextToImage(new Text2ImgRequest(
                    Prompt: prompt,
                    Seed.Random(),
                    Width: resolution,
                    Height: resolution
                ));

                await persistor.Persist(generated);
            }
        }
    }
}
