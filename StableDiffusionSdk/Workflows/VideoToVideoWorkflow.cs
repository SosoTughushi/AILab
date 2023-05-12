using StableDiffusionSdk.Prompts;
using StableDiffusionSdk.Utilities.Videos;
using StableDiffusionTools.Domain;
using StableDiffusionTools.ImageUtilities;
using StableDiffusionTools.Integrations.StableDiffusionWebUi;

namespace StableDiffusionSdk.Workflows
{
    public class VideoToVideoWorkflow
    {
        private readonly StableDiffusionApi _stableDiffusionApi;
        private readonly Img2ImgRequestFactory _img2ImgRequestFactory;

        public VideoToVideoWorkflow(StableDiffusionApi stableDiffusionApi, Img2ImgRequestFactory img2ImgRequestFactory)
        {
            _stableDiffusionApi = stableDiffusionApi;
            _img2ImgRequestFactory = img2ImgRequestFactory;
        }

        public async Task Run(string inputVideoLocation, int takeEveryXthFrame, int rezolution)
        {
            var persistor = new ImagePersister(Path.Combine(Path.GetDirectoryName(inputVideoLocation)!,
                Path.GetFileNameWithoutExtension(inputVideoLocation)));
            foreach (var frame in VideoProcessor.DisassembleVideoToFrames(inputVideoLocation, takeEveryXthFrame))
            {
                var image = await frame.ReadImage();
                image = await image.Resize(rezolution, rezolution);

                try
                {
                    var request = await _img2ImgRequestFactory(image);
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