
using System.Runtime.InteropServices;
using StableDiffusionSdk.Jobs;
using StableDiffusionSdk.Prompts;
using StableDiffusionTools.Domain;
using StableDiffusionTools.ImageUtilities;
using StableDiffusionTools.Integrations.StableDiffusionWebUi;

namespace StableDiffusionSdk.Workflows;

public class StretchWorkflow
{
    private readonly StableDiffusionApi _stableDiffusionApi;
    private readonly IPrompter _prompter;

    public StretchWorkflow(StableDiffusionApi stableDiffusionApi, IPrompter prompter)
    {
        _stableDiffusionApi = stableDiffusionApi;
        _prompter = prompter;
    }
    public async Task Run( string file)
    {
        var persister =
            new ImagePersister(Path.Combine(Path.GetDirectoryName(file)!,
                Path.GetFileNameWithoutExtension(file)));

        var input = await file.ReadImage();
        input = await input.Resize(ImageResolution._1024);

        var rgbRegulator = new RgbRegulator();

        for (var i = 0; i < 30; i++)
        {
            try
            {
                var stretched = await input.LogarithmicWarpLeft(0.25);
                var defused = await _stableDiffusionApi.ImageToImage(
                    new Img2ImgRequest(
                        InputImage: stretched,
                        Prompt: await _prompter.GetPrompt(stretched),
                        DenoisingStrength: 0.2,
                        Seed.Random(),
                        NegativePrompt: string.Empty)
                );
                await persister.Persist(defused);
                input = defused;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return;
            }
        }

    }
}