using BenchmarkDotNet.Running;
using IoBoundOptimized;

var summary = BenchmarkRunner.Run<IoBoundOptimizedBenchmark>();