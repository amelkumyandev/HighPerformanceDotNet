using BenchmarkDotNet.Attributes;
using PoolingBenchmark.DataProcessing;
using System.Buffers;

namespace PoolingBenchmark.Benchmarks
{
    [MemoryDiagnoser]
    public class PoolBenchmarks
    {
        private const int NumElements = 1000000;
        private readonly DataProcessor processor = new DataProcessor();

        [Benchmark]
        public void StandardArrayAllocation()
        {
            int[] buffer = new int[NumElements];  // Allocate new array
            processor.ProcessData(buffer);
        }

        [Benchmark]
        public void ArrayPoolAllocation()
        {
            var arrayPool = ArrayPool<int>.Shared;
            int[] buffer = arrayPool.Rent(NumElements);  // Rent buffer from the pool

            try
            {
                processor.ProcessData(buffer);  // Process data
            }
            finally
            {
                arrayPool.Return(buffer);  // Return the buffer to the pool
            }
        }
    }
}
