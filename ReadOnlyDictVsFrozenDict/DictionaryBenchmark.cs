using BenchmarkDotNet.Attributes;
using System.Collections.ObjectModel;
using System.Collections.Frozen;

namespace ReadOnlyDictVsFrozenDict
{
    [MemoryDiagnoser]
    public class DictionaryBenchmark
    {
        private const int CollectionSize = 10_000_000;
        private Dictionary<int, string> _data;
        private ReadOnlyDictionary<int, string> _readOnlyDict;
        private FrozenDictionary<int, string> _frozenDict; 
        private int[] _randomAccessIndices;

        [GlobalSetup]
        public void Setup()
        {
            // Initialize a large collection with pre-allocated capacity
            _data = new Dictionary<int, string>(CollectionSize);
            for (int i = 0; i < CollectionSize; i++)
            {
                _data[i] = $"Value {i}";
            }

            // Precompute random access indices for the read benchmarks
            Random random = new Random();
            _randomAccessIndices = Enumerable.Range(0, CollectionSize)
                .OrderBy(_ => random.Next())
                .Take(3000_000) // Perform 3000,000 random reads
                .ToArray();

            // Initialize both dictionaries in the GlobalSetup so they are available for all methods
            _readOnlyDict = new ReadOnlyDictionary<int, string>(_data);
            _frozenDict = _data.ToFrozenDictionary();
        }

        // Benchmark for creating ReadOnlyDictionary
        [Benchmark]
        public void InitializeReadOnlyDictionary()
        {
            var readOnlyDict = new ReadOnlyDictionary<int, string>(_data);
        }

        // Benchmark for creating FrozenDictionary
        [Benchmark]
        public void InitializeFrozenDictionary()
        {
            var frozenDict = _data.ToFrozenDictionary();  // Freezes the dictionary
        }

        // Simulate random read scenario for ReadOnlyDictionary
        [Benchmark]
        public void RandomReadFromReadOnlyDictionary()
        {
            foreach (int index in _randomAccessIndices)
            {
                var value = _readOnlyDict[index];
            }
        }

        // Simulate random read scenario for FrozenDictionary
        [Benchmark]
        public void RandomReadFromFrozenDictionary()
        {
            foreach (int index in _randomAccessIndices)
            {
                var value = _frozenDict[index];
            }
        }

        // Cleanup after each iteration
        [IterationCleanup]
        public void Cleanup()
        {
            GC.Collect();  // Force garbage collection to clean up between iterations
        }
    }
}
