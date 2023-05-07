using Microsoft.Extensions.Configuration;
using StableDiffusionSdk.Integrations.OpenAi;
using StableDiffusionSdk.Prompts;
using StableDiffusionSdk.Workflows;
using StableDiffusionTools.ImageUtilities;
using StableDiffusionTools.Integrations.EbSynth;
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

var comicDiffusionPrompter = new EldenRingPrompter(gptApi, stableDiffusionApi, "Outer space").Cached(2);

//var videoToVideoWorkflow = new VideoToVideoWorkflow(stableDiffusionApi, comicDiffusionPrompter);
//await videoToVideoWorkflow.Run(
//    inputVideoLocation: @"D:\Stable Diffusion\Workspace\Tsnisi\Tunnel\Sauce.mp4",
//    outputFolder: @"D:\Stable Diffusion\Workspace\Tsnisi\Tunnel\Comic",
//    4);


var smoothZoomInWorkflow = new VideoToVideoWorkflow(stableDiffusionApi, comicDiffusionPrompter);
await smoothZoomInWorkflow.Run(
    inputVideoLocation: @"C:\Users\TomTo\Videos\Captures\Outer Wilds 2023-05-07 01-38-38.mp4",
    outputFolder:@"C:\Users\TomTo\Videos\Captures\Outer Wilds 2023-05-07 01-38-38",
    takeEveryXthFrame:30*10,
    ImageResolution._1408);