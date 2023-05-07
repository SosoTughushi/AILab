using StableDiffusionSdk.Jobs;
using StableDiffusionTools.Domain;
using StableDiffusionTools.ImageUtilities;

namespace StableDiffusionSdk.Workflows
{
    public static class ImageToImageJobExtensions
    {
        public static IJob<ImageDomainModel, ImageDomainModel> Create()
        {
            return DynamicJob.Create((ImageDomainModel img) => Task.FromResult(img));
        }

        public static IJob<ImageDomainModel, ImageDomainModel> LoopRecursively(
            this IJob<ImageDomainModel, ImageDomainModel> imgToImgJob, int steps)
        {
            var img2Img = imgToImgJob;

            for (var i = 0; i < steps - 1; i++)
            {
                img2Img = img2Img.MapResult(imgToImgJob.Run);
            }

            return img2Img;
        }


        public static IJob<ImageDomainModel, ImageDomainModel> ZoomInSmoothly(
            this IJob<ImageDomainModel, ImageDomainModel> image2Image,
            double zoomPercent,
            ZoomInDirection zoomInDirection,
            int stepCount)
        {
            var zoomDelta = zoomPercent - 100;
            var zoomStep = zoomDelta / stepCount;
            
            return DynamicJob.Create(async (ImageDomainModel img) =>
            {
                var result = img;
                for (var i = 1; i <= stepCount; i++)
                {
                    var zoom = 100 + zoomStep * i;
                    result = await image2Image.Run(await img.Zoom(zoom, zoomInDirection));
                }

                return result;
            });
        }

        

        public static IJob<ImageDomainModel, ImageDomainModel> ZoomInSmoothly(
            this IJob<ImageDomainModel, ImageDomainModel> image2Image,
            double zoomPercent,
            ZoomInDirection zoomInDirection,
            int stepCount,
            ImagePersister persistor)
        {
            var zoomDelta = zoomPercent - 100;
            var zoomStep = zoomDelta / stepCount;

            return image2Image.MapResult(async img =>
            {
                var result = img;
                for (var i = 1; i <= stepCount; i++)
                {
                    var zoom = 100 + zoomStep * i;
                    result = await img.Zoom(zoom, zoomInDirection);
                    await persistor.Persist(result);
                }
                return result;
            });
            
            return DynamicJob.Create(async (ImageDomainModel img) =>
            {
                var result = img;
                for (var i = 1; i <= stepCount; i++)
                {
                    var zoom = 100 + zoomStep * i;
                    result = await image2Image.Run(await img.Zoom(zoom, zoomInDirection));
                }

                return result;
            });
        }

        public static IJob<ImageDomainModel, ImageDomainModel> RegulateRgb(
            this IJob<ImageDomainModel, ImageDomainModel> imgToImgJob, RgbRegulator regulator)
        {
            return imgToImgJob.MapParameter((ImageDomainModel image) => regulator.Regulate(image));
        }

        public static IJob<ImageDomainModel, ImageDomainModel> Persist(
            this IJob<ImageDomainModel, ImageDomainModel> job, ImagePersister persistor)
            => job.MapResult(async (img) =>
            {
                await persistor.Persist(img);
                return img;
            });
    }
}