using Accord.Imaging;
using Microsoft.Extensions.Configuration;
using StableDiffusionSdk.Prompts;
using StableDiffusionSdk.Workflows;
using StableDiffusionTools.Domain;
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


var comicDiffusionPrompter = new ComicDiffusionPrompter(gptApi, stableDiffusionApi, "").Cached(50);

var eldenRingPrompter = new EldenRingPrompter(gptApi, stableDiffusionApi,
        "light at the end of the tunnel, near death experience, positive, blurry and cloudy, dream alike, oil painting")
    .Cached(50);

async Task<Img2ImgRequest> CreateImg2ImgRequest(ImageDomainModel img) => new (
        InputImage: img,
        Prompt: await eldenRingPrompter!.GetPrompt(img),
        DenoisingStrength: 0.3,
        Seed.Random()
    );

var smoothZoomInWorkflow = new SmoothZoomInWorkflow(stableDiffusionApi, CreateImg2ImgRequest);

//await smoothZoomInWorkflow.Run(@"C:\Users\TomTo\Downloads\drone_crash\6\00014.jpg",
//    ImageResolution._1216,
//    105,
//    50,
//    0.1,
//    2);

var videoToVideoWorkflow = new VideoToVideoWorkflow(stableDiffusionApi, CreateImg2ImgRequest);

//await videoToVideoWorkflow.Run(@"C:\Users\TomTo\Downloads\drone_crash.mp4", 
//    30, 
//    ImageResolution._1408);


var warpWorkflow = new WarpInWorkflow(stableDiffusionApi, CreateImg2ImgRequest, resolution:ImageResolution._1024);

//await warpWorkflow.Run(@"C:\Users\TomTo\Downloads\drone_crash\6\00014.jpg",
//    new WarpDirection(-0.1, -0.3),
//    120
//    );

var pdfToVideoWorkflow = new PdfToVideoWorkflow(stableDiffusionApi, gptApi, ImageResolution._1024);

await foreach (var prompt in pdfToVideoWorkflow.GetPrompts(@"D:\Downloads\alice in the wonderland.pdf", 2))
{
    Console.WriteLine(prompt);
    Console.WriteLine();
}


