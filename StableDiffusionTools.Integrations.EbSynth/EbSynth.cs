using System.Diagnostics;
using StableDiffusionTools.Domain;
using StableDiffusionTools.ImageUtilities;

namespace StableDiffusionTools.Integrations.EbSynth
{
    public class EbSynth
    {
        private readonly string _ebsynthPath;

        public EbSynth(string ebsynthPath = "path/to/ebsynth.exe")
        {
            _ebsynthPath = ebsynthPath;
        }


        public async Task<ImageDomainModel> TransferStyle(ImageDomainModel style, ImageDomainModel source,
            ImageDomainModel target)
        {
            var persistor = new ImagePersister("eb-synth-temp");
            var stylePath = await persistor.Persist(style);
            var sourceGuidePath = await persistor.Persist(source);
            var targetGuidePath = await persistor.Persist(target);

            return await RunEbsynth(stylePath, sourceGuidePath, targetGuidePath);
        }

        public async Task<ImageDomainModel> RunEbsynth(
            string stylePath,
            string sourceGuidePath,
            string targetGuidePath
        )
        {
            var outputPath = Path.Combine(Path.GetDirectoryName(_ebsynthPath)!,"output.png");
            await Task.Run(() =>
            {
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = _ebsynthPath,
                        Arguments =
                            $"-style \"{stylePath}\" -guide \"{sourceGuidePath}\" \"{targetGuidePath}\" -output \"{outputPath}\"",
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true,
                    }
                };

                process.Start();
                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
                Console.WriteLine(output);
            });
            return await outputPath.ReadImage();
        }
    }
}