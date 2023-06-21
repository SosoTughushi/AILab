using StableDiffusionSdk.Prompts;
using StableDiffusionSdk.Utilities.Videos;
using StableDiffusionTools.Domain;
using StableDiffusionTools.ImageUtilities;
using StableDiffusionTools.Integrations.EbSynth;
using StableDiffusionTools.Integrations.StableDiffusionWebUi;

namespace StableDiffusionSdk.Workflows.VideoToVideo;

public class VideoToVideoViaEbSynth
{
    private readonly StableDiffusionApi _stableDiffusionApi;
    private readonly EbSynth _ebSynth;
    private readonly IPrompter _prompter;

    public VideoToVideoViaEbSynth(StableDiffusionApi stableDiffusionApi, EbSynth ebSynth,
        IPrompter prompter)
    {
        _stableDiffusionApi = stableDiffusionApi;
        _ebSynth = ebSynth;
        _prompter = prompter;
    }

    public async Task Run(string inputVideoLocation, string outputFolder, int takeEveryXthFrame, int synthOnEachBatch, int resolution)
    {
        var persistor = new ImagePersister(outputFolder);

        var persistStyle = new ImagePersister(Path.Combine(persistor.OutputFolder, "temp"));

        var frame = -1;
        List<string> buffer = new List<string>();
        foreach (var frameLocation in inputVideoLocation.DisassembleVideoToFrames(takeEveryXthFrame))
        {
            frame++;

            buffer.Add(frameLocation);
            if (frame % synthOnEachBatch != synthOnEachBatch - 1)
            {
                continue;
            }

            var batch = buffer.ToArray();
            buffer.Clear();

            var middlePath = batch[batch.Length / 2];
            var middle = await middlePath.ReadImage();
            var middleGuideImage = await middle.Resize(resolution);
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
                var imageDownscaled = await image.Resize(resolution);

                var downscaledPath = await persistStyle.Persist(imageDownscaled);

                var synth = await _ebSynth.RunEbsynth(stylePath, middleGuidePath, downscaledPath);
                await persistor.Persist(synth);
            }
        }
    }
}