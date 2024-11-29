using BenchmarkDotNet.Running;
using TasksArePreferableOverThreads;

var summary = BenchmarkRunner.Run<BenchmarkIoBoundTasks>();