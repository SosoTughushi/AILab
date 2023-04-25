namespace StableDiffusionSdk.Modules.Images
{
    public class ImagePersister
    {
        public string OutputFolder { get; }
        private int _savedImageCount;

        public ImagePersister(string baseOutputFolder)
        {
            int folderNumber = 1;

            do
            {
                OutputFolder = Path.Combine(baseOutputFolder, folderNumber.ToString());
                folderNumber++;
            } while (Directory.Exists(OutputFolder));

            Directory.CreateDirectory(OutputFolder);
            Directory.CreateDirectory($"{OutputFolder}");
        }

        public async Task<ImageDomainModel> Persist(ImageDomainModel image)
        {
            _savedImageCount++;
            var fileName = Path.Combine(OutputFolder, $"{_savedImageCount:D5}.jpg");
            await image.SaveAsJpg(fileName);

            return image;
        }
    }
}
