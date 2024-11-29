using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Formats.Jpeg;

namespace ImageProcessingPipeline
{
    public static class ImageGenerator
    {
        public static List<string> GenerateSampleImages(int count)
        {
            string dir = Path.Combine(Directory.GetCurrentDirectory(), "images");
            Directory.CreateDirectory(dir);

            var paths = new List<string>();
            for (int i = 0; i < count; i++)
            {
                string filePath = Path.Combine(dir, $"image_{i}.jpg"); // Valid file extension
                CreateSampleImage(filePath, i);
                paths.Add(filePath);
            }
            return paths;
        }

        private static void CreateSampleImage(string path, int seed)
        {
            int width = 1024;
            int height = 768;

            // Create a new image with a specific pixel format (Rgba32)
            using (var image = new Image<Rgba32>(width, height))
            {
                var random = new Random(seed);

                // Set pixels in the image
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        // Generate random colors
                        byte r = (byte)random.Next(256);
                        byte g = (byte)random.Next(256);
                        byte b = (byte)random.Next(256);

                        // Set the pixel color using ImageSharp's way
                        image[x, y] = new Rgba32(r, g, b);
                    }
                }

                // Save the image in JPEG format with valid extension
                image.Save(path, new JpegEncoder());
            }
        }
    }
}
