using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using StableDiffusionSdk.HermansDialogicalSelfTheory;
using StableDiffusionSdk.Prompts;
using StableDiffusionSdk.Workflows;
using StableDiffusionTools.Integrations.EbSynth;
using StableDiffusionTools.Integrations.OpenAi;
using StableDiffusionTools.Integrations.StableDiffusionWebUi;

namespace StableDiffusionSdk
{
    internal class SerficeConfigurator
    {
        public static IServiceProvider Create()
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddSingleton<IConfiguration>(new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build());


            serviceCollection.AddOpenAiServices();

            serviceCollection.AddSingleton<EbSynth>(provider =>
                new EbSynth(provider.GetRequiredService<IConfiguration>()["EbSynthLocation"]));

            serviceCollection.AddSingleton<ComicDiffusionPrompter>();
            serviceCollection.AddSingleton<EldenRingPrompter>();

            serviceCollection.AddSingleton<SmoothZoomInWorkflow>();
            serviceCollection.AddSingleton<VideoToVideoWorkflow>();
            serviceCollection.AddSingleton<WarpInWorkflow>();
            serviceCollection.AddSingleton<PdfToVideoWorkflow>();


            serviceCollection.AddSingleton<SelfOrchestrator>();

            return serviceCollection.BuildServiceProvider();


        }

    }
}
