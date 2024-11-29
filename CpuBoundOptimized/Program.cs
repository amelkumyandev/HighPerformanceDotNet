using BenchmarkDotNet.Running;
using CpuBoundOptimized;

var summary = BenchmarkRunner.Run<CpuBoundOptimizedBenchmark>();