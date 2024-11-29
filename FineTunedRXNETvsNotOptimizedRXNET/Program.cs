using BenchmarkDotNet.Running;
using FineTunedRXNETvsNotOptimizedRXNET;

var summary = BenchmarkRunner.Run<NumberProcessorBenchmark>();

