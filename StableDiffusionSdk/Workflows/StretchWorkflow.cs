
using StableDiffusionSdk.Jobs;
using StableDiffusionSdk.Modules.Images;
using StableDiffusionSdk.Prompts;
using StableDiffusionSdk.Utilities.Images;
using StableDiffusionTools.Domain;
using StableDiffusionTools.Integrations.StableDiffusionWebUi;

namespace StableDiffusionSdk.Workflows;

public static class StretchWorkflow
{
    public static async Task Run(StableDiffusionApi stableDiffusionApi, string file, IPrompter prompter)
    {
        var persister =
            new ImagePersister(Path.Combine(Path.GetDirectoryName(file)!,
                Path.GetFileNameWithoutExtension(file)));

        var rgbRegulator = new RgbRegulator();
        var direction = new StretchDirection(10, 0);

        var img2ImgJob = DynamicJob.Create(async (ImageDomainModel image) =>
            {
                var result = await stableDiffusionApi.ImageToImage(
                    new Img2ImgRequest(
                        InputImage: image,
                        Prompt: await prompter.GetPrompt(image),
                        DenoisingStrength: 0.3,
                        Seed.Random(),
                        NegativePrompt: string.Empty)
                );

                await persister.Persist(result);
                return result;
            })
            // .MapParameter((ImageDomainModel image) => image.Stretch(new StretchDirection(10, 0), new CropPosition(0.5, 0.5)))
            //.MapParameter((ImageDomainModel image) => rgbRegulator.Regulate(image))
            //.MapParameter((ImageDomainModel image) => image.Zoom(102, ZoomDirectionBuilder.Top(20)))
            .MapParameter((ImageDomainModel image) => image.Resize(ImageResolution._1472));
            
        try
        {
            var img = await file.ReadImage();
            await img2ImgJob.LoopRecursively(50).Run(img);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}