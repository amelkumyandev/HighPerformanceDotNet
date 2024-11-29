using BenchmarkDotNet.Running;
using PLINQ;

// Run the benchmarks
var summary = BenchmarkRunner.Run<ParallelLinqBenchmarks>();