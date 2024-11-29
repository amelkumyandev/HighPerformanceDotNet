using BenchmarkDotNet.Running;
using ThreadPoolTuningBenchmark;

var summary = BenchmarkRunner.Run<ThreadPoolBenchmark>();
