using System.Drawing;
using StableDiffusionSdk.Modules.Images;
using Color = System.Drawing.Color;
using Rectangle = System.Drawing.Rectangle;

public static class ChangeRgb
{

    public static async Task<ImageDomainModel> AdjustRgb(this ImageDomainModel image, double targetRed, double targetGreen, double targetBlue)
    {
        using var originalImage = image.ToSystemDrawingImage();
        using var bitmapImage = new Bitmap(originalImage);
        using var outputImage = new Bitmap(bitmapImage.Width, bitmapImage.Height);

        // Calculate the current average RGB values
        var (averageRed, averageGreen, averageBlue) = await image.CalculateAverageRgb();

        // Calculate the adjustment factors
        double redFactor = targetRed / averageRed;
        double greenFactor = targetGreen / averageGreen;
        double blueFactor = targetBlue / averageBlue;

        for (int x = 0; x < bitmapImage.Width; x++)
        {
            for (int y = 0; y < bitmapImage.Height; y++)
            {
                Color pixel = bitmapImage.GetPixel(x, y);

                int newRed = Truncate(pixel.R * redFactor);
                int newGreen = Truncate(pixel.G * greenFactor);
                int newBlue = Truncate(pixel.B * blueFactor);

                Color newPixel = Color.FromArgb(pixel.A, newRed, newGreen, newBlue);
                outputImage.SetPixel(x, y, newPixel);
            }
        }

        return ImageDomainModel.FromSystemDrawingImage(outputImage, image);
    }

    public static Task<(double red, double green, double blue)> CalculateAverageRgb(this ImageDomainModel image)
    {
        using var originalImage = image.ToSystemDrawingImage();
        using var bitmapImage = new Bitmap(originalImage);

        long totalRed = 0;
        long totalGreen = 0;
        long totalBlue = 0;
        int totalPixels = bitmapImage.Width * bitmapImage.Height;

        for (int x = 0; x < bitmapImage.Width; x++)
        {
            for (int y = 0; y < bitmapImage.Height; y++)
            {
                Color pixel = bitmapImage.GetPixel(x, y);
                totalRed += pixel.R;
                totalGreen += pixel.G;
                totalBlue += pixel.B;
            }
        }

        double averageRed = (double)totalRed / totalPixels;
        double averageGreen = (double)totalGreen / totalPixels;
        double averageBlue = (double)totalBlue / totalPixels;

        return Task.FromResult((averageRed, averageGreen, averageBlue));
    }
    
    private static int Truncate(double value)
    {
        return (int)Math.Min(Math.Max(value, 0), 255);
    }
}

public class RgbRegulator
{
    private bool _initialized;
    private double _red;
    private double _green;
    private double _blue;

    public async Task<ImageDomainModel> Regulate(ImageDomainModel image)
    {
        if (!_initialized)
        {
            _initialized = true;
            (_red, _green, _blue) = await image.CalculateAverageRgb();
            return image;
        }

        return await image.AdjustRgb(_red, _green, _blue);
    }
}