namespace ConfigureAwaitFalse
{
    public class DataProcessor
    {
        public async Task<string[]> ProcessDataAsync(string data)
        {
            return await Task.Run(() =>
            {
                var lines = data.Split('\n');
                var processedData = lines
                    .Where(line => !string.IsNullOrWhiteSpace(line))
                    .Select(line => line.ToUpper())
                    .ToArray();

                return processedData;
            });
        }
    }
}
