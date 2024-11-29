
using BenchmarkDotNet.Attributes;

namespace LINQDefferedImmediate
{

    public static class DummyDataGenerator
    {
        public static List<User> GenerateUsers(int count)
        {
            return Enumerable.Range(1, count)
                .Select(i => new User(i, $"User_{i}", DateTime.UtcNow.AddDays(-i)))
                .ToList();
        }
    }

    [MemoryDiagnoser]
    public class LinqExecutionBenchmarks
    {
        private List<User> users;

        [GlobalSetup]
        public void Setup()
        {
            users = DummyDataGenerator.GenerateUsers(100000); // Generate 100,000 users
        }

        // Deferred Execution: Multiple enumerations cause the query to execute multiple times
        [Benchmark]
        public void DeferredExecutionBenchmark()
        {
            var query = users.Where(u => u.CreatedAt > DateTime.UtcNow.AddDays(-30));

            // Query is executed twice
            var count = query.Count();
            var countAgain = query.Count();
        }

        // Immediate Execution: The query is executed once and materialized into a list
        [Benchmark]
        public void ImmediateExecutionBenchmark()
        {
            var query = users.Where(u => u.CreatedAt > DateTime.UtcNow.AddDays(-30)).ToList();

            // Query is executed once, then cached in memory
            var count = query.Count();
            var countAgain = query.Count();
        }
    }
}
