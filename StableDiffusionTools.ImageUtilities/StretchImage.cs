using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using StableDiffusionTools.Domain;
using Image = SixLabors.ImageSharp.Image;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.Formats.Png;
using System.Numerics;
using OpenCvSharp;
using SixLabors.ImageSharp.Processing.Processors;

namespace StableDiffusionTools.ImageUtilities;

public static class StretchImageExtensions
{
    public static async Task<ImageDomainModel> LogarithmicWarpLeft(this ImageDomainModel source, double k)
    {
        // Load the image from base64 string
        Mat mat = Cv2.ImDecode(Convert.FromBase64String(source.ContentAsBase64String), ImreadModes.Color);

        // Compute the logarithmic function to apply
        double centerX = mat.Width / 2.0;
        Func<double, double> logFunction = (x) => x + k * Math.Log(Math.Abs(centerX - x));

        // Compute the new pixel coordinates for each pixel in the image
        Mat map_x = new Mat(mat.Size(), MatType.CV_32FC1);
        Mat map_y = new Mat(mat.Size(), MatType.CV_32FC1);
        for (int y = 0; y < mat.Rows; y++)
        {
            for (int x = 0; x < mat.Cols; x++)
            {
                double newX = logFunction(x);
                map_x.At<float>(y, x) = (float)newX;
                map_y.At<float>(y, x) = (float)y;
            }
        }

        // Apply the perspective transform to the image
        Mat warpedMat = new Mat();
        Cv2.Remap(mat, warpedMat, map_x, map_y, InterpolationFlags.Linear);

        // Convert the resulting image to a Bitmap and return it
        warpedMat.ConvertTo(warpedMat, MatType.CV_8UC3);
        Cv2.ImEncode(".jpg", warpedMat, out var imageData);
        var resultBitmap = new Bitmap(new MemoryStream(imageData));

        return ImageDomainModel.FromSystemDrawingImage(resultBitmap, source);
    }
}

public record StretchDirection(double DeltaX, double DeltaY);