namespace PartitionedCollectionVsListT
{
    public class ListProcessing
    {
        private readonly List<int> _data;
        private readonly int[] _results;

        public ListProcessing(List<int> data)
        {
            _data = data;
            _results = new int[_data.Count];
        }

        public void ProcessData()
        {
            Parallel.For(0, _data.Count, i =>
            {
                _results[i] = Compute(_data[i]);
            });
        }

        private int Compute(int value)
        {
            // Simulate a CPU-bound operation
            return value * value;
        }
    }
}
