using BenchmarkDotNet.Attributes;

namespace TaskFactoryAndCustomTaskScheduler
{
    public class TaskBenchmark
    {
        private TaskFactory _defaultFactory;
        private TaskFactory _customFactory;

        [GlobalSetup]
        public void Setup()
        {
            _defaultFactory = new TaskFactory();
            _customFactory = new TaskFactory(
                cancellationToken: CancellationToken.None,
                creationOptions: TaskCreationOptions.None,
                continuationOptions: TaskContinuationOptions.None,
                scheduler: new PrioritizedLimitedConcurrencyScheduler(3));
        }

        [Benchmark]
        public async Task DefaultFactoryBenchmark()
        {
            Task[] tasks = new Task[10];
            for (int i = 0; i < tasks.Length; i++)
            {
                tasks[i] = _defaultFactory.StartNew(() => SimulateWork());
            }
            await Task.WhenAll(tasks);
        }

        [Benchmark]
        public async Task CustomFactoryBenchmark()
        {
            Task[] tasks = new Task[10];
            for (int i = 0; i < tasks.Length; i++)
            {
                tasks[i] = _customFactory.StartNew(() => SimulateWork());
            }
            await Task.WhenAll(tasks);
        }

        private void SimulateWork()
        {
            Thread.Sleep(1000); // Simulate work
        }
    }

}
