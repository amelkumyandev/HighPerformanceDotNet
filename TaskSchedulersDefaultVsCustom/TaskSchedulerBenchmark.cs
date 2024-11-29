using BenchmarkDotNet.Attributes;

namespace TaskSchedulersDefaultVsCustom
{
    public class TaskSchedulerBenchmark
    {
        private readonly TaskFactory _defaultFactory;
        private readonly TaskFactory _limitedConcurrencyFactory;
        private readonly TaskFactory _prioritizedFactory;

        public TaskSchedulerBenchmark()
        {
            _defaultFactory = new TaskFactory();
            _limitedConcurrencyFactory = new TaskFactory(new LimitedConcurrencyTaskScheduler(3));
            _prioritizedFactory = new TaskFactory(new PrioritizedTaskScheduler());
        }

        [Benchmark]
        public async Task DefaultSchedulerBenchmark()
        {
            Task[] tasks = new Task[10];
            for (int i = 0; i < tasks.Length; i++)
            {
                tasks[i] = _defaultFactory.StartNew(() => SimulateWork());
            }
            await Task.WhenAll(tasks);
        }

        [Benchmark]
        public async Task LimitedConcurrencySchedulerBenchmark()
        {
            Task[] tasks = new Task[10];
            for (int i = 0; i < tasks.Length; i++)
            {
                tasks[i] = _limitedConcurrencyFactory.StartNew(() => SimulateWork());
            }
            await Task.WhenAll(tasks);
        }

        [Benchmark]
        public async Task PrioritizedSchedulerBenchmark()
        {
            Task[] tasks = new Task[10];
            for (int i = 0; i < tasks.Length; i++)
            {
                tasks[i] = i % 2 == 0
                    ? _prioritizedFactory.StartNew(() => SimulateWork(), TaskCreationOptions.None)  // High priority
                    : _prioritizedFactory.StartNew(() => SimulateWork(), TaskCreationOptions.PreferFairness); // Low priority
            }
            await Task.WhenAll(tasks);
        }

        private void SimulateWork()
        {
            Thread.Sleep(1000); // Simulate a CPU-bound task
        }
    }
}
