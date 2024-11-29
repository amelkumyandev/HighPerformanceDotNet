using BenchmarkDotNet.Attributes;
namespace PLINQ
{
    [MemoryDiagnoser]
    public class ParallelLinqBenchmarks
    {
        private int[] numbers;

        [GlobalSetup]
        public void Setup()
        {
            // Generate 1 million integers (1 to 1,000,000)
            numbers = Enumerable.Range(1, 1000000).ToArray();
        }

        // Sequential LINQ query (non-parallel)
        [Benchmark]
        public int[] SequentialQuery()
        {
            // Sequential query: filter even numbers
            return numbers.Where(n => n % 2 == 0).ToArray();
        }

        // Parallel LINQ query using AsParallel()
        [Benchmark]
        public int[] ParallelQuery()
        {
            // Parallel query: filter even numbers
            return numbers.AsParallel()
                          .Where(n => n % 2 == 0)
                          .ToArray();
        }

        // Parallel query with controlled degree of parallelism (limiting to 4 threads)
        [Benchmark]
        public int[] ParallelQueryWithDegreeOfParallelism()
        {
            return numbers.AsParallel()
                          .WithDegreeOfParallelism(4) // Limit to 4 parallel threads
                          .Where(n => n % 2 == 0)
                          .ToArray();
        }

        // Ordered parallel query to maintain element order in the output
        [Benchmark]
        public int[] ParallelOrderedQuery()
        {
            return numbers.AsParallel()
                          .AsOrdered() // Ensure output retains input order
                          .Where(n => n % 2 == 0)
                          .ToArray();
        }

       /* // Using ForAll for side-effect operations (e.g., logging)
        [Benchmark]
        public void ParallelForAllQuery()
        {
            // Print even numbers to the console in parallel (side-effect)
            numbers.AsParallel()
                   .Where(n => n % 2 == 0)
                   .ForAll(n => Console.WriteLine(n));
        }
       */
    }
}
