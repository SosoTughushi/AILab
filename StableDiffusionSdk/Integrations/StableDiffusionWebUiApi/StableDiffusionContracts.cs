using StableDiffusionSdk.Modules.Images;

namespace StableDiffusionSdk.Integrations.StableDiffusionWebUiApi
{
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

    public enum InterrogationModel
    {
        Clip,
        DeepBoory
    }

    public record InterrogateRequest(ImageDomainModel InputImage, InterrogationModel Model = InterrogationModel.Clip);


}
