using StableDiffusionSdk.Infrastructure;
using StableDiffusionSdk.Integrations.OpenAi;
using StableDiffusionTools.Domain;
using StableDiffusionTools.Integrations.StableDiffusionWebUi;

namespace StableDiffusionSdk.Prompts;

public class ComicDiffusionPrompter : IPrompter
{
    private readonly GptApi _gptApi;
    private readonly StableDiffusionApi _stableDiffusionApi;
    private readonly string _prompt;

    public ComicDiffusionPrompter(GptApi gptApi, StableDiffusionApi stableDiffusionApi, string prompt)
    {
        _gptApi = gptApi;
        _stableDiffusionApi = stableDiffusionApi;
        _prompt = prompt;
    }
    public async Task<string> GetPrompt(ImageDomainModel image)
    {
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
        var randomStyle = styles.PickRandom();
        var gptPrompter = new GptPrompter(_gptApi, _stableDiffusionApi, $"{_prompt}, trigger word [{randomStyle}]");
        return await gptPrompter.GetPrompt(image);
    }
}