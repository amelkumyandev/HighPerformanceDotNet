using BenchmarkDotNet.Attributes;

namespace ThreadPoolTuningBenchmark
{
    // Benchmark class to compare default, increased, and scaled I/O completion port thread settings
    [MemoryDiagnoser]
    public class ThreadPoolBenchmark
    {
        private ThreadPoolTuning _tuningExample;
        private const int TaskCount = 100;  // Fixed task count for all scenarios

        [GlobalSetup]
        public void Setup()
        {
            _tuningExample = new ThreadPoolTuning();
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            _tuningExample.Dispose();
        }

        // Benchmark with default I/O completion port thread settings (let ThreadPool handle it)
        [Benchmark(Baseline = true)]
        public async Task DefaultIoThreads()
        {
            await _tuningExample.SimulateImageProcessing(TaskCount);
        }

        // Benchmark with scaled I/O completion port threads (based on default, multiplied by 2)
        [Benchmark]
        public async Task ScaledIoThreads()
        {
            _tuningExample.ScaleIoThreadPool(5.0);  // Scale I/O completion port threads by 2x
            await _tuningExample.SimulateImageProcessing(TaskCount);
        }

        // Benchmark with decreased I/O completion port threads (for comparison)
        [Benchmark]
        public async Task DecreasedIoThreads()
        {
            _tuningExample.ScaleIoThreadPool(0.5);  // Decrease I/O threads (50% of default)
            await _tuningExample.SimulateImageProcessing(TaskCount);
        }
    }

}
