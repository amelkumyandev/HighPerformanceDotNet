using System.Collections.Concurrent;

namespace PartitionedCollectionVsListTUpdated
{
    public class PartitionedProcessing
    {
        private readonly int[] _data;
        private readonly int[] _results;

        public PartitionedProcessing(int[] data)
        {
            _data = data;
            _results = new int[_data.Length];
        }

        public void ProcessData()
        {
            var rangePartitioner = Partitioner.Create(0, _data.Length, 1000); // Adjust chunk size as needed

            Parallel.ForEach(rangePartitioner, (range) =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    _results[i] = Compute(_data[i]);
                }
            });
        }

        private int Compute(int value)
        {
            // Simulate a CPU-bound operation
            return value * value;
        }
    }
}
