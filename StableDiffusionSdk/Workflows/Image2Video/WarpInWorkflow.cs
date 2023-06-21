using System.Runtime.InteropServices;
using StableDiffusionSdk.Jobs;
using StableDiffusionSdk.Prompts;
using StableDiffusionTools.Domain;
using StableDiffusionTools.ImageUtilities;
using StableDiffusionTools.Integrations.StableDiffusionWebUi;

namespace StableDiffusionSdk.Workflows.Image2Video;

public class WarpInWorkflow
{
    private readonly StableDiffusionApi _stableDiffusionApi;

    public WarpInWorkflow(StableDiffusionApi stableDiffusionApi)
    {
        _stableDiffusionApi = stableDiffusionApi;
    }

    public async Task<ImageDomainModel> Run(string file, int resolution, WarpDirection direction, double zoomPercent, Img2ImgRequestFactory img2ImgRequestFactory)
    {
        var persister =
            new ImagePersister(Path.Combine(Path.GetDirectoryName(file)!,
                Path.GetFileNameWithoutExtension(file)));

        var input = await file.ReadImage();
        input = await input.Resize(resolution);

        var rgbRegulator = new RgbRegulator();

        for (var i = 0; i < 20; i++)
        {
            var warped = await input.LogarithmicWarp(direction);

            var adjusted = await rgbRegulator.Regulate(warped);

            var zoomed = await adjusted.Zoom(zoomPercent, new ZoomInDirection(0, 0));

            var request = await img2ImgRequestFactory(zoomed);

            request = request with
            {
                Seed = Seed.Random()
            };

            var defused = await _stableDiffusionApi.ImageToImage(request);

            await persister.Persist(defused);
            input = defused;
        }

        return input;
    }
}