using BenchmarkDotNet.Attributes;
using SixLabors.ImageSharp.PixelFormats;

namespace ImageProcessingPipeline
{
    [MemoryDiagnoser]
    public class ImageProcessingBenchmark
    {
        private List<string> imagePaths;
        private ImagePipeline pipeline;

        [GlobalSetup]
        public void Setup()
        {
            imagePaths = ImageGenerator.GenerateSampleImages(10); // Generate sample images
            pipeline = new ImagePipeline();
        }

        [Benchmark(Baseline = true)]
        public void SequentialProcessing()
        {
            foreach (var path in imagePaths)
            {
                using (var image = SixLabors.ImageSharp.Image.Load<Rgba32>(path)) // ImageSharp loading
                {
                    var newSize = new SixLabors.ImageSharp.Size(800, 600); // Use ImageSharp Size
                    using (var resized = ImageProcessingHelpers.ResizeImage(image, newSize))
                    {
                        using (var filtered = ImageProcessingHelpers.ApplyGrayscaleFilter(resized))
                        {
                            ImageProcessingHelpers.SaveImage(filtered);
                        }
                    }
                }
            }
        }

        [Benchmark]
        public async Task PipelinedProcessing()
        {
            await pipeline.ProcessImagesAsync(imagePaths);
        }
    }
}
