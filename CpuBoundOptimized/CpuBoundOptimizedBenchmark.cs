using BenchmarkDotNet.Attributes;

using System.Numerics;

namespace CpuBoundOptimized
{
    public class CpuBoundOptimizedBenchmark
    {
        private double[] data;

        [GlobalSetup]
        public void Setup()
        {
            int size = 10_000_000;
            data = new double[size];
            var random = new Random(42);
            for (int i = 0; i < size; i++)
            {
                // Ensure input values are reasonable for polynomial computation
                data[i] = random.NextDouble() * 1000; // range: [0, 1000)
            }
        }

        [Benchmark(Baseline = true)]
        public double[] OriginalProcessing()
        {
            return ProcessData(data);
        }

        [Benchmark]
        public double[] OptimizedProcessing()
        {
            return ProcessDataOptimized(data);
        }

        public static double[] ProcessData(double[] data)
        {
            double[] result = new double[data.Length];
            for (int i = 0; i < data.Length; i++)
            {
                result[i] = ComplexComputation(data[i]);
            }
            return result;
        }

        private static double ComplexComputation(double input)
        {
            // Polynomial computation example
            return (input * input) + (3 * input) + 2;
        }

        public static double[] ProcessDataOptimized(double[] data)
        {
            int length = data.Length;
            double[] result = new double[length];
            int vectorSize = Vector<double>.Count;
            int i = 0;

            // Precompute constants as vectors
            var threeVector = new Vector<double>(3.0);
            var twoVector = new Vector<double>(2.0);

            // Process data in vectorized chunks
            for (; i <= length - vectorSize; i += vectorSize)
            {
                var inputVector = new Vector<double>(data, i);

                // Perform vectorized operations: (x * x) + (3 * x) + 2
                var squared = Vector.Multiply(inputVector, inputVector);
                var threeX = Vector.Multiply(inputVector, threeVector);
                var sum = Vector.Add(squared, threeX);
                var resultVector = Vector.Add(sum, twoVector);

                resultVector.CopyTo(result, i);
            }

            // Process remaining elements sequentially
            for (; i < length; i++)
            {
                result[i] = ComplexComputation(data[i]);
            }

            return result;
        }

        // Cleanup after each iteration
        [IterationCleanup]
        public void Cleanup()
        {
            GC.Collect();  // Force garbage collection to clean up between iterations
        }
    }
}
