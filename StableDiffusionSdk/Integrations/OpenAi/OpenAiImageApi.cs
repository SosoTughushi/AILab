using OpenAI.GPT3;
using OpenAI.GPT3.Interfaces;
using OpenAI.GPT3.Managers;
using OpenAI.GPT3.ObjectModels.RequestModels;
using StableDiffusionSdk.Integrations.StableDiffusionWebUiApi;
using StableDiffusionSdk.Modules.Images;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace StableDiffusionSdk.Integrations.OpenAi
{
    public class OpenAiImageApi : ITextToImage
    {
        
        private readonly IOpenAIService _openAiService;

        public OpenAiImageApi(string apiKey, string apiUrl = "https://api.openai.com")
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

        public async Task<ImageDomainModel> TextToImage(Text2ImgRequest request)
        {
            var result = await _openAiService.Image.CreateImage(new ImageCreateRequest(request.Prompt)
            {
                Size = $"{request.Width}x{request.Height}"
            });

            if (!result.Successful)
            {
                throw new Exception(result.Error.Message);
            }

            var firstResult = result.Results.First();

            var base64 = await GetImageAsBase64Async(firstResult.Url);

            return new ImageDomainModel(base64, request.Width, request.Height);
        }

        private static async Task<string> GetImageAsBase64Async(string imageUrl)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                using (HttpResponseMessage response = await httpClient.GetAsync(imageUrl))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        using (Stream stream = await response.Content.ReadAsStreamAsync())
                        {
                            using (MemoryStream memoryStream = new MemoryStream())
                            {
                                using (var image = Image.Load(stream))
                                {
                                    image.Save(memoryStream, new SixLabors.ImageSharp.Formats.Png.PngEncoder());
                                }

                                byte[] imageBytes = memoryStream.ToArray();
                                string base64String = Convert.ToBase64String(imageBytes);
                                return base64String;
                            }
                        }
                    }
                    else
                    {
                        throw new Exception($"Failed to download the image: {response.ReasonPhrase}");
                    }
                }
            }
        }
    }
}
