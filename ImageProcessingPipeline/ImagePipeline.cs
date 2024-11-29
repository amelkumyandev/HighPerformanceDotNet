using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Jpeg;
using System.Threading.Tasks.Dataflow;

namespace ImageProcessingPipeline
{
    public class ImagePipeline
    {
        public async Task ProcessImagesAsync(IEnumerable<string> imagePaths)
        {
            // Stage 1: Load Images
            var loadBlock = new TransformBlock<string, Image>(path =>
            {
                return Image.Load(path); // ImageSharp loading
            }, new ExecutionDataflowBlockOptions
            {
                MaxDegreeOfParallelism = 4 // Adjust based on your CPU cores
            });

            // Stage 2: Resize Images
            var resizeBlock = new TransformBlock<Image, Image>(image =>
            {
                image.Mutate(x => x.Resize(800, 600)); // ImageSharp resizing
                return image;
            }, new ExecutionDataflowBlockOptions
            {
                MaxDegreeOfParallelism = 4
            });

            // Stage 3: Apply Grayscale Filter
            var filterBlock = new TransformBlock<Image, Image>(image =>
            {
                image.Mutate(x => x.Grayscale()); // ImageSharp grayscale filter
                return image;
            }, new ExecutionDataflowBlockOptions
            {
                MaxDegreeOfParallelism = 4
            });

            // Stage 4: Save Images
            var saveBlock = new ActionBlock<Image>(image =>
            {
                string outputDir = Path.Combine(Directory.GetCurrentDirectory(), "output");
                Directory.CreateDirectory(outputDir);
                string outputPath = Path.Combine(outputDir, $"{Guid.NewGuid()}.jpg"); // Ensure a valid file path with extension
                image.Save(outputPath, new JpegEncoder()); // ImageSharp save with JpegEncoder
            }, new ExecutionDataflowBlockOptions
            {
                MaxDegreeOfParallelism = 4
            });

            // Link the blocks
            var linkOptions = new DataflowLinkOptions { PropagateCompletion = true };
            loadBlock.LinkTo(resizeBlock, linkOptions);
            resizeBlock.LinkTo(filterBlock, linkOptions);
            filterBlock.LinkTo(saveBlock, linkOptions);

            // Post data to the pipeline
            foreach (var path in imagePaths)
            {
                await loadBlock.SendAsync(path);
            }

            // Signal completion
            loadBlock.Complete();
            await saveBlock.Completion;
        }
    }
}
