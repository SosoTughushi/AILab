using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using StableDiffusionTools.Domain;
using Image = System.Drawing.Image;
using Rectangle = System.Drawing.Rectangle;

namespace StableDiffusionTools.ImageUtilities
{
    public static class ImageExtensions
    {
        public static async Task<ImageDomainModel> Resize(this ImageDomainModel image, int width, int height)
        {
            using var originalImage = image.ToSystemDrawingImage();
            using var resizedImage = new Bitmap(width, height);
            using (var graphics = Graphics.FromImage(resizedImage))
            {
                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                double originalAspectRatio = (double)originalImage.Width / originalImage.Height;
                double targetAspectRatio = (double)width / height;
                int cropWidth, cropHeight;

                if (originalAspectRatio > targetAspectRatio)
                {
                    cropWidth = (int)(originalImage.Height * targetAspectRatio);
                    cropHeight = originalImage.Height;
                }
                else
                {
                    cropWidth = originalImage.Width;
                    cropHeight = (int)(originalImage.Width / targetAspectRatio);
                }

                int cropX = (originalImage.Width - cropWidth) / 2;
                int cropY = (originalImage.Height - cropHeight) / 2;

                var cropRect = new Rectangle(cropX, cropY, cropWidth, cropHeight);
                graphics.DrawImage(originalImage, new Rectangle(0, 0, width, height), cropRect, GraphicsUnit.Pixel);
            }

            return await resizedImage.ToImageDomainModel(image);
        }



        public static async Task<ImageDomainModel> Resize(this ImageDomainModel inputImage, int maxDimension)
        {
            using var originalImage = inputImage.ToSystemDrawingImage();
            var width = inputImage.Width;
            var height = inputImage.Height;
            var aspectRatio = (double)inputImage.Width / inputImage.Height;

            int newWidth, newHeight;

            if (width > height)
            {
                newWidth = maxDimension;
                newHeight = (int)(newWidth / aspectRatio);
            }
            else
            {
                newHeight = maxDimension;
                newWidth = (int)(newHeight * aspectRatio);
            }

            int x = (originalImage.Width - newWidth) / 2;
            int y = (originalImage.Height - newHeight) / 2;

            using var resizedImage = new Bitmap(newWidth, newHeight);
            using (var graphics = Graphics.FromImage(resizedImage))
            {
                graphics.DrawImage(originalImage, new Rectangle(0, 0, newWidth, newHeight), new Rectangle(x, y, newWidth, newHeight), GraphicsUnit.Pixel);
            }

            return await resizedImage.ToImageDomainModel(inputImage);
        }

        private static async Task<ImageDomainModel> ToImageDomainModel(this Image image, ImageDomainModel originalImage)
        {
            using var ms = new MemoryStream();
            image.Save(ms, ImageFormat.Png);
            return new ImageDomainModel(Convert.ToBase64String(ms.ToArray()), image.Width, image.Height);
        }
    }

    public static class ImageResolution
    {
        public const int _512 = 512;
        public const int _576 = 576;
        public const int _640 = 640;
        public const int _704 = 704;
        public const int _768 = 768;
        public const int _832 = 832;
        public const int _896 = 896;
        public const int _960 = 960;
        public const int _1024 = 1024;
        public const int _1088 = 1088;
        public const int _1152 = 1152;
        public const int _1216 = 1216;
        public const int _1280 = 1280;
        public const int _1344 = 1344;
        public const int _1408 = 1408;
        public const int _1472 = 1472;
        public const int _1536 = 1536;
        public const int _1600 = 1600;
        public const int _1664 = 1664;
        public const int _1728 = 1728;
        public const int _1792 = 1792;
        public const int _1856 = 1856;
        public const int _1920 = 1920;
        public const int _1984 = 1984;
        public const int _2048 = 2048;
    }
}