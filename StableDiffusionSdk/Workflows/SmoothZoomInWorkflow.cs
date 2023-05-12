using StableDiffusionSdk.Infrastructure;
using StableDiffusionSdk.Jobs;
using StableDiffusionSdk.Prompts;
using StableDiffusionTools.Domain;
using StableDiffusionTools.ImageUtilities;
using StableDiffusionTools.Integrations.StableDiffusionWebUi;

namespace StableDiffusionSdk.Workflows;

public delegate Task<Img2ImgRequest> Img2ImgRequestFactory(ImageDomainModel imageDomainModel);

public class SmoothZoomInWorkflow
{
    private readonly StableDiffusionApi _stableDiffusionApi;
    private readonly Img2ImgRequestFactory _img2ImgRequestFactory;

    public SmoothZoomInWorkflow(StableDiffusionApi stableDiffusionApi, Img2ImgRequestFactory img2ImgRequestFactory)
    {
        _stableDiffusionApi = stableDiffusionApi;
        _img2ImgRequestFactory = img2ImgRequestFactory;
    }

    public async Task<ImageDomainModel> Run(string path, int rezolution, double zoomPercent, double zoomInCount, double denoisingStrength, int middleStepCount)
    {
        var baseOutputFolder = Path.Combine(Path.GetDirectoryName(path)!,
            Path.GetFileNameWithoutExtension(path));
        var _persistor = new ImagePersister(baseOutputFolder);
        var jsonWriter = new JsonWriter(_persistor.OutputFolder);

            

        //var zoomDirection = ZoomDirectionBuilder.Right(21.3).Bottom(12.5);
        var zoomDirection = ZoomDirectionBuilder.Right(10).Bottom(10);

        var input = await path.ReadImage();

        var preview = await input.PreviewZoom(zoomPercent, zoomDirection, 40);
        await _persistor.Persist(preview);

        input = await input.Resize(rezolution);

        ImageDomainModel lastResult;
        var regulator = new RgbRegulator();
        for (var recursionCount = 0; recursionCount < zoomInCount; recursionCount++)
        {
            
            var seed = Seed.Random();

            var zoomDelta = zoomPercent - 100;
            var zoomDeltaEachStep = zoomDelta / middleStepCount;
            double denoisingStrengthStep = denoisingStrength / middleStepCount;

            var result = input;
            for (var i = 1; i <= middleStepCount; i++)
            {
                var zoomed = await input.Zoom(100 + zoomDeltaEachStep * i, zoomDirection);

                // var regulated = await regulator.Regulate(zoomed);
                var regulated = zoomed;

                var ds = Math.Round(denoisingStrengthStep * i, 3);

                var request = await _img2ImgRequestFactory(regulated);
                await jsonWriter.Write(request);

                request = request with
                {
                    Seed = seed,
                    DenoisingStrength = ds
                };
                result = await _stableDiffusionApi.ImageToImage(request);

                await _persistor.Persist(result);
            }

            input = result;
        }

        return input;
    }
}