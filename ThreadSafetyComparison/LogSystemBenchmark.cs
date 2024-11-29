using BenchmarkDotNet.Attributes;
using System.Collections.Concurrent;

namespace ThreadSafetyComparison
{
    [MemoryDiagnoser]
    public class DictionaryBenchmark
    {
        private const int CollectionSize = 100_000;  // Total collection size
        private const int UpdatePercentage = 30_000; // 30% of CollectionSize
        private const int DeletePercentage = 20_000; // 20% of CollectionSize

        private Dictionary<int, string> _lockDict;
        private Dictionary<int, string> _monitorDict;
        private Dictionary<int, string> _mutexDict;
        private ConcurrentDictionary<int, string> _concurrentDict;

        private object _lock = new object();
        private readonly object _monitor = new object();
        private readonly Mutex _mutex = new Mutex();

        private int[] _randomUpdateIndices;
        private int[] _randomDeleteIndices;

        [GlobalSetup]
        public void Setup()
        {
            _lockDict = new Dictionary<int, string>();
            _monitorDict = new Dictionary<int, string>();
            _mutexDict = new Dictionary<int, string>();
            _concurrentDict = new ConcurrentDictionary<int, string>();

            // Populate the dictionaries with initial data
            for (int i = 0; i < CollectionSize; i++)
            {
                string value = $"Value {i}";
                _lockDict[i] = value;
                _monitorDict[i] = value;
                _mutexDict[i] = value;
                _concurrentDict[i] = value;
            }

            // Randomly select indices for update and delete operations
            Random random = new Random();
            _randomUpdateIndices = Enumerable.Range(0, CollectionSize).OrderBy(_ => random.Next()).Take(UpdatePercentage).ToArray();
            _randomDeleteIndices = Enumerable.Range(0, CollectionSize).OrderBy(_ => random.Next()).Take(DeletePercentage).ToArray();
        }

        // ---------------- LOCK DICTIONARY BENCHMARK --------------------

        [Benchmark]
        public void AddWithLock()
        {
            Parallel.For(CollectionSize, CollectionSize + 10_000, i =>
            {
                lock (_lock)
                {
                    _lockDict[i] = $"Value {i}";
                }
            });
        }

        [Benchmark]
        public void ReadWithLock()
        {
            Parallel.For(0, CollectionSize, i =>
            {
                lock (_lock)
                {
                    _lockDict.TryGetValue(i, out _);
                }
            });
        }

        [Benchmark]
        public void PartialUpdateWithLock()
        {
            Parallel.ForEach(_randomUpdateIndices, i =>
            {
                lock (_lock)
                {
                    _lockDict[i] = $"Updated Value {i}";
                }
            });
        }

        [Benchmark]
        public void PartialDeleteWithLock()
        {
            Parallel.ForEach(_randomDeleteIndices, i =>
            {
                lock (_lock)
                {
                    _lockDict.Remove(i);
                }
            });
        }

        // ---------------- MONITOR DICTIONARY BENCHMARK --------------------

        [Benchmark]
        public void AddWithMonitor()
        {
            Parallel.For(CollectionSize, CollectionSize + 10_000, i =>
            {
                bool lockTaken = false;
                try
                {
                    Monitor.Enter(_monitor, ref lockTaken);
                    _monitorDict[i] = $"Value {i}";
                }
                finally
                {
                    if (lockTaken) Monitor.Exit(_monitor);
                }
            });
        }

        [Benchmark]
        public void ReadWithMonitor()
        {
            Parallel.For(0, CollectionSize, i =>
            {
                bool lockTaken = false;
                try
                {
                    Monitor.Enter(_monitor, ref lockTaken);
                    _monitorDict.TryGetValue(i, out _);
                }
                finally
                {
                    if (lockTaken) Monitor.Exit(_monitor);
                }
            });
        }

        [Benchmark]
        public void PartialUpdateWithMonitor()
        {
            Parallel.ForEach(_randomUpdateIndices, i =>
            {
                bool lockTaken = false;
                try
                {
                    Monitor.Enter(_monitor, ref lockTaken);
                    _monitorDict[i] = $"Updated Value {i}";
                }
                finally
                {
                    if (lockTaken) Monitor.Exit(_monitor);
                }
            });
        }

        [Benchmark]
        public void PartialDeleteWithMonitor()
        {
            Parallel.ForEach(_randomDeleteIndices, i =>
            {
                bool lockTaken = false;
                try
                {
                    Monitor.Enter(_monitor, ref lockTaken);
                    _monitorDict.Remove(i);
                }
                finally
                {
                    if (lockTaken) Monitor.Exit(_monitor);
                }
            });
        }

        // ---------------- MUTEX DICTIONARY BENCHMARK --------------------

        [Benchmark]
        public void AddWithMutex()
        {
            Parallel.For(CollectionSize, CollectionSize + 10_000, i =>
            {
                _mutex.WaitOne();
                try
                {
                    _mutexDict[i] = $"Value {i}";
                }
                finally
                {
                    _mutex.ReleaseMutex();
                }
            });
        }

        [Benchmark]
        public void ReadWithMutex()
        {
            Parallel.For(0, CollectionSize, i =>
            {
                _mutex.WaitOne();
                try
                {
                    _mutexDict.TryGetValue(i, out _);
                }
                finally
                {
                    _mutex.ReleaseMutex();
                }
            });
        }

        [Benchmark]
        public void PartialUpdateWithMutex()
        {
            Parallel.ForEach(_randomUpdateIndices, i =>
            {
                _mutex.WaitOne();
                try
                {
                    _mutexDict[i] = $"Updated Value {i}";
                }
                finally
                {
                    _mutex.ReleaseMutex();
                }
            });
        }

        [Benchmark]
        public void PartialDeleteWithMutex()
        {
            Parallel.ForEach(_randomDeleteIndices, i =>
            {
                _mutex.WaitOne();
                try
                {
                    _mutexDict.Remove(i);
                }
                finally
                {
                    _mutex.ReleaseMutex();
                }
            });
        }

        // ---------------- CONCURRENT DICTIONARY BENCHMARK --------------------

        [Benchmark]
        public void AddWithConcurrentDictionary()
        {
            Parallel.For(CollectionSize, CollectionSize + 10_000, i =>
            {
                _concurrentDict[i] = $"Value {i}";
            });
        }

        [Benchmark]
        public void ReadWithConcurrentDictionary()
        {
            Parallel.For(0, CollectionSize, i =>
            {
                _concurrentDict.TryGetValue(i, out _);
            });
        }

        [Benchmark]
        public void PartialUpdateWithConcurrentDictionary()
        {
            Parallel.ForEach(_randomUpdateIndices, i =>
            {
                _concurrentDict[i] = $"Updated Value {i}";
            });
        }

        [Benchmark]
        public void PartialDeleteWithConcurrentDictionary()
        {
            Parallel.ForEach(_randomDeleteIndices, i =>
            {
                _concurrentDict.TryRemove(i, out _);
            });
        }

        // Cleanup after each iteration
        [IterationCleanup]
        public void Cleanup()
        {
            GC.Collect();  // Force garbage collection to clean up between iterations
        }
    }
}
