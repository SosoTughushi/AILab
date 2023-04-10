using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StableDiffusionSdk.DomainModels;

namespace StableDiffusionSdk.Jobs.Prompters
{
    public class DefaultPrompter : IJob<(ImageDomainModel, string), string>
    {
        public Task<string> Run((ImageDomainModel, string) parameters)
        {
            return Task.FromResult(parameters.Item2);
        }
    }
}
