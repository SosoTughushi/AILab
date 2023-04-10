using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StableDiffusionSdk.DomainModels;
using StableDiffusionSdk.Infrastructure;
using StableDiffusionSdk.Integrations.OpenAiGptApi;
using StableDiffusionSdk.Jobs;
using StableDiffusionSdk.Jobs.Image;
using StableDiffusionSdk.Jobs.Prompters;

namespace StableDiffusionSdk.Workflows
{
    public static class HypnotizeWorkflow
    {
        public static async Task Run(StableDiffusionApi.StableDiffusionApi stableDiffusionApi, GptApi gptApi,
            ImagePersister globalPersister)
        {
            async Task<string> CreatePrompt(ImageDomainModel image)
            {
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

                var s = $@"
Jack white wearing bright red shirt holding and showing photo of jack white holding and showing photo of jack white. 
3 colors: white red black, Bright red circles in the background, looking up, Recursive, Fractal, sharp, trigger word: ""{styles[new Random().Next(styles.Length)]}""";

                var interrogated = await stableDiffusionApi.Interrogate(new InterrogateRequest(image));
                var gptPrompt = await gptApi.Consolidate(s, interrogated);
                Console.WriteLine(gptPrompt);
                return gptPrompt;
            }


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
                                prompt = await CreatePrompt(image);
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