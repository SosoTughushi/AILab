using Microsoft.Extensions.Configuration;
using StableDiffusionSdk.Integrations.OpenAiGptApi;
using StableDiffusionSdk.Integrations.StableDiffusionWebUiApi;
using StableDiffusionSdk.Modules.Images;
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


//foreach (var file in Directory.EnumerateFiles(@"D:\Stable Diffusion\Recursive\NIghtJob"))
    for (var i = 0; i < 5; i++)
    {
        var path = @"D:\Stable Diffusion\Recursive\NIghtJob\bored.jpg";
        var smoothZoomInWorkflow = new SmoothZoomInWorkflow(stableDiffusionApi, gptApi);

        await smoothZoomInWorkflow.Run(path, ImageResolution._1408, 120);
    }