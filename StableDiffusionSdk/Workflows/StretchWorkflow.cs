using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StableDiffusionSdk.DomainModels;
using StableDiffusionSdk.Infrastructure;
using StableDiffusionSdk.Jobs;
using StableDiffusionSdk.Jobs.Image;

namespace StableDiffusionSdk.Workflows
{
    public static class StretchWorkflow
    {
        public static async Task Run(StableDiffusionApi.StableDiffusionApi stableDiffusionApi, string file)
        {
            var persister =
                new ImagePersister(Path.Combine(Path.GetDirectoryName(file)!,
                    Path.GetFileNameWithoutExtension(file)));

            var rgbRegulator = new RgbRegulator();
            var direction = new StretchDirection(10, 0);
            var prompt = "man with kaleidoscope glasses, marioalberti artstyle";

            var img2ImgJob = DynamicJob.Create(async (ImageDomainModel image) =>
                {
                    var result = await stableDiffusionApi.ImageToImage(
                        new Img2ImgRequest(
                            InputImage: image,
                            Prompt: prompt,
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
}