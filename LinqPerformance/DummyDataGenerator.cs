using LinqPerformance.Model;

namespace LinqPerformance
{
    public static class DummyDataGenerator
    {
        public static List<User> GenerateUsers(int count)
        {
            return Enumerable.Range(1, count)
                .Select(i => new User(i, $"User_{i}", DateTime.UtcNow, new List<Order>()))
                .ToList();
        }

        public static List<Order> GenerateOrders(int userCount, int orderCount)
        {
            return Enumerable.Range(1, orderCount)
                .Select(i => new Order(i, i % userCount, i * 10, DateTime.UtcNow.AddDays(-i)))
                .ToList();
        }
    }
}
