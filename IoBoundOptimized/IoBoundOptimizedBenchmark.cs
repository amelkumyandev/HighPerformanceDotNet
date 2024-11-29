using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoBoundOptimized
{
    public class IoBoundOptimizedBenchmark
    {
        private List<string> urls;

        [GlobalSetup]
        public void Setup()
        {
            urls = new List<string>
        {
            "https://picsum.photos/id/1/200/300",
            "https://picsum.photos/id/2/200/300",
            "https://picsum.photos/id/3/200/300",
            "https://picsum.photos/id/4/200/300",
            "https://picsum.photos/id/5/200/300",
            "https://picsum.photos/id/6/200/300",
            "https://picsum.photos/id/7/200/300",
            "https://picsum.photos/id/8/200/300",
            "https://picsum.photos/id/9/200/300",
            "https://picsum.photos/id/10/200/300",
            "https://picsum.photos/id/11/200/300",
            "https://picsum.photos/id/12/200/300",
            "https://picsum.photos/id/13/200/300",
            "https://picsum.photos/id/13/200/300",
            "https://picsum.photos/id/14/200/300",
            "https://picsum.photos/id/15/200/300",
        };
        }

        [Benchmark(Baseline = true)]
        public async Task SequentialDownload()
        {
            await FileDownloader.DownloadFilesSequentially(urls);
        }

        [Benchmark]
        public async Task OptimizedDownload()
        {
            await FileDownloader.DownloadFilesOptimized(urls, maxConcurrency: 16);
        }

        // Cleanup after each iteration
        [IterationCleanup]
        public void Cleanup()
        {
            GC.Collect();  // Force garbage collection to clean up between iterations
        }
    }
}
