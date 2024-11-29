using BenchmarkDotNet.Running;
using LINQDefferedImmediate;

var summary = BenchmarkRunner.Run<LinqExecutionBenchmarks>();
