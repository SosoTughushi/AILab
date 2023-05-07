using StableDiffusionSdk.Infrastructure;
using StableDiffusionSdk.Integrations.OpenAi;
using StableDiffusionSdk.Utilities.Prompts;
using StableDiffusionTools.ImageUtilities;
using StableDiffusionTools.Integrations.StableDiffusionWebUi;

namespace StableDiffusionSdk.Workflows;

public class LyricsToImagesWorkflow
{
    private readonly GptApi _gptApi;
    private readonly ITextToImage _stableDiffusionApi;

    public LyricsToImagesWorkflow(GptApi gptApi, ITextToImage stableDiffusionApi)
    {
        _gptApi = gptApi;
        _stableDiffusionApi = stableDiffusionApi;
    }

    public async Task Run(string name, string lyrics)
    {
        var prompts = await _gptApi.SongLyricsToPrompts(lyrics);
        var imagePersistor = new ImagePersister(name);
        var jsonWriter = new JsonWriter(imagePersistor.OutputFolder);

        foreach (var prompt in prompts)
        {
            var request = new Text2ImgRequest(
                Prompt: $"{prompt}",
                Seed: Seed.Random(),
                Width: ImageResolution._1024,
                Height: ImageResolution._1024,
                CfgScale: 7
            );
            await jsonWriter.Write(request);
            var image = await _stableDiffusionApi.TextToImage(request);
            await imagePersistor.Persist(image);
        }
    }
}