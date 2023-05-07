using Newtonsoft.Json;
using System.Drawing;

namespace StableDiffusionTools.Domain;

public record ImageDomainModel(
    [property: JsonIgnore] string ContentAsBase64String,
    int Width,
    int Height)
{
    public static ImageDomainModel FromSystemDrawingImage(Bitmap rotatedImage, ImageDomainModel input)
    {
        using var ms = new MemoryStream();
        rotatedImage.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
        return new ImageDomainModel(Convert.ToBase64String(ms.ToArray()), input.Width, input.Height);
    }
};


public interface ITextToImage
{
    Task<ImageDomainModel> TextToImage(Text2ImgRequest request);
}

public record Text2ImgRequest
(
    string Prompt,
    Seed Seed,
    int Width,
    int Height,
    double CfgScale = 7,
    int Steps = 20,
    string? NegativePrompt = null,
    bool RestoreFaces = false
);


public record Seed(int Value)
{
    public static Seed Random() => new(new Random().Next());
}

