namespace BatchProcessingExample
{
    public static class DataGenerator
    {
        public static List<double> GenerateData(int count)
        {
            var list = new List<double>(count);
            var random = new Random();
            for (int i = 0; i < count; i++)
            {
                list.Add(random.NextDouble() * 1000);
            }
            return list;
        }
    }
}
