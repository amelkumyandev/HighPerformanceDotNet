using BenchmarkDotNet.Running;
using ThreadsAreMorePreferableThanTasks;

var summary = BenchmarkRunner.Run<BenchmarkCpuBoundTasks>();
