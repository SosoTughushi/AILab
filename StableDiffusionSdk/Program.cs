using Microsoft.Extensions.Configuration;
using StableDiffusionSdk.Prompts;
using StableDiffusionSdk.Workflows;
using StableDiffusionTools.ImageUtilities;
using StableDiffusionTools.Integrations.EbSynth;
using StableDiffusionTools.Integrations.OpenAi;
using StableDiffusionTools.Integrations.StableDiffusionWebUi;

// Read configuration from appsettings.json, appsettings.local.json, and environment variables


var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables() // Add this line to read environment variables
    .Build();

var gptApiKey = configuration["GptApiKey"]!;
var gptApi = new GptApi(gptApiKey);

var stableDiffusionUrl = configuration["StableDiffusionUrl"]!;

// Create a new instance of StableDiffusionApi using the retrieved URL
var stableDiffusionApi = new StableDiffusionApi(stableDiffusionUrl);
var ebSynth = new EbSynth(configuration["EbSynthLocation"]!);

var eldenRingPrompter = new ComicDiffusionPrompter(gptApi, stableDiffusionApi,"skulls, scary, human anatomy").Cached(10);

//var videoToVideoWorkflow = new VideoToVideoWorkflow(stableDiffusionApi, comicDiffusionPrompter);
//await videoToVideoWorkflow.Run(
//    inputVideoLocation: @"D:\Stable Diffusion\Workspace\Tsnisi\Tunnel\Sauce.mp4",
//    outputFolder: @"D:\Stable Diffusion\Workspace\Tsnisi\Tunnel\Comic",
//    4);


var smoothZoomInWorkflow = new StretchWorkflow(stableDiffusionApi, eldenRingPrompter);
foreach (var file in Directory.EnumerateFiles(@"D:\Stable Diffusion\Recursive\Bright"))
{
    await smoothZoomInWorkflow.Run(
        file: file
        
        
        );

    return;
}