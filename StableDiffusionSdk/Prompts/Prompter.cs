using StableDiffusionTools.Domain;

namespace StableDiffusionSdk.Prompts
{
    public interface IPrompter
    {
        Task<string> GetPrompt(ImageDomainModel image);
    }

    public static class CachedPromptExtensions
    {
        public static IPrompter Cached(this IPrompter decorated, int cacheDuration)
        {
            return new CachedPrompt(decorated, cacheDuration);
        }
    }

    public class DefaultPrompoter : IPrompter
    {
        private readonly string _prompt;

        public DefaultPrompoter(string prompt)
        {
            _prompt = prompt;
        }
        public Task<string> GetPrompt(ImageDomainModel image)
        {
            return Task.FromResult(_prompt);
        }
    }

    public class CachedPrompt : IPrompter
    {
        private readonly IPrompter _decorated;
        private readonly int _cacheDuration;

        private int _count;
        private string? _cached;

        public CachedPrompt(IPrompter decorated, int cacheDuration)
        {
            _decorated = decorated;
            _cacheDuration = cacheDuration;
        }
        public async Task<string> GetPrompt(ImageDomainModel image)
        {
            _count++;
            if (_cached == null || _count % _cacheDuration == 0)
            {
                _cached = await _decorated.GetPrompt(image);
            }

            return _cached;
        }
    }
}