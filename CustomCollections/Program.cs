using BenchmarkDotNet.Running;
using CustomCollections;

var summary = BenchmarkRunner.Run<HashTableBenchmarks>();