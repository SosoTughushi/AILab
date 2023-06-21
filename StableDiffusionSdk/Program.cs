using Microsoft.Extensions.DependencyInjection;
using StableDiffusionSdk;
using StableDiffusionSdk.Prompts;
using StableDiffusionSdk.Workflows;
using StableDiffusionSdk.Workflows.VideoToVideo;
using StableDiffusionTools.Domain;
using StableDiffusionTools.ImageUtilities;
using StableDiffusionTools.Integrations.OpenAi;
using StableDiffusionTools.Integrations.StableDiffusionWebUi;


var provider = SerficeConfigurator.Create();


var workflow = provider.GetRequiredService<ConsistentStyleTransfgerWorkflow>();


var file = @"D:\Drone Footage\cyprus\DJI_0074.MP4";
var prompter = new ComicDiffusionPrompter(
        provider.GetRequiredService<GptApi>(), 
        provider.GetRequiredService<StableDiffusionApi>(),
        "friends in the car looking at camera")
    .Cached(5);

await workflow.Run(file, 
    ImageResolution._1024,
    5, 
    2, 
    async img => new Img2ImgRequest(
    img,
    await prompter.GetPrompt(img),
    0.25,
    Seed.Random(),
    NegativePrompt:""
));