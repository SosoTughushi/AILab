using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using StableDiffusionSdk.Prompts;
using StableDiffusionSdk.Utilities.Videos;
using StableDiffusionTools.Domain;
using StableDiffusionTools.ImageUtilities;
using StableDiffusionTools.Integrations.EbSynth;
using StableDiffusionTools.Integrations.StableDiffusionWebUi;

namespace StableDiffusionSdk.Workflows
{
    public class VideoToVideo2Workflow
    {
        private readonly StableDiffusionApi _stableDiffusionApi;
        private readonly EbSynth _ebSynth;
        private readonly IPrompter _prompter;

        public VideoToVideo2Workflow(StableDiffusionApi stableDiffusionApi, EbSynth ebSynth, IPrompter prompter)
        {
            _stableDiffusionApi = stableDiffusionApi;
            _ebSynth = ebSynth;
            _prompter = prompter;
        }

        public async Task Run(string videoPath, int frameXth, int size)
        {
            var firstFrameProcessed = false;
            var seed = Seed.Random();
            var persistor = new ImagePersister(Path.Combine(Path.GetDirectoryName(videoPath),
                Path.GetFileNameWithoutExtension(videoPath)));
            ImageDomainModel? previousFrameReal = null;
            ImageDomainModel? previousFrameDefused = null;
            foreach (var file in VideoProcessor.DisassembleVideoToFrames(videoPath, frameXth))
            {
                var currentFrame = await file.ReadImage();
                currentFrame = await currentFrame.Resize(size);

                if (!firstFrameProcessed)
                {
                    previousFrameReal = currentFrame;
                    previousFrameDefused = await _stableDiffusionApi.ImageToImage(new Img2ImgRequest(
                        InputImage: currentFrame,
                        Prompt: await _prompter.GetPrompt(currentFrame),
                        DenoisingStrength: 0.35,
                        Seed: seed
                    ));

                    await persistor.Persist(previousFrameDefused);
                    firstFrameProcessed = true;
                    continue;
                }

                //var transferredStyle =
                //    await _ebSynth.TransferStyle(previousFrameDefused!, previousFrameReal!, currentFrame);

                var defused = await _stableDiffusionApi.ImageToImage(new Img2ImgRequest(
                    InputImage: currentFrame,
                    Prompt: await _prompter.GetPrompt(currentFrame),
                    DenoisingStrength: 0.35,
                    Seed: seed
                ));

                await persistor.Persist(defused);

                previousFrameReal = currentFrame;
                previousFrameDefused = defused;
            }
        }
    }
}
