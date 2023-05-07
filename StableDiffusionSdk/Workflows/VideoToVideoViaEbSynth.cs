using StableDiffusionSdk.Infrastructure;
using StableDiffusionSdk.Integrations.EbSynth;
using StableDiffusionSdk.Integrations.OpenAiGptApi;
using StableDiffusionSdk.Integrations.StableDiffusionWebUiApi;
using StableDiffusionSdk.Modules.Images;
using StableDiffusionSdk.Modules.Prompts;
using StableDiffusionSdk.Prompts;

namespace StableDiffusionSdk.Workflows
{
    public class VideoToVideoViaEbSynth
    {
        private readonly StableDiffusionApi _stableDiffusionApi;
        private readonly EbSynth _ebSynth;
        private readonly IPrompter _prompter;
        private readonly int _resolution;

        public VideoToVideoViaEbSynth(StableDiffusionApi stableDiffusionApi,  EbSynth ebSynth, IPrompter prompter, int resolution)
        {
            _stableDiffusionApi = stableDiffusionApi;
            _ebSynth = ebSynth;
            _prompter = prompter;
            _resolution = resolution;
        }

        public async Task Run(string inputVideoLocation, string outputFolder, int takeEveryXthFrame, int synthOnEachBatch)
        {
            var persistor = new ImagePersister(outputFolder);

            var persistStyle = new ImagePersister(Path.Combine(persistor.OutputFolder, "temp"));

            var frame = -1;
            List<string> buffer = new List<string>();
            foreach(var frameLocation in VideoProcessor.DisassembleVideoToFrames(inputVideoLocation, takeEveryXthFrame))
            {
                frame++;
                
                buffer.Add(frameLocation);
                if (frame % synthOnEachBatch != synthOnEachBatch -1)
                {
                    continue;
                }
                
                var batch = buffer.ToArray();
                buffer.Clear();

                var middlePath = batch[batch.Length / 2];
                var middle = await middlePath.ReadImage();
                var middleGuideImage = await middle.Resize(_resolution);
                var middleDefused = await _stableDiffusionApi.ImageToImage(new Img2ImgRequest(
                    InputImage: middleGuideImage!,
                    Prompt: await _prompter.GetPrompt(middleGuideImage),
                    DenoisingStrength: 0.35,
                    Seed.Random()
                ));

                var stylePath = await persistStyle.Persist(middleDefused);
                var middleGuidePath = await persistStyle.Persist(middleGuideImage);
                

                foreach (var path in batch)
                {
                    if (path == middlePath)
                    {
                        await persistor.Persist(middleDefused);
                        continue;
                    }
                    var image = await path.ReadImage();
                    var imageDownscaled = await image.Resize(_resolution);

                    var downscaledPath = await persistStyle.Persist(imageDownscaled);

                    var synth = await _ebSynth.RunEbsynth(stylePath, middleGuidePath, downscaledPath);
                    await persistor.Persist(synth);
                }
            }
        }
    }
}