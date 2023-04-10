using Microsoft.Extensions.Configuration;
using StableDiffusionSdk.DomainModels;
using StableDiffusionSdk.Infrastructure;
using StableDiffusionSdk.Integrations.OpenAiGptApi;
using StableDiffusionSdk.Jobs;
using StableDiffusionSdk.Jobs.Image;
using StableDiffusionSdk.Jobs.Interrogators;
using StableDiffusionSdk.Jobs.Prompters;
using StableDiffusionSdk.StableDiffusionApi;
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

var globalPersister = new ImagePersister("GeneratedImages");


//foreach (var file in Directory.EnumerateFiles(@"D:\Stable Diffusion\Recursive\NIghtJob"))
    for (var i = 0; i < 5; i++)
    {
        var path = @"D:\Stable Diffusion\Recursive\NIghtJob\bored.jpg";
        var input = await path.ReadImage();
        var regulator = new RgbRegulator();
        var baseOutputFolder = Path.Combine(Path.GetDirectoryName(path)!,
            Path.GetFileNameWithoutExtension(path));
        var persister = new ImagePersister(baseOutputFolder);
        var jsonWriter = new JsonWriter(persister.OutputFolder);

        string? prompt = null!;
        var promptCount = 0;
        var imageCounter = 0;

        async Task<string> CreatePrompt(
            ImageDomainModel image)
        {
            promptCount++;
            if (promptCount % 15 == 0 || prompt is null)
            {
                var clip = await stableDiffusionApi.InterrogatePlease(image, InterrogationModel.Clip);

                var randomPrompt = new[]
                {
                    "van gogh",
                    "steampunk",
                    "blade runner thematics",
                    "animatrix",
                    "replace everything with cyberpunk",
                    "cosmic entity in galaxy",
                };

                var styles = new[]
                {
                    "charliebo artstyle",
                    "holliemengert artstyle",
                    "marioalberti artstyle",
                    "pepelarraz artstyle",
                    "andreasrocha artstyle",
                    "jamesdaly artstyle",
                    "comicmay artsyle"
                };

                prompt = await gptApi.Consolidate(
                    @$"{randomPrompt[new Random().Next(randomPrompt.Length)]}, trigger word: [{styles[new Random().Next(styles.Length)]}]",
                    clip);
            }

            return prompt;
        }

        var zoomDirection = ZoomDirectionBuilder.Right(21.3).Bottom(12.5);
        var zoomPercent = 120;

        await ImageToImageJobExtensions.Create()
            .MapResult(image => image.PreviewZoom(zoomPercent, zoomDirection, 40))
            .Persist(persister)
            .Run(input);

        //return;
        var seed = Seed.Random();
        await ImageToImageJobExtensions.Create()
                .MapParameter(async (ImageDomainModel image) =>
                {
                    if (imageCounter < 15)
                    {
                        return image;
                    }

                    return await regulator.Regulate(image);
                })
                .MapResult((image) => image.Resize(ImageResolution._1408))
                .MapResult(async image =>
                {
                    imageCounter++;
                    var gptPrompt = await CreatePrompt(image);

                    var request = new Img2ImgRequest(
                        InputImage: image,
                        Prompt: gptPrompt,
                        DenoisingStrength: 0.2,
                        NegativePrompt: "",
                        Seed: seed
                    );

                    await jsonWriter.Write(request);
                    return await stableDiffusionApi.ImageToImage(request);
                })
                .Persist(persister)
                .ZoomInSmoothly(zoomPercent, zoomDirection, 5)
                // .Persist(globalPersister)
                .LoopRecursively(50)

                //.ZoomInSmoothly(101, new ZoomInDirection(0, 0), 10)
                //.ZoomInSmoothly(95, new ZoomInDirection(0, 0), 10)
                .RunSafe(input)
            ;
    }