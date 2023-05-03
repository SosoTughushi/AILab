using Microsoft.Extensions.Configuration;
using StableDiffusionSdk.Integrations.EbSynth;
using StableDiffusionSdk.Integrations.OpenAiGptApi;
using StableDiffusionSdk.Integrations.StableDiffusionWebUiApi;
using StableDiffusionSdk.Modules.Images;
using StableDiffusionSdk.Prompts;
using StableDiffusionSdk.Workflows;

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

var comicDiffusionPrompter = new ComicDiffusionPrompter(gptApi, stableDiffusionApi).Cached(10);

var ebSynthVideoWorkflow =
    new VideoToVideoViaEbSynth(stableDiffusionApi, ebSynth, comicDiffusionPrompter, ImageResolution._1024);


