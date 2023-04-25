using StableDiffusionSdk.Jobs;
using StableDiffusionSdk.Modules.Images;

namespace StableDiffusionSdk.Modules.Prompts
{
    public class DefaultPrompter : IJob<(ImageDomainModel, string), string>
    {
        public Task<string> Run((ImageDomainModel, string) parameters)
        {
            return Task.FromResult(parameters.Item2);
        }
    }
}
