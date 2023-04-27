using StableDiffusionSdk.Infrastructure;
using StableDiffusionSdk.Integrations.StableDiffusionWebUiApi;
using StableDiffusionSdk.Modules.Images;

namespace StableDiffusionSdk.Workflows
{
    public class VideoToVideoWorkflow
    {
        private readonly StableDiffusionApi _stableDiffusionApi;

        public VideoToVideoWorkflow(StableDiffusionApi stableDiffusionApi)
        {
            _stableDiffusionApi = stableDiffusionApi;
        }

        public async Task Run(string inputVideoLocation, string outputFolder, int takeEveryXthFrame)
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
            
            var persistor = new ImagePersister(outputFolder);
            foreach(var frame in VideoProcessor.DisassembleVideoToFrames(inputVideoLocation, takeEveryXthFrame))
            {
                var image = await frame.ReadImage();
                image = await image.Resize(ImageResolution._1408);
                var defused = await _stableDiffusionApi.ImageToImage(new Img2ImgRequest(
                    InputImage: image!,
                    Prompt: $"man and woman next to eachother standing on top of the hill, arcane style",
                    DenoisingStrength: 0.2,
                    Seed.Random()

                ));

                await persistor.Persist(defused);
            }
        }
    }
}