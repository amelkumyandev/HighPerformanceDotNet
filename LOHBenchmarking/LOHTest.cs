using BenchmarkDotNet.Attributes;
using System.Runtime;

namespace LOHBenchmarking
{
    public class LOHTest
    {
        private const int LargeObjectSize = 90000; // Bytes

        [Benchmark]
        public void AllocateWithoutCompaction()
        {
            for (int i = 0; i < 1000; i++)
            {
                byte[] largeArray = new byte[LargeObjectSize];
            }
        }

        [Benchmark]
        public void AllocateWithCompaction()
        {
            for (int i = 0; i < 1000; i++)
            {
                byte[] largeArray = new byte[LargeObjectSize];
            }
            GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
            GC.Collect(); // Trigger a full GC and compact the LOH
        }
    }

}
