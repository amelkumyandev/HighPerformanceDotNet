using BenchmarkDotNet.Running;
using LinqRewritingBenchmark;

var summary = BenchmarkRunner.Run<LinqRewritingBenchmarks>();
