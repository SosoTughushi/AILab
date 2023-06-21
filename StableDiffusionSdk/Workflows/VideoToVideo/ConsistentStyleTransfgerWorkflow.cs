using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StableDiffusionSdk.Utilities;
using StableDiffusionSdk.Utilities.Videos;
using StableDiffusionSdk.Workflows.Image2Video;
using StableDiffusionTools.Domain;
using StableDiffusionTools.ImageUtilities;
using StableDiffusionTools.Integrations.EbSynth;
using StableDiffusionTools.Integrations.StableDiffusionWebUi;

namespace StableDiffusionSdk.Workflows.VideoToVideo
{
    public class ConsistentStyleTransfgerWorkflow
    {
        private readonly StableDiffusionApi _stableDiffusionApi;
        private readonly EbSynth _ebSynth;

        public ConsistentStyleTransfgerWorkflow(
            StableDiffusionApi stableDiffusionApi,
            EbSynth ebSynth)
        {
            _stableDiffusionApi = stableDiffusionApi;
            _ebSynth = ebSynth;
        }

        public async Task Run(string sourceVideo, int resolution, int takeEveryXthFrame, int transferEachStyleToNFrames,
            Img2ImgRequestFactory createImg2ImgRequest)
        {
            var path = Path.Combine(Path.GetDirectoryName(sourceVideo)!,
                Path.GetFileNameWithoutExtension(sourceVideo));
            var stylesPersistor = new ImagePersister($"{path}/styles");

            foreach (var frameLocation in sourceVideo.DisassembleVideoToFrames(takeEveryXthFrame *
                                                                               transferEachStyleToNFrames))
            {
                var frame = await frameLocation.ReadImage();
                frame = await frame.Resize(resolution);
                var rdyStyle = await _stableDiffusionApi.ImageToImage(await createImg2ImgRequest(frame));
                await stylesPersistor.Persist(rdyStyle);
            }

            var stylesInterpolatedPersister = new ImagePersister($"{path}/stylesInterpolated");
            ImageDomainModel? previousStyle = null;

            foreach (var styleLocation in Directory.EnumerateFiles(stylesPersistor.OutputFolder))
            {
                var style = await styleLocation.ReadImage();

                if (previousStyle == null)
                {
                    await stylesInterpolatedPersister.Persist(style);
                    previousStyle = style;
                }
                else
                {
                    var interpolated = await ImageInterpolation.InterpolateAsync(previousStyle, style);
                    await stylesInterpolatedPersister.Persist(interpolated);
                    previousStyle = null;
                }

            }

            var resultPersistor = new ImagePersister(path);
            var counter = 0;
            var stylesLocationsArray = Directory.EnumerateFiles(stylesInterpolatedPersister.OutputFolder).ToArray();
            ImageDomainModel? previousFrame = null;
            foreach (var frameLocation in sourceVideo.DisassembleVideoToFrames(takeEveryXthFrame))
            {
                try
                {
                    var frame = await frameLocation.ReadImage();
                    frame = await frame.Resize(resolution);
                    var closestStyleLocation = stylesLocationsArray[counter / transferEachStyleToNFrames];
                    var closestStyle = await closestStyleLocation.ReadImage();

                    if (previousFrame == null)
                    {
                        previousFrame = frame;
                        await resultPersistor.Persist(closestStyle);
                        continue;
                    }

                    var synthed = await _ebSynth.TransferStyle(closestStyle, previousFrame, frame);
                    var result = await _stableDiffusionApi.ImageToImage(await createImg2ImgRequest(synthed) with {DenoisingStrength = 0.1});
                    await resultPersistor.Persist(result);

                }
                finally
                {
                    counter++;
                }
            }
        }
    }
}