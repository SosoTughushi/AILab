using StableDiffusionSdk.Infrastructure;
using StableDiffusionSdk.Integrations.OpenAiGptApi;
using StableDiffusionSdk.Integrations.StableDiffusionWebUiApi;
using StableDiffusionSdk.Modules.Images;
using StableDiffusionSdk.Modules.Prompts;
using StableDiffusionSdk.Prompts;

namespace StableDiffusionSdk.Workflows
{
    public class VideoToVideoWorkflow
    {
        private readonly StableDiffusionApi _stableDiffusionApi;
        private readonly IPrompter _prompter;

        public VideoToVideoWorkflow(StableDiffusionApi stableDiffusionApi, IPrompter prompter)
        {
            _stableDiffusionApi = stableDiffusionApi;
            _prompter = prompter;
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
                    Prompt: await _prompter.GetPrompt(image),
                    DenoisingStrength: 0.25,
                    Seed.Random()

                ));

                await persistor.Persist(defused);
            }
        }
        
    }
}