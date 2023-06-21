using MediaToolkit;
using MediaToolkit.Model;
using MediaToolkit.Options;

namespace StableDiffusionSdk.Utilities.Videos;

public static class VideoProcessor
{
    public static IEnumerable<string> DisassembleVideoToFrames(this string mp4VideoFullAddress, int takeEveryXthFrame)
    {
        if (!File.Exists(mp4VideoFullAddress))
        {
            throw new FileNotFoundException("Video file not found.");
        }
        
        var outputFolderForFrames = "FramesTemp";
        try
        {
            if (Directory.Exists(outputFolderForFrames))
            {
                Directory.Delete(outputFolderForFrames, true);
            }
            Directory.CreateDirectory(outputFolderForFrames);

            var frameIndex = 0;
            using var engine = new Engine();
            var inputFile = new MediaFile { Filename = mp4VideoFullAddress };
            engine.GetMetadata(inputFile);

            var frameRate = inputFile.Metadata.VideoData.Fps;
            var duration = inputFile.Metadata.Duration.TotalSeconds;
            var frameCount = (int)(duration * frameRate);

            for (var i = 0; i < frameCount; i += takeEveryXthFrame)
            {
                var fileName = Path.Combine(outputFolderForFrames, $"frame-{frameIndex:00000}.png");
                var outputFile = new MediaFile { Filename = fileName };
                var options = new ConversionOptions { Seek = TimeSpan.FromSeconds((double)i / frameRate) };

                engine.GetThumbnail(inputFile, outputFile, options);
                frameIndex++;
                yield return fileName;
            }
        }
        finally
        {
            Directory.Delete(outputFolderForFrames, true);
        }

    }
}