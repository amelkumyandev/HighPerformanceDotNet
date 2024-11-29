namespace LinqBestPractices
{
    public static class DummyDataGenerator
    {
        public static List<User> GenerateUsers(int userCount, int ordersPerUser)
        {
            return Enumerable.Range(1, userCount).Select(i =>
                new User(i, $"User_{i}", i % 2 == 0, Enumerable.Range(1, ordersPerUser)
                    .Select(j => new Order(j, i, j * 10)).ToList())).ToList();
        }
    }
}
