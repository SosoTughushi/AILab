using StableDiffusionTools.Domain;

namespace StableDiffusionSdk.Workflows
{
    public static class StretchImageExtensions
    {
        public static async Task<ImageDomainModel> LogarithmicWarp(this ImageDomainModel source,
            WarpDirection direction)
        {
            var result = source;

            if (direction.DeltaX > 0)
            {
                result = await result.LogarithmicWarpLeft(direction.DeltaX);
            }

            if (direction.DeltaX < 0)
            {
                result = await result.LogarithmicWarpRight(Math.Abs(direction.DeltaX));
            }

            if (direction.DeltaY > 0)
            {
                result = await result.LogarithmicWarpUp(direction.DeltaY);
            }

            if (direction.DeltaY < 0)
            {
                result = await result.LogarithmicWarpDown(Math.Abs(direction.DeltaY));
            }

            return result;
        }

        public static async Task<ImageDomainModel> LogarithmicWarpLeft(this ImageDomainModel source,
            double warpStrength)
        {
            return await LogarithmicWarp(source, warpStrength, false);
        }

        public static async Task<ImageDomainModel> LogarithmicWarpRight(this ImageDomainModel source,
            double warpStrength)
        {
            return await LogarithmicWarp(source, warpStrength, true);
        }

        public static async Task<ImageDomainModel> LogarithmicWarpUp(this ImageDomainModel source, double warpStrength)
        {
            return await LogarithmicWarpVertical(source, warpStrength, false);
        }

        public static async Task<ImageDomainModel> LogarithmicWarpDown(this ImageDomainModel source,
            double warpStrength)
        {
            return await LogarithmicWarpVertical(source, warpStrength, true);
        }

        private static async Task<ImageDomainModel> LogarithmicWarpVertical(ImageDomainModel source,
            double warpStrength, bool reverse)
        {
            // Convert base64 to Image
            byte[] imageBytes = Convert.FromBase64String(source.ContentAsBase64String);
            using var image = Image.Load<Rgba32>(imageBytes);

            // Apply the logarithmic transformation and get the maximum y-coordinate
            double maxY = InverseLogarithmicTransform(source.Height - 1, source.Height, warpStrength, reverse);

            var resultImage = new Image<Rgba32>(source.Width, (int)Math.Ceiling(maxY) + 1);

            for (int x = 0; x < source.Width; x++)
            {
                for (int y = 0; y < resultImage.Height; y++)
                {
                    double originalY = InverseLogarithmicTransform(y, source.Height, warpStrength, reverse);
                    int originalYFloor = (int)Math.Floor(originalY);
                    int originalYCeil = (int)Math.Ceiling(originalY);
                    if (originalYFloor >= 0 && originalYCeil < source.Height)
                    {
                        Rgba32 pixelFloor = image[x, originalYFloor];
                        Rgba32 pixelCeil = image[x, originalYCeil];
                        float t = (float)(originalY - originalYFloor);
                        resultImage[x, y] = Interpolate(pixelFloor, pixelCeil, t);
                    }
                }
            }

            // Stretch the image back to its original height
            resultImage.Mutate(ctx => ctx.Resize(source.Width, source.Height));

            // Convert Image to base64
            var outputStream = new MemoryStream();
            await resultImage.SaveAsPngAsync(outputStream);

            var base64 = Convert.ToBase64String(outputStream.ToArray());

            return new ImageDomainModel(base64, source.Width, source.Height);
        }

        private static async Task<ImageDomainModel> LogarithmicWarp(ImageDomainModel source, double warpStrength,
            bool reverse)
        {
            // Convert base64 to Image
            byte[] imageBytes = Convert.FromBase64String(source.ContentAsBase64String);
            using var image = Image.Load<Rgba32>(imageBytes);

            // Apply the logarithmic transformation and get the maximum x-coordinate
            double maxX = InverseLogarithmicTransform(source.Width - 1, source.Width, warpStrength, reverse);

            var resultImage = new Image<Rgba32>((int)Math.Ceiling(maxX) + 1, source.Height);

            for (int y = 0; y < source.Height; y++)
            {
                for (int x = 0; x < resultImage.Width; x++)
                {
                    double originalX = InverseLogarithmicTransform(x, source.Width, warpStrength, reverse);
                    int originalXFloor = (int)Math.Floor(originalX);
                    int originalXCeil = (int)Math.Ceiling(originalX);
                    if (originalXFloor >= 0 && originalXCeil < source.Width)
                    {
                        Rgba32 pixelFloor = image[originalXFloor, y];
                        Rgba32 pixelCeil = image[originalXCeil, y];
                        float t = (float)(originalX - originalXFloor);
                        resultImage[x, y] = Interpolate(pixelFloor, pixelCeil, t);
                    }
                }
            }

            // Stretch the image back to its original width
            resultImage.Mutate(ctx => ctx.Resize(source.Width, source.Height));

            // Convert Image to base64
            var outputStream = new MemoryStream();
            await resultImage.SaveAsPngAsync(outputStream);

            var base64 = Convert.ToBase64String(outputStream.ToArray());

            return source with { ContentAsBase64String = base64 };
        }

        private static double InverseLogarithmicTransform(int x, int width, double warpStrength, bool reverse = false)
        {
            double a = 1;
            double b = Math.Pow(Math.E, warpStrength);
            double scaledX = a + (b - a) * ((double)x / (width - 1)); // Scale x to [a, b]

            Func<double, double> distributer = Math.Log;

            if (reverse) distributer = Math.Exp;
            double y = distributer(scaledX);

            // Normalize the range of the transformation to [0, width - 1]
            double min = distributer(a);
            double max = distributer(b);
            y = (y - min) / (max - min) * (width - 1);

            return y;
        }

        private static Rgba32 Interpolate(Rgba32 color1, Rgba32 color2, float t)
        {
            float r = color1.R * (1 - t) + color2.R * t;
            float g = color1.G * (1 - t) + color2.G * t;
            float b = color1.B * (1 - t) + color2.B * t;
            float a = color1.A * (1 - t) + color2.A * t;
            return new Rgba32(
                (byte)Math.Clamp(r, 0, 255),
                (byte)Math.Clamp(g, 0, 255),
                (byte)Math.Clamp(b, 0, 255),
                (byte)Math.Clamp(a, 0, 255)
            );
        }
    }

    public record WarpDirection(double DeltaX, double DeltaY);
}