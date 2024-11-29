using BenchmarkDotNet.Running;
using LinqPerformance;

var summary = BenchmarkRunner.Run<LinqBenchmarks>();
