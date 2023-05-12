using System.Runtime.InteropServices;
using StableDiffusionSdk.Jobs;
using StableDiffusionSdk.Prompts;
using StableDiffusionTools.Domain;
using StableDiffusionTools.ImageUtilities;
using StableDiffusionTools.Integrations.StableDiffusionWebUi;

namespace StableDiffusionSdk.Workflows;

public class WarpInWorkflow
{
    private readonly StableDiffusionApi _stableDiffusionApi;
    private readonly Img2ImgRequestFactory _img2ImgRequestFactory;
    private readonly int _resolution;

    public WarpInWorkflow(StableDiffusionApi stableDiffusionApi, Img2ImgRequestFactory img2ImgRequestFactory, int resolution)
    {
        _stableDiffusionApi = stableDiffusionApi;
        _img2ImgRequestFactory = img2ImgRequestFactory;
        _resolution = resolution;
    }

    public async Task<ImageDomainModel> Run(string file, WarpDirection direction, double zoomPercent)
    {
        var persister =
            new ImagePersister(Path.Combine(Path.GetDirectoryName(file)!,
                Path.GetFileNameWithoutExtension(file)));

        var input = await file.ReadImage();
        input = await input.Resize(_resolution);

        var rgbRegulator = new RgbRegulator();

        for (var i = 0; i < 20; i++)
        {
            var warped = await input.LogarithmicWarp(direction);

            var adjusted = await rgbRegulator.Regulate(warped);

            var zoomed = await adjusted.Zoom(zoomPercent, new ZoomInDirection(0, 0));

            var request = await _img2ImgRequestFactory(zoomed);

            request = request with
            {
                DenoisingStrength = 0.25,
                Seed = Seed.Random()
            };

            var defused = await _stableDiffusionApi.ImageToImage(request);

            await persister.Persist(defused);
            input = defused;
        }

        return input;
    }
}