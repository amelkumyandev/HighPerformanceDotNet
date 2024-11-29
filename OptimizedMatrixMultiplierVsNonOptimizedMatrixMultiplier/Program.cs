using BenchmarkDotNet.Running;
using OptimizedMatrixMultiplierVsNonOptimizedMatrixMultiplier;

var summary = BenchmarkRunner.Run<MatrixMultiplicationBenchmark>();