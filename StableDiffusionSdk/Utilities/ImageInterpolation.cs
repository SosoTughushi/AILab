using System.Drawing;
using FFMpegCore;
using StableDiffusionTools.Domain;
using Image = SixLabors.ImageSharp.Image;

namespace StableDiffusionSdk.Utilities
{
    public static class ImageInterpolation
    {
        public static async Task<ImageDomainModel> InterpolateAsync(ImageDomainModel left, ImageDomainModel right)
        {
            var leftImage = Image.Load(Base64Decode(left.ContentAsBase64String));
            var rightImage = Image.Load(Base64Decode(right.ContentAsBase64String));

            // Save the images as temporary files
            var leftPath = Path.GetTempFileName();
            var rightPath = Path.GetTempFileName();

            leftImage.SaveAsPng(leftPath);
            rightImage.SaveAsPng(rightPath);

            // Create a video from the images
            var videoPath = Path.GetTempFileName();

            FFMpeg.Join(
                videoPath,
                leftPath,
                rightPath);

            // Extract the middle frame of the video as an image
            var resultPath = Path.GetTempFileName();

            await FFMpeg.SnapshotAsync(videoPath, resultPath, new System.Drawing.Size(left.Width, left.Height), TimeSpan.FromSeconds(0.5));

            // Load the snapshot into a SixLabors.ImageSharp.Image
            var resultImage = Image.Load(resultPath);

            // Convert the Image to a byte array
            var resultByteArray = ImageToByteArray(resultImage);
            var resultBase64 = Base64Encode(resultByteArray);

            return new ImageDomainModel(resultBase64, resultImage.Width, resultImage.Height);
        }


        public static byte[] Base64Decode(string base64EncodedData)
        {
            return Convert.FromBase64String(base64EncodedData);
        }

        public static string Base64Encode(byte[] bytes)
        {
            return Convert.ToBase64String(bytes);
        }
        private static byte[] ImageToByteArray(Image image)
        {
            using var memoryStream = new MemoryStream();
            image.SaveAsPng(memoryStream);
            return memoryStream.ToArray();
        }
    }


}
