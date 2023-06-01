using Accord.Imaging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StableDiffusionSdk;
using StableDiffusionSdk.HermansDialogicalSelfTheory;
using StableDiffusionSdk.Prompts;
using StableDiffusionTools.ImageUtilities;


var provider = SerficeConfigurator.Create();



var consoleOrchestrator = provider.GetRequiredService<SelfOrchestrator>();


await consoleOrchestrator.Run("Abortion");




