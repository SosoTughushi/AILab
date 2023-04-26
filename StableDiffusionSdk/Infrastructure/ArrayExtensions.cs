using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StableDiffusionSdk.Infrastructure
{
    public static class ArrayExtensions
    {
        public static T PickRandom<T>(this T[] array)
        {
            var random = new Random();
            return array[random.Next(array.Length)];
        }
    }
}
