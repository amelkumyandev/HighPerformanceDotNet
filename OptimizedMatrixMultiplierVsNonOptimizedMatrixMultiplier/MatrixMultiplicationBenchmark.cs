using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace OptimizedMatrixMultiplierVsNonOptimizedMatrixMultiplier
{
    public class MatrixMultiplicationBenchmark
    {
        private readonly int[,] _matrixA;
        private readonly int[,] _matrixB;

        public MatrixMultiplicationBenchmark()
        {
            _matrixA = GenerateMatrix(500, 500);
            _matrixB = GenerateMatrix(500, 500);
        }

        [Benchmark(Baseline = true)]
        public void NonOptimized()
        {
            var result = NonOptimizedMatrixMultiplier.MultiplyMatrices(_matrixA, _matrixB);
        }

        [Benchmark]
        public void Optimized()
        {
            var result = OptimizedMatrixMultiplier.MultiplyMatrices(_matrixA, _matrixB);
        }

        private static int[,] GenerateMatrix(int rows, int cols)
        {
            Random rand = new Random();
            int[,] matrix = new int[rows, cols];
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    matrix[i, j] = rand.Next(1, 10);
                }
            }
            return matrix;
        }

        public static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<MatrixMultiplicationBenchmark>();
        }
    }

}
