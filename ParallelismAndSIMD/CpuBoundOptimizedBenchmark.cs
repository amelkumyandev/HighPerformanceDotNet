using BenchmarkDotNet.Attributes;

namespace ParallelismAndSIMD
{
    public class CpuBoundOptimizedBenchmark
    {
        private double[] data;

        [GlobalSetup]
        public void Setup()
        {
            int size = 100_000;
            data = new double[size];
            var random = new Random(42);
            for (int i = 0; i < size; i++)
            {
                data[i] = random.NextDouble() * Math.PI;
            }
        }

        [Benchmark(Baseline = true)]
        public double[] OriginalProcessing()
        {
            return SimpleDataProcessing.ProcessData(data);
        }

        [Benchmark]
        public double[] OptimizedProcessing()
        {
            return  OptimizedDataProcessing.ProcessDataOptimized(data);
        }

        [IterationCleanup]
        public void Cleanup()
        {
            GC.Collect();  // Force garbage collection to clean up between iterations
        }
    }
}
