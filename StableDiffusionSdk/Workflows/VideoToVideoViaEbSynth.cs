using StableDiffusionSdk.Infrastructure;
using StableDiffusionSdk.Integrations.EbSynth;
using StableDiffusionSdk.Integrations.OpenAiGptApi;
using StableDiffusionSdk.Integrations.StableDiffusionWebUiApi;
using StableDiffusionSdk.Modules.Images;
using StableDiffusionSdk.Modules.Prompts;

namespace StableDiffusionSdk.Workflows
{
    public class VideoToVideoViaEbSynth
    {
        private readonly StableDiffusionApi _stableDiffusionApi;
        private readonly GptApi _gptApi;
        private readonly EbSynth _ebSynth;
        private readonly int _resolution;

        public VideoToVideoViaEbSynth(StableDiffusionApi stableDiffusionApi, GptApi gptApi, EbSynth ebSynth, int resolution)
        {
            _stableDiffusionApi = stableDiffusionApi;
            _gptApi = gptApi;
            _ebSynth = ebSynth;
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
                var middleGuideImage = await middle.Resize(_resolution, _resolution);
                var middleDefused = await _stableDiffusionApi.ImageToImage(new Img2ImgRequest(
                    InputImage: middleGuideImage!,
                    Prompt: await CreatePrompt(middleGuideImage),
                    DenoisingStrength: 0.25,
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
                    var imageDownscaled = await image.Resize(_resolution, _resolution);

                    var downscaledPath = await persistStyle.Persist(imageDownscaled);

                    var synth = await _ebSynth.RunEbsynth(stylePath, middleGuidePath, downscaledPath);
                    await persistor.Persist(synth);
                }
            }
        }

        private int promptCount = 0;
        private string? promptCache;

        private async Task<string> CreatePrompt(ImageDomainModel image)
        {
            promptCount++;

            if (promptCache != null && promptCount % 2 != 0) return promptCache;
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
            promptCache =
                await _gptApi.Consolidate($"trigger word [{styles.PickRandom()}]", clip);
            return promptCache;
        }
    }
}