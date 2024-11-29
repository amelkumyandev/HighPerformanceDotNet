using BenchmarkDotNet.Attributes;

namespace ThreadsAreMorePreferableThanTasks
{
    public class BenchmarkCpuBoundTasks
    {
        private const int NumberOfThreads = 4;
        private CpuBoundTaskWithThreads _threadImplementation = new CpuBoundTaskWithThreads();
        private CpuBoundTaskWithTasks _taskImplementation = new CpuBoundTaskWithTasks();

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
