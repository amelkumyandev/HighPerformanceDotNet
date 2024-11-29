namespace LinqBestPractices
{
    public record User(int Id, string Name, bool IsActive, List<Order> Orders);
}
