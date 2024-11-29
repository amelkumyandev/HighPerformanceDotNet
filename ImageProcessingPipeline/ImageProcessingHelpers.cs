using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Jpeg;

namespace ImageProcessingPipeline
{
    public static class ImageProcessingHelpers
    {
        public static Image ResizeImage(Image image, Size newSize)
        {
            image.Mutate(x => x.Resize(newSize.Width, newSize.Height));
            return image;
        }

        public static Image ApplyGrayscaleFilter(Image image)
        {
            image.Mutate(x => x.Grayscale());
            return image;
        }

        public static void SaveImage(Image image)
        {
            string outputDir = Path.Combine(Directory.GetCurrentDirectory(), "output");
            Directory.CreateDirectory(outputDir);

            // Ensure the file path includes the correct extension
            string filePath = Path.Combine(outputDir, $"{Guid.NewGuid()}.jpg"); // Add ".jpg" extension

            // Save the image using JpegEncoder
            image.Save(filePath, new JpegEncoder());
        }
    }
}
