using StableDiffusionSdk.Infrastructure;
using StableDiffusionSdk.Integrations.OpenAiGptApi;
using StableDiffusionSdk.Integrations.StableDiffusionWebUiApi;
using StableDiffusionSdk.Modules.Images;
using StableDiffusionSdk.Modules.Prompts;

namespace StableDiffusionSdk.Workflows
{
    public class VideoToVideoWorkflow
    {
        private readonly StableDiffusionApi _stableDiffusionApi;
        private readonly GptApi _gptApi;

        public VideoToVideoWorkflow(StableDiffusionApi stableDiffusionApi, GptApi gptApi)
        {
            _stableDiffusionApi = stableDiffusionApi;
            _gptApi = gptApi;
        }

        public async Task Run(string inputVideoLocation, string outputFolder, int takeEveryXthFrame)
        {
            var persistor = new ImagePersister(outputFolder);
            foreach(var frame in VideoProcessor.DisassembleVideoToFrames(inputVideoLocation, takeEveryXthFrame))
            {
                var image = await frame.ReadImage();
                image = await image.Resize(ImageResolution._1408);
                var defused = await _stableDiffusionApi.ImageToImage(new Img2ImgRequest(
                    InputImage: image!,
                    Prompt: await CreatePrompt(image),
                    DenoisingStrength: 0.25,
                    Seed.Random()

                ));

                await persistor.Persist(defused);
            }
        }

        private int promptCount = 0;
        private string? promptCache;
        private async Task<string> CreatePrompt(ImageDomainModel image)
        {
            promptCount++;

            if (promptCache != null && promptCount % 10 != 0) return promptCache;
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

            var clip = await _stableDiffusionApi.Interrogate(new InterrogateRequest(image, InterrogationModel.Clip));
            promptCache = await _gptApi.Consolidate($"generate random word, trigger word [{styles.PickRandom()}]", clip);
            return promptCache;
        }
    }
}