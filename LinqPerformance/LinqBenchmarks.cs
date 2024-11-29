using BenchmarkDotNet.Attributes;
using LinqPerformance.Model;
using System.Collections.Generic;
using System.Linq;


namespace LinqPerformance
{
    [MemoryDiagnoser]
    public class LinqBenchmarks
    {
        private List<User> users;
        private List<Order> orders;

        [GlobalSetup]
        public void Setup()
        {
            users = DummyDataGenerator.GenerateUsers(1000);
            orders = DummyDataGenerator.GenerateOrders(1000, 5000);
        }

        // Poorly Optimized Query (N+1 Problem)
        public List<User> PoorlyOptimizedQuery()
        {
            var users = DummyDataGenerator.GenerateUsers(1000);
            foreach (var user in users)
            {
                var userOrders = orders
                    .Where(o => o.UserId == user.Id && o.Amount > 500)
                    .ToList();
                // Processing logic here
            }
            return users;
        }

        // Well Optimized Query (Using Joins)
        public List<User> WellOptimizedQuery()
        {
            var users = DummyDataGenerator.GenerateUsers(1000);
            var orders = DummyDataGenerator.GenerateOrders(1000, 5000);

            var usersWithHighValueOrders = orders.Where(o => o.Amount > 500)
                .Join(users, o => o.UserId, u => u.Id, (o, u) => u)
                .Distinct()
                .ToList();

            return usersWithHighValueOrders;
        }

        [Benchmark]
        public void PoorlyOptimizedQueryBenchmark()
        {
            PoorlyOptimizedQuery();
        }

        [Benchmark]
        public void WellOptimizedQueryBenchmark()
        {
            WellOptimizedQuery();
        }
    }
}
