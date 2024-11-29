using BenchmarkDotNet.Attributes;

namespace PartitionedCollectionVsListT
{
    public class PartitioningBenchmark
    {
        private List<int> _listData;
        private int[] _arrayData;
        private ListProcessing _listProcessing;
        private PartitionedProcessing _partitionedProcessing;

        [Params(10000000)] // 1 million elements
        public int N;

        [GlobalSetup]
        public void GlobalSetup()
        {
            _listData = new List<int>(N);
            _arrayData = new int[N];

            var random = new Random(100); // Seed for reproducibility

            for (int i = 0; i < N; i++)
            {
                int value = random.Next(1, 100);
                _listData.Add(value);
                _arrayData[i] = value;
            }

            _listProcessing = new ListProcessing(_listData);
            _partitionedProcessing = new PartitionedProcessing(_arrayData);
        }

        [IterationSetup]
        public void IterationSetup()
        {
            // Force garbage collection before each iteration
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }

        [Benchmark]
        public void ListProcessing()
        {
            _listProcessing.ProcessData();
        }

        [Benchmark]
        public void PartitionedProcessing()
        {
            _partitionedProcessing.ProcessData();
        }
    }
}
