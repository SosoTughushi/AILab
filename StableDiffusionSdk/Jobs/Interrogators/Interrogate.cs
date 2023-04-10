using StableDiffusionSdk.DomainModels;

namespace StableDiffusionSdk.Jobs.Interrogators
{
    public static class InterrogateJob
    {

        public static async Task<string> InterrogatePlease(this StableDiffusionApi.StableDiffusionApi api, ImageDomainModel image, InterrogationModel model)
        {
            var result = await api.Interrogate(new InterrogateRequest(image, model));
            return result;
        }
    }
    
}
