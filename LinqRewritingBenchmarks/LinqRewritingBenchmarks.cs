using BenchmarkDotNet.Attributes;

namespace LinqRewritingBenchmark
{
    [MemoryDiagnoser]
    public class LinqRewritingBenchmarks
    {
        private List<User> users;

        [GlobalSetup]
        public void Setup()
        {
            users = Enumerable.Range(1, 10000)
            .Select(i => new User(i, $"FirstName_{i}", $"LastName_{i}", i % 2 == 0))
            .ToList();
        }

        // Original inefficient query using Count
        [Benchmark]
        public bool OriginalAnyVsCount()
        {
            return users.Count(u => u.IsActive) > 0;
        }

        // Optimized query using Any
        [Benchmark]
        public bool OptimizedAnyVsCount()
        {
            return users.Any(u => u.IsActive);
        }

        // Original inefficient query with OrderBy before Where
        [Benchmark]
        public List<User> OriginalOrderByBeforeWhere()
        {
            return users.OrderBy(u => u.LastName)
                        .ThenBy(u => u.FirstName)
                        .Where(u => u.IsActive)
                        .ToList();
        }

        // Optimized query with Where before OrderBy
        [Benchmark]
        public List<User> OptimizedWhereBeforeOrderBy()
        {
            return users.Where(u => u.IsActive)
                        .OrderBy(u => u.LastName)
                        .ThenBy(u => u.FirstName)
                        .ToList();
        }

    }

}
