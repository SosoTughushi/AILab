using SixLabors.ImageSharp.Formats.Jpeg;

namespace StableDiffusionSdk.Modules.Images;

public static class ImageHelpers
{
    public static async Task<ImageDomainModel> ReadImage(this string imagePath)
    {
        using var image = await Image.LoadAsync<Rgba32>(imagePath);
        int width = image.Width;
        int height = image.Height;

        byte[] imageBytes;
        await using (var ms = new MemoryStream())
        {
            await image.SaveAsync(ms, new JpegEncoder());
            imageBytes = ms.ToArray();
        }

        var base64String = Convert.ToBase64String(imageBytes);

        return new ImageDomainModel(base64String, width, height);
    }

    public static async Task<string> SaveAsJpg(this ImageDomainModel image, string relativePath)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(relativePath)!);

        // Convert image.ContentAsBase64String to byte array and save it on fullPath
        byte[] imageBytes = Convert.FromBase64String(image.ContentAsBase64String);
        await File.WriteAllBytesAsync(relativePath, imageBytes);

        return Path.GetFullPath(relativePath);
    }
}