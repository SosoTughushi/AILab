using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StableDiffusionSdk.Prompt
{
    public static class PromptHelpers
    {
        public static string ReplacePeople(this string source, string toReplaceWith)
        {
            return source.Replace(" woman ", " XXXX ")
                .Replace(" man ", " XXXX ")
                .Replace(" child ", " XXXX ")
                .Replace(" boy ", " XXXX ")
                .Replace(" girl ", " XXXX ")
                .Replace(" men ", " XXXXs ")
                .Replace(" person ", " XXXX ")
                .Replace(" people ", " XXXXs ")
                .Replace(" monk ", " XXXX ")
                .Replace("XXXX", toReplaceWith);
        }
    }
}
