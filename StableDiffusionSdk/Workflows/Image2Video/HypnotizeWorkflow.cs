using StableDiffusionSdk.Jobs;
using StableDiffusionSdk.Prompts;
using StableDiffusionTools.Domain;
using StableDiffusionTools.ImageUtilities;
using StableDiffusionTools.Integrations.StableDiffusionWebUi;

namespace StableDiffusionSdk.Workflows.Image2Video
{
    public static class HypnotizeWorkflow
    {
        public static async Task Run(StableDiffusionApi stableDiffusionApi,
            ImagePersister globalPersister, IPrompter prompter)
        {


            var file = @"D:\Stable Diffusion\Recursive\\NIghtJob\hypnotize.jpg";
            var persister =
                new ImagePersister(Path.Combine(Path.GetDirectoryName(file)!,
                    Path.GetFileNameWithoutExtension(file)));

            var zooomStep = 140.0;
            var zoomedPercent = 0.0;
            var rgbRegulator = new RgbRegulator();

            var counter = 0;

            var prompt = string.Empty;
            var direction = ZoomDirectionBuilder.Bottom(45).Right(13);
            var img2Img =
                    DynamicJob.Create(async (ImageDomainModel image) =>
                        {
                            counter++;
                            var strength = 0.3;
                            var inBetweenCount = 3;

                            if (prompt == string.Empty)
                            {
                                prompt = await prompter.GetPrompt(image);
                            }

                            var seed = Seed.Random();

                            ImageDomainModel inBetweenResult = null!;
                            for (var i = 1; i <= inBetweenCount; i++)
                            {
                                var inBetweenStrength = Math.Round(strength / inBetweenCount * i, 3);
                                var inBetweenZoom = Math.Round((zooomStep - 100) / inBetweenCount * i + 100, 3);

                                inBetweenResult = await stableDiffusionApi.ImageToImage(
                                    new Img2ImgRequest(
                                        InputImage: await image.Zoom(inBetweenZoom, direction),
                                        Prompt: prompt,
                                        DenoisingStrength: inBetweenStrength,
                                        seed,
                                        NegativePrompt:
                                        ""));


                                await persister.Persist(inBetweenResult);
                                await globalPersister.Persist(inBetweenResult);

                                Console.WriteLine(inBetweenZoom);
                            }

                            return inBetweenResult!;
                        })
                        .MapParameter((ImageDomainModel image) =>
                            image.Resize(ImageResolution._1408, ImageResolution._1408))
                        // .MapParameter((ImageDomainModel image) => image.Zoom(zooomStep, direction))
                        .MapParameter((ImageDomainModel image) => rgbRegulator.Regulate(image))
                //.MapResult(image => persister.Persist(image))
                //.MapResult(image => globalPersister.Persist(image))
                ;
            for (var i = 0; i < 20; i++)
            {
                try
                {
                    await img2Img.LoopRecursively(12).Run(await file.ReadImage());
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }
    }
}