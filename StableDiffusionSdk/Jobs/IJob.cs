
namespace StableDiffusionSdk.Jobs
{
    public interface IJob<TParameters, TResult>
    {
        public Task<TResult> Run(TParameters parameters);
    }
}