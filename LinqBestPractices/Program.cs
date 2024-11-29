using BenchmarkDotNet.Running;
using LinqBestPractices;

var summary = BenchmarkRunner.Run<LinqBestPracticesBenchmarks>();
