using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StableDiffusionSdk.Integrations.StableDiffusionWebUiApi.Models
{
    public record StableDiffusionModel(string Name, List<string> TriggerWords)
    {
        public static StableDiffusionModel JamesWebbDeepSpace => new StableDiffusionModel(
            Name: "JWST-Deep-Space",
            TriggerWords: new List<string>()
            {
                "JWST"
            });

        public static StableDiffusionModel ComicDiffusion => new StableDiffusionModel(
            Name: "comic-diffusion-V2",
            TriggerWords: new List<string>()
            {
                "charliebo artstyle",
                "holliemengert artstyle",
                "marioalberti artstyle",
                "pepelarraz artstyle",
                "andreasrocha artstyle",
                "jamesdaly artstyle",
                "comicmay artsyle"
            });
    };
}
