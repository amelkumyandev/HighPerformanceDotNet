using BenchmarkDotNet.Attributes;

namespace LinqBestPractices
{
    [MemoryDiagnoser]
    public class LinqBestPracticesBenchmarks
    {
        private List<User> users;

        [GlobalSetup]
        public void Setup()
        {
            users = DummyDataGenerator.GenerateUsers(1000, 10); // Generate 1,000 users with 10 orders each
        }

        // Example of a poorly optimized query (premature materialization, filtering after projection)
        [Benchmark]
        public void PoorlyOptimizedQuery()
        {
            var usersList = users.ToList(); // Materialize prematurely
            var filtered = usersList.Select(u => new { u.Name, u.IsActive })
                                    .Where(u => u.IsActive)
                                    .ToList();
        }

        // Example of a well-optimized query (deferred execution, efficient filtering)
        [Benchmark]
        public void WellOptimizedQuery()
        {
            var filtered = users.Where(u => u.IsActive)
                                .Select(u => new { u.Name, u.IsActive })
                                .ToList(); // Materialization happens only after filtering
        }

        // Example of N+1 problem
        [Benchmark]
        public void NPlusOneProblemQuery()
        {
            foreach (var user in users)
            {
                var userOrders = user.Orders.Where(o => o.Amount > 100).ToList(); // N+1 problem
            }
        }

        // Example of a poorly optimized SelectMany query (without any pre-filtering)
        [Benchmark]
        public void NotOptimizedSelectManyQuery()
        {
            var allOrders = users.SelectMany(u => u.Orders).Where(o => o.Amount > 100).ToList(); // No pre-filtering
        }

        // Optimized query with SelectMany
        [Benchmark]
        public void OptimizedSelectManyQuery()
        {
            var orders = users.Where(u => u.Orders.Any(o => o.Amount > 100))
                              .SelectMany(u => u.Orders.Where(o => o.Amount > 100))
                              .ToList();
        }
    }
}
