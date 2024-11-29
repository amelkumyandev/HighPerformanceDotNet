
using BenchmarkDotNet.Running;
using ValueTaskVsTask;

var summary = BenchmarkRunner.Run<ValueTaskBenchmark>();