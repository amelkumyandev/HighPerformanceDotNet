using BenchmarkDotNet.Running;
using ParallelismAndSIMD;

var summary = BenchmarkRunner.Run<CpuBoundOptimizedBenchmark>();