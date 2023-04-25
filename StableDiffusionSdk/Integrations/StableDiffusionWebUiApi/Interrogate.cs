using StableDiffusionSdk.Modules.Images;

namespace StableDiffusionSdk.Integrations.StableDiffusionWebUiApi
{
    public static class InterrogateJob
    {

        public static async Task<string> InterrogatePlease(this StableDiffusionApi api, ImageDomainModel image, InterrogationModel model)
        {
            var result = await api.Interrogate(new InterrogateRequest(image, model));
            return result;
        }
    }

}
