using OpenAI.GPT3;
using OpenAI.GPT3.Interfaces;
using OpenAI.GPT3.Managers;
using OpenAI.GPT3.ObjectModels;
using OpenAI.GPT3.ObjectModels.RequestModels;

namespace StableDiffusionSdk.Integrations.OpenAi;

public class GptApi
{
    private readonly IOpenAIService _openAiService;

    public GptApi(string apiKey, string apiUrl = "https://api.openai.com")
    {
        var options = new OpenAiOptions
        {
            ApiKey = apiKey,
            BaseDomain = apiUrl
        };

        var httpClientHandler = new HttpClientHandler();
        var httpClient = new HttpClient(httpClientHandler)
        {
            Timeout = TimeSpan.FromSeconds(200)
        };

        _openAiService = new OpenAIService(options, httpClient);
    }

    public async Task<string> GenerateTextAsync(string prompt)
    {
        var request = new ChatCompletionCreateRequest
        {
            Model = Models.ChatGpt3_5Turbo,
            Messages = new List<ChatMessage>
            {
                ChatMessage.FromUser(prompt)
            },
            Temperature = 0.01f
        };

        var response = await _openAiService.ChatCompletion.CreateCompletion(request);

        if (response.Successful)
        {
            return response.Choices.First().Message.Content;
        }

        throw new Exception($"Failed to generate text: {response.Error?.Message}");
    }
}