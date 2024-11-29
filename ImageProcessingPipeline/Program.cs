
using BenchmarkDotNet.Running;
using ImageProcessingPipeline;

var summary = BenchmarkRunner.Run<ImageProcessingBenchmark>();