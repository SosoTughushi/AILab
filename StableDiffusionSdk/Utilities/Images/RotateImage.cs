using System.Drawing;
using StableDiffusionSdk.Modules.Images;
using SD = System.Drawing;
public static class RotateImageExtensions
{
    public static SD.Image ToSystemDrawingImage(this ImageDomainModel imageDomainModel)
    {
        using var ms = new MemoryStream(Convert.FromBase64String(imageDomainModel.ContentAsBase64String));
        return SD.Image.FromStream(ms);
    }
    public static async Task<ImageDomainModel> Rotate(this ImageDomainModel image, double angle)
    {
        using var originalImage = image.ToSystemDrawingImage();

        using var rotatedImage = new Bitmap(originalImage.Width, originalImage.Height);
        using (var graphics = Graphics.FromImage(rotatedImage))
        {
            graphics.TranslateTransform((float)originalImage.Width / 2, (float)originalImage.Height / 2);
            graphics.RotateTransform((float)angle);
            graphics.TranslateTransform(-(float)originalImage.Width / 2, -(float)originalImage.Height / 2);
            graphics.DrawImage(originalImage, new SD.Point(0, 0));
        }

        return ImageDomainModel.FromSystemDrawingImage(rotatedImage, image);
    }
}