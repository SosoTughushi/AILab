using StableDiffusionSdk.Utilities.Videos;
using StableDiffusionTools.Domain;
using StableDiffusionTools.ImageUtilities;
using StableDiffusionTools.Integrations.OpenAi;
using StableDiffusionTools.Integrations.StableDiffusionWebUi;

namespace StableDiffusionSdk.Workflows
{
    public class YoutubeSubtitleGenerator
    {
        private static readonly string Template = "todo: fill me";


        private readonly GptApi _gptApi;
        private readonly StableDiffusionApi _stableDiffusionApi;
        private readonly int _resolution;

        public YoutubeSubtitleGenerator(GptApi gptApi, StableDiffusionApi stableDiffusionApi, int resolution)
        {
            _gptApi = gptApi;
            _stableDiffusionApi = stableDiffusionApi;
            _resolution = resolution;
        }

        public async Task Run(string videoFile, int everyXthSecond)
        {
            var count = 0;
            var interrogationResults = new List<(string, int)>();
            foreach (var image in VideoProcessor.DisassembleVideoToFrames(videoFile, 30 * everyXthSecond))
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
                    Width: _resolution,
                    Height: _resolution
                ));

                await persistor.Persist(generated);
            }
        }
    }
}
