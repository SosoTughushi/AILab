using System.Drawing;
using System.Drawing.Drawing2D;
using StableDiffusionTools.Domain;

namespace StableDiffusionTools.ImageUtilities;

public static class StretchImageExtensions
{
    public static async Task<ImageDomainModel> Stretch(this ImageDomainModel image, StretchDirection direction, CropPosition position)
    {
        using var originalImage = image.ToSystemDrawingImage();

        var newWidth = originalImage.Width + originalImage.Width * direction.DeltaX / 100;
        var newHeight = originalImage.Height + originalImage.Height * direction.DeltaY / 100;

        using var stretchedImage = new Bitmap(newWidth, newHeight, originalImage.PixelFormat);
        using (var g = Graphics.FromImage(stretchedImage))
        {
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.DrawImage(originalImage, 0, 0, newWidth, newHeight);
        }

        var cropRect = new System.Drawing.Rectangle(
            (int)(position.X * (newWidth - originalImage.Width)),
            (int)(position.Y * (newHeight - originalImage.Height)),
            originalImage.Width,
            originalImage.Height
        );

        using var croppedImage = new Bitmap(originalImage.Width, originalImage.Height, originalImage.PixelFormat);
        using (var g = Graphics.FromImage(croppedImage))
        {
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.DrawImage(stretchedImage, new System.Drawing.Rectangle(0, 0, originalImage.Width, originalImage.Height), cropRect, GraphicsUnit.Pixel);
        }

        return ImageDomainModel.FromSystemDrawingImage(croppedImage, image);
    }
}

public record StretchDirection(int DeltaX, int DeltaY);
public record CropPosition(double X, double Y);