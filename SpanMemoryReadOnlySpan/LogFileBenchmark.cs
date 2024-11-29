using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Toolchains.InProcess.NoEmit;

namespace SpanMemoryReadOnlySpan
{
    [MemoryDiagnoser]
    [Config(typeof(InProcessConfig))] // Use InProcessConfig to run benchmarks in the same process
    public class LogFileBenchmark
    {
        private const string FilePath = "large-log-file.txt";
        private readonly LogFileProcessorOld _oldProcessor = new LogFileProcessorOld();
        private readonly LogFileProcessorNew _newProcessor = new LogFileProcessorNew();

        [Benchmark]
        public async Task ProcessLogFileOld()
        {
            await _oldProcessor.ProcessLogFileAsync(FilePath);
        }

        [Benchmark]
        public async Task ProcessLogFileNew()
        {
            await _newProcessor.ProcessLogFileAsync(FilePath);
        }
    }

    public class InProcessConfig : ManualConfig
    {
        public InProcessConfig()
        {
            AddJob(Job.Default.WithToolchain(InProcessNoEmitToolchain.Instance)); // Run benchmarks in-process
        }
    }
}
