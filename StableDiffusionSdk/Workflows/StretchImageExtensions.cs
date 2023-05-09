using StableDiffusionTools.Domain;

public static class StretchImageExtensions
{
    public static async Task<ImageDomainModel> LogarithmicWarpLeft(this ImageDomainModel source, double warpStrength)
    {
        // Convert base64 to Image
        byte[] imageBytes = Convert.FromBase64String(source.ContentAsBase64String);
        using var image = Image.Load<Rgba32>(imageBytes);

        // Apply the logarithmic transformation and get the maximum x-coordinate
        double maxX = InverseLogarithmicTransform(source.Width - 1, source.Width, warpStrength);

        var resultImage = new Image<Rgba32>((int)Math.Ceiling(maxX) + 1, source.Height);

        for (int y = 0; y < source.Height; y++)
        {
            for (int x = 0; x < resultImage.Width; x++)
            {
                double originalX = InverseLogarithmicTransform(x, source.Width, warpStrength);
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

        return new ImageDomainModel(base64, source.Width, source.Height);
    }

    private static double InverseLogarithmicTransform(int x, int width, double warpStrength)
    {
        double a = 1;
        double b = Math.Pow(Math.E, warpStrength);
        double scaledX = a + (b - a) * ((double)x / (width - 1)); // Scale x to [a, b]
        double y = Math.Log(scaledX); // Apply logarithmic transformation

        // Normalize the range of the transformation to [0, width - 1]
        double min = Math.Log(a);
        double max = Math.Log(b);
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