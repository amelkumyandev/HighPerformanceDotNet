using BenchmarkDotNet.Attributes;
using System.Collections.Generic;

namespace ParallelLoopsAgainstOrdinaryLoops
{
    public class ImageResizingBenchmark
    {
        private List<string> imagePaths;
        private ImageResizer sequentialResizer;
        private ParallelImageResizer parallelResizer;

        [GlobalSetup]
        public void Setup()
        {
            // Initialize image paths (Assume we have a list of image file paths)
            imagePaths = new List<string> {
                "Images/image1.png",
                "Images/image2.png",
                "Images/image3.png",
                "Images/image4.png",
                "Images/image5.png",
                "Images/image6.png",
                "Images/image7.png",
                "Images/image8.png",
                "Images/image9.png",
                "Images/image10.png"
            };
            sequentialResizer = new ImageResizer();
            parallelResizer = new ParallelImageResizer();
        }

        [Benchmark]
        public void SequentialResize()
        {
            sequentialResizer.ResizeImagesSequential(imagePaths, 2.0);  // Doubling the size
        }

        [Benchmark]
        public void ParallelResize()
        {
            parallelResizer.ResizeImagesParallel(imagePaths, 2.0);  // Doubling the size
        }
    }
}
