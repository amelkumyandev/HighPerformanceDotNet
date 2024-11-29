using BenchmarkDotNet.Running;
using ThreadSafetyComparison;

var summary = BenchmarkRunner.Run<DictionaryBenchmark>();