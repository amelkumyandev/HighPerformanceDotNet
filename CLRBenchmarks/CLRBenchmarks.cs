using BenchmarkDotNet.Attributes;

namespace CLRBenchmarksService
{
    [MemoryDiagnoser]
    public class CLRBenchmarks
    {
        private const int NumIterations = 1000000;

        // Class-based version
        [Benchmark]
        public void ClassBenchmark()
        {
            var points = new PointClass[NumIterations];
            for (int i = 0; i < NumIterations; i++)
            {
                points[i] = new PointClass { X = i, Y = i };
            }
        }

        // Struct-based version
        [Benchmark]
        public void StructBenchmark()
        {
            var points = new PointStruct[NumIterations];
            for (int i = 0; i < NumIterations; i++)
            {
                points[i] = new PointStruct { X = i, Y = i };
            }
        }

        // Record-based version
        [Benchmark]
        public void RecordBenchmark()
        {
            var points = new PointRecord[NumIterations];
            for (int i = 0; i < NumIterations; i++)
            {
                points[i] = new PointRecord(i, i);
            }
        }

        // Struct with loop unrolling
        [Benchmark]
        public void StructWithUnrollingBenchmark()
        {
            var points = new PointStruct[NumIterations];
            int i = 0;
            for (; i < NumIterations - 3; i += 4)
            {
                points[i] = new PointStruct { X = i, Y = i };
                points[i + 1] = new PointStruct { X = i + 1, Y = i + 1 };
                points[i + 2] = new PointStruct { X = i + 2, Y = i + 2 };
                points[i + 3] = new PointStruct { X = i + 3, Y = i + 3 };
            }
            // Handle remaining iterations
            for (; i < NumIterations; i++)
            {
                points[i] = new PointStruct { X = i, Y = i };
            }
        }
    }

    // Class definition
    public class PointClass
    {
        public int X { get; set; }
        public int Y { get; set; }
    }

    // Struct definition
    public struct PointStruct
    {
        public int X { get; set; }
        public int Y { get; set; }
    }

    // Record definition
    public record PointRecord(int X, int Y);
}
