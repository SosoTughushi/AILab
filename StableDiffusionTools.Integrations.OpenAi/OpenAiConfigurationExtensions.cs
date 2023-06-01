using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenAI.GPT3;
using OpenAI.GPT3.Managers;

namespace StableDiffusionTools.Integrations.OpenAi
{
    public static class OpenAiConfigurationExtensions
    {
        public static void AddOpenAiServices(this IServiceCollection serviceCollection)
        {

            serviceCollection.AddSingleton<OpenAIService>(provider =>
            {
                var options = new OpenAiOptions
                {
                    ApiKey = provider.GetRequiredService<IConfiguration>()["GptApiKey"],
                    BaseDomain = "https://api.openai.com"
                };

                var httpClientHandler = new HttpClientHandler();
                var httpClient = new HttpClient(httpClientHandler)
                {
                    Timeout = TimeSpan.FromSeconds(200)
                };

                return new OpenAIService(options, httpClient);
            });

            serviceCollection.AddSingleton<GptApi>();
        }
    }
}
