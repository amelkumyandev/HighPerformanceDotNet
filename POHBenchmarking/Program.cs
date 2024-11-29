using BenchmarkDotNet.Running;
using POHBenchmarking;

var summary = BenchmarkRunner.Run<PinnedObjectTest>();
