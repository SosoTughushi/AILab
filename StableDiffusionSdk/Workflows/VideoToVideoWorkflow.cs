using StableDiffusionSdk.Modules.Images;
using StableDiffusionSdk.Prompts;
using StableDiffusionSdk.Utilities.Images;
using StableDiffusionSdk.Utilities.Videos;
using StableDiffusionTools.Integrations.StableDiffusionWebUi;

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

        public async Task Run(string inputVideoLocation, string outputFolder, int takeEveryXthFrame, int rezolution)
        {
            var persistor = new ImagePersister(outputFolder);
            foreach(var frame in VideoProcessor.DisassembleVideoToFrames(inputVideoLocation, takeEveryXthFrame))
            {
                var image = await frame.ReadImage();
                image = await image.Resize(rezolution, rezolution);

                try
                {
                    var defused = await _stableDiffusionApi.ImageToImage(new Img2ImgRequest(
                        InputImage: image!,
                        Prompt: await _prompter.GetPrompt(image),
                        NegativePrompt:"text, letters, comic characters, numbers",
                        DenoisingStrength: 0.35,
                        Seed: Seed.Random()

                    ));

                    await persistor.Persist(defused);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }
        
    }
}