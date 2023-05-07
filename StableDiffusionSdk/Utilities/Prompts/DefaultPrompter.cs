using StableDiffusionSdk.Jobs;
using StableDiffusionTools.Domain;

namespace StableDiffusionSdk.Utilities.Prompts
{
    public class DefaultPrompter : IJob<(ImageDomainModel, string), string>
    {
        public Task<string> Run((ImageDomainModel, string) parameters)
        {
            return Task.FromResult(parameters.Item2);
        }
    }
}
