using System.Drawing;
using Color = System.Drawing.Color;

namespace StableDiffusionSdk.Modules.Images;

public static class ZoomDirectionBuilder
{
    public static ZoomInDirection Top(double deltaY)
    {
        return new ZoomInDirection(0, -deltaY);
    }

    public static ZoomInDirection Bottom(double deltaY)
    {
        return new ZoomInDirection(0, deltaY);
    }

    public static ZoomInDirection Left(double deltaX)
    {
        return new ZoomInDirection(-deltaX, 0);
    }

    public static ZoomInDirection Right(double deltaX)
    {
        return new ZoomInDirection(deltaX, 0);
    }

    public static ZoomInDirection Center() => new ZoomInDirection(0, 0);

    public static ZoomInDirection Top(this ZoomInDirection direction, double deltaY)
    {
        return direction with { DeltaY = direction.DeltaY + deltaY };
    }

    public static ZoomInDirection Bottom(this ZoomInDirection direction, double deltaY)
    {
        return direction with { DeltaY = direction.DeltaY - deltaY };
    }

    public static ZoomInDirection Left(this ZoomInDirection direction, double deltaX)
    {
        return direction with { DeltaX = direction.DeltaX - deltaX };
    }

    public static ZoomInDirection Right(this ZoomInDirection direction, double deltaX)
    {
        return direction with { DeltaX = direction.DeltaX + deltaX };
    }
}

public record ZoomInDirection(double DeltaX, double DeltaY);



public static class ZoomImageExtensions
{
    public static async Task<ImageDomainModel> Zoom(this ImageDomainModel image, double zoomPercent,
        ZoomInDirection direction)
    {
        using var originalImage = image.ToSystemDrawingImage();
        var zoomedImage = GetZoomedImage(originalImage, zoomPercent);
        var sourceRect = GetSourceRectangle(direction, zoomedImage, originalImage, zoomPercent);
        var destRect = new System.Drawing.Rectangle(0, 0, image.Width, image.Height);

        using var resultImage = new Bitmap(image.Width, image.Height, originalImage.PixelFormat);
        using (var g = Graphics.FromImage(resultImage))
        {
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            g.DrawImage(zoomedImage, destRect, sourceRect, GraphicsUnit.Pixel);
        }

        return ImageDomainModel.FromSystemDrawingImage(resultImage, image);
    }

    public static async Task<ImageDomainModel> PreviewZoom(this ImageDomainModel image, double zoomPercent,
        ZoomInDirection direction, int iterationCount)
    {
        using var originalImage = image.ToSystemDrawingImage();
        using var resultImage = new Bitmap(originalImage.Width, originalImage.Height, originalImage.PixelFormat);

        using (var g = Graphics.FromImage(resultImage))
        {
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            g.DrawImage(originalImage, 0, 0, image.Width, image.Height);

            var pen = new Pen(Color.Black, 2);
            var previousRectangle = new System.Drawing.Rectangle(0, 0, originalImage.Width, originalImage.Height);

            for (int i = 0; i < iterationCount; i++)
            {
                var newWidth = (int)(previousRectangle.Width / (zoomPercent / 100));
                var newHeight = (int)(previousRectangle.Height / (zoomPercent / 100));

                var offsetX = (previousRectangle.Width - newWidth) * 0.5 * (1 + (double)direction.DeltaX / 100);
                var offsetY = (previousRectangle.Height - newHeight) * 0.5 * (1 + (double)direction.DeltaY / 100);

                var x = previousRectangle.Left + offsetX;
                var y = previousRectangle.Top + offsetY;

                var newRect = new System.Drawing.Rectangle((int)x, (int)y, newWidth, newHeight);
                g.DrawRectangle(pen, newRect);

                previousRectangle = newRect;
            }
        }

        return ImageDomainModel.FromSystemDrawingImage(resultImage, image);
    }

    private static Bitmap GetZoomedImage(System.Drawing.Image originalImage, double zoomPercent)
    {
        var newWidth = (int)(originalImage.Width * zoomPercent / 100);
        var newHeight = (int)(originalImage.Height * zoomPercent / 100);

        var zoomedImage = new Bitmap(newWidth, newHeight, originalImage.PixelFormat);
        using (var g = Graphics.FromImage(zoomedImage))
        {
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            g.DrawImage(originalImage, 0, 0, newWidth, newHeight);
        }

        return zoomedImage;
    }

    private static System.Drawing.Rectangle GetSourceRectangle(ZoomInDirection direction, Bitmap zoomedImage,
        System.Drawing.Image originalImage, double zoomPercent)
    {
        double scaleFactor = 1 / (zoomPercent / 100);
        double newWidth = originalImage.Width * scaleFactor;
        double newHeight = originalImage.Height * scaleFactor;

        double offsetX = (zoomedImage.Width - newWidth) * 0.5 * (1 + (double)direction.DeltaX / 100);
        double offsetY = (zoomedImage.Height - newHeight) * 0.5 * (1 + (double)direction.DeltaY / 100);

        return new System.Drawing.Rectangle((int)offsetX, (int)offsetY, (int)newWidth, (int)newHeight);
    }
}