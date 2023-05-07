using StableDiffusionSdk.Integrations.OpenAi;
using StableDiffusionSdk.Utilities.Prompts;
using StableDiffusionTools.Domain;
using StableDiffusionTools.Integrations.StableDiffusionWebUi;

namespace StableDiffusionSdk.Prompts;

public class GptPrompter : IPrompter
{
    private readonly GptApi _gptApi;
    private readonly StableDiffusionApi _stableDiffusionApi;
    private readonly string _prompt;

    public GptPrompter(GptApi gptApi, StableDiffusionApi stableDiffusionApi, string prompt)
    {
        _gptApi = gptApi;
        _stableDiffusionApi = stableDiffusionApi;
        _prompt = prompt;
    }

    public async Task<string> GetPrompt(ImageDomainModel image)
    {
        var clip = await _stableDiffusionApi.Interrogate(
            new InterrogateRequest(image, InterrogationModel.Clip));

        return await _gptApi.Consolidate(_prompt, clip);
    }
}