

using System.Drawing;
using Newtonsoft.Json;

namespace StableDiffusionSdk.DomainModels
{
    public record ImageDomainModel([property: JsonIgnore] string ContentAsBase64String, int Width, int Height)
    {
        public static ImageDomainModel FromSystemDrawingImage(Bitmap rotatedImage, ImageDomainModel input)
        {
            using var ms = new MemoryStream();
            rotatedImage.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            return new ImageDomainModel(Convert.ToBase64String(ms.ToArray()), input.Width, input.Height);
        }
    };

    public class RandomSeed
    {
        private RandomSeed()
        {
        }

        public static RandomSeed Value { get; } = new RandomSeed();
    }

    public record Img2ImgRequest
    (
        ImageDomainModel InputImage,
        string Prompt,
        double DenoisingStrength,
        Seed Seed,
        int CfgScale = 20,
        int Steps = 20,
        string? NegativePrompt = null,
        bool RestoreFaces = false
    );

    public record Seed(int Value)
    {
        public static Seed Random() => new Seed(new Random().Next());
    }

    public enum InterrogationModel
    {
        Clip,
        DeepBoory
    }

    public record InterrogateRequest(ImageDomainModel InputImage, InterrogationModel Model = InterrogationModel.Clip);


}
