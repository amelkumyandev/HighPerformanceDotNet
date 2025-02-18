using BenchmarkDotNet.Running;
using CountdownEvenVsTaskWhenAll;

BenchmarkRunner.Run<ConcurrencyBenchmark>();
