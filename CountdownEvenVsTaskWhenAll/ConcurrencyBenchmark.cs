using BenchmarkDotNet.Attributes;

namespace CountdownEvenVsTaskWhenAll
{
    // We use MemoryDiagnoser for additional GC/memory insights
    [MemoryDiagnoser]
    public class ConcurrencyBenchmark
    {
        // Number of parallel tasks to spawn
        [Params(100)]
        public int WorkItemCount { get; set; }

        // I/O-bound simulated delay (in milliseconds)
        [Params(100)]
        public int DelayMs { get; set; }

        // CPU-bound 'work multiplier' to increase loop iterations
        [Params(1_000_000)]
        public int CpuWorkIterations { get; set; }

        // -----------------------------------------------------------
        // I/O-BOUND SCENARIOS
        // -----------------------------------------------------------

        [Benchmark]
        public async Task IO_Bound_TaskWhenAll()
        {
            var tasks = new Task[WorkItemCount];
            for (int i = 0; i < WorkItemCount; i++)
            {
                tasks[i] = SimulateIOWorkAsync(DelayMs);
            }
            await Task.WhenAll(tasks);
        }

        [Benchmark]
        public void IO_Bound_CountdownEvent()
        {
            using var cde = new CountdownEvent(WorkItemCount);

            for (int i = 0; i < WorkItemCount; i++)
            {
                Task.Run(async () =>
                {
                    try
                    {
                        await SimulateIOWorkAsync(DelayMs);
                    }
                    finally
                    {
                        cde.Signal();
                    }
                });
            }

            // Blocking the current thread until all tasks signal
            cde.Wait();
        }

        // -----------------------------------------------------------
        // CPU-BOUND (HPC-LIKE) SCENARIOS
        // -----------------------------------------------------------

        [Benchmark]
        public void CPU_Bound_TaskWhenAll()
        {
            var tasks = new Task[WorkItemCount];
            for (int i = 0; i < WorkItemCount; i++)
            {
                tasks[i] = Task.Run(() => SimulateCpuWork(CpuWorkIterations));
            }

            // We block here just for benchmark comparison
            Task.WhenAll(tasks).GetAwaiter().GetResult();
        }

        [Benchmark]
        public void CPU_Bound_CountdownEvent()
        {
            using var cde = new CountdownEvent(WorkItemCount);

            for (int i = 0; i < WorkItemCount; i++)
            {
                Task.Run(() =>
                {
                    try
                    {
                        SimulateCpuWork(CpuWorkIterations);
                    }
                    finally
                    {
                        cde.Signal();
                    }
                });
            }

            cde.Wait();
        }

        // -----------------------------------------------------------
        // HELPER METHODS
        // -----------------------------------------------------------

        private static async Task SimulateIOWorkAsync(int delayMs)
        {
            // Simulate some asynchronous I/O, e.g., reading a file or fetching from an API
            await Task.Delay(delayMs);
        }

        private static void SimulateCpuWork(int iterations)
        {
            // Simple busy loop to simulate CPU-bound work
            double value = 0;
            for (int i = 0; i < iterations; i++)
            {
                // Arbitrary computation
                value += Math.Sqrt(i);
            }
        }
    }
}
