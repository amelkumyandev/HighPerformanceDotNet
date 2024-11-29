using BenchmarkDotNet.Running;
using LockFreeOverTraditionalLock;

var summary = BenchmarkRunner.Run<CounterBenchmark>();
