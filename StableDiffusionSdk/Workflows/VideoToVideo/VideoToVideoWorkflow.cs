using StableDiffusionSdk.Prompts;
using StableDiffusionSdk.Utilities.Videos;
using StableDiffusionSdk.Workflows.Image2Video;
using StableDiffusionTools.Domain;
using StableDiffusionTools.ImageUtilities;
using StableDiffusionTools.Integrations.StableDiffusionWebUi;

namespace StableDiffusionSdk.Workflows.VideoToVideo
{
    public class VideoToVideoWorkflow
    {
        private readonly StableDiffusionApi _stableDiffusionApi;

        public VideoToVideoWorkflow(StableDiffusionApi stableDiffusionApi)
        {
            _stableDiffusionApi = stableDiffusionApi;
        }

        public async Task Run(string inputVideoLocation, int takeEveryXthFrame, int rezolution,
            Img2ImgRequestFactory img2ImgRequestFactory)
        {
            var persistor = new ImagePersister(Path.Combine(Path.GetDirectoryName(inputVideoLocation)!,
                Path.GetFileNameWithoutExtension(inputVideoLocation)));
            foreach (var frame in inputVideoLocation.DisassembleVideoToFrames(takeEveryXthFrame))
            {
                var image = await frame.ReadImage();
                image = await image.Resize(rezolution, rezolution);

                try
                {
                    var request = await img2ImgRequestFactory(image);
                    var defused = await _stableDiffusionApi.ImageToImage(request);

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