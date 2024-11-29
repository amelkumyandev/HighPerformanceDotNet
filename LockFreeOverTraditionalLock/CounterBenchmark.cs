using BenchmarkDotNet.Attributes;

namespace LockFreeOverTraditionalLock
{
    public class CounterBenchmark
    {
        private LockFreeCounter _lockFreeCounter;
        private LockBasedCounter _lockBasedCounter;

        // Define the number of increments and threads for the benchmark
        private const int NumIncrements = 1_000_000;
        private const int NumThreads = 8;

        [GlobalSetup]
        public void Setup()
        {
            _lockFreeCounter = new LockFreeCounter();
            _lockBasedCounter = new LockBasedCounter();
        }

        [Benchmark]
        public void LockFreeCounterTest()
        {
            Parallel.For(0, NumThreads, _ =>
            {
                for (int i = 0; i < NumIncrements / NumThreads; i++)
                {
                    _lockFreeCounter.Increment();
                }
            });
        }

        [Benchmark]
        public void LockBasedCounterTest()
        {
            Parallel.For(0, NumThreads, _ =>
            {
                for (int i = 0; i < NumIncrements / NumThreads; i++)
                {
                    _lockBasedCounter.Increment();
                }
            });
        }

        // This method will run after each iteration to clear the GC
        [IterationSetup]
        public void CleanUpIteration()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
    }
}
