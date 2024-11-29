using BenchmarkDotNet.Attributes;

namespace BatchProcessingExample
{
    [MemoryDiagnoser]
    public class BatchProcessingBenchmark
    {
        private List<double> data;
        private BatchCalculator calculator;

        public int BatchSize = 1000;

        [GlobalSetup]
        public void Setup()
        {
            data = DataGenerator.GenerateData(10000);
            calculator = new BatchCalculator();

            // Clear previous results
            var csvFilePath = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "results.csv");
            if (System.IO.File.Exists(csvFilePath))
            {
                System.IO.File.Delete(csvFilePath);
            }
        }

        [Benchmark(Baseline = true)]
        public void IndividualProcessing()
        {
            calculator.ProcessDataSequentially(data);
        }

        [Benchmark]
        public void BatchProcessing()
        {
            calculator.ProcessDataInBatches(data, BatchSize);
        }

        // Cleanup after each iteration
        [IterationCleanup]
        public void Cleanup()
        {
            GC.Collect();  // Force garbage collection to clean up between iterations
        }
    }
}
