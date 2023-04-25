using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StableDiffusionSdk.Modules.Images
{
    public static class ImageExtensions
    {
        public static ImageDomainModel Resize(this ImageDomainModel image, int width, int height)
        {
            return image with { Width = width, Height = height };
        }

        public static Task<ImageDomainModel> Resize(this ImageDomainModel inputImage, int maxDimension)
        {
            var width = inputImage.Width;
            var height = inputImage.Height;
            var aspectRatio = (double)inputImage.Width / inputImage.Height;

            if (width > height)
            {
                width = maxDimension;
                height = (int)(width / aspectRatio);
            }
            else
            {
                height = maxDimension;
                width = (int)(height * aspectRatio);
            }

            width = width / 64 * 64;
            height = height / 64 * 64;

            return Task.FromResult(inputImage with { Height = height, Width = width });
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