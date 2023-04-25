using System.Drawing;
using Newtonsoft.Json;

namespace StableDiffusionSdk.Modules.Images;

public record ImageDomainModel([property: JsonIgnore] string ContentAsBase64String, int Width, int Height)
{
    public static ImageDomainModel FromSystemDrawingImage(Bitmap rotatedImage, ImageDomainModel input)
    {
        using var ms = new MemoryStream();
        rotatedImage.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
        return new ImageDomainModel(Convert.ToBase64String(ms.ToArray()), input.Width, input.Height);
    }
};