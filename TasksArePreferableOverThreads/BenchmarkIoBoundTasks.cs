using BenchmarkDotNet.Attributes;

namespace TasksArePreferableOverThreads
{
    public class BenchmarkIoBoundTasks
    {
        private const int NumberOfThreads = 10;
        private IoBoundTaskWithThreads _threadImplementation = new IoBoundTaskWithThreads();
        private IoBoundTaskWithTasks _taskImplementation = new IoBoundTaskWithTasks();

        [Benchmark]
        public void BenchmarkWithThreads()
        {
            _threadImplementation.StartTasks(NumberOfThreads);
        }

        [Benchmark]
        public async Task BenchmarkWithTasks()
        {
            await _taskImplementation.StartTasksAsync(NumberOfThreads);
        }
    }

}
