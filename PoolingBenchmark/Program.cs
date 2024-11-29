using BenchmarkDotNet.Running;
using PoolingBenchmark.Benchmarks;

var summary = BenchmarkRunner.Run<PoolBenchmarks>();
