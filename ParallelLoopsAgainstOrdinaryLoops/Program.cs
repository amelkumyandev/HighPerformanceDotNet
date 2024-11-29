
using BenchmarkDotNet.Running;
using ParallelLoopsAgainstOrdinaryLoops;

var summary = BenchmarkRunner.Run<ImageResizingBenchmark>();