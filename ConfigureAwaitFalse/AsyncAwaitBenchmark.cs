namespace ConfigureAwaitFalse
{
    using BenchmarkDotNet.Attributes;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class AsyncAwaitBenchmark
    {
        private readonly DataService _dataService = new DataService();
        private readonly DataProcessor _dataProcessor = new DataProcessor();
        private readonly FileWriter _fileWriter = new FileWriter();

        private const string url = "https://jsonplaceholder.typicode.com/posts";  // Real API endpoint
        private const string filePath = "output.txt";  // Output file path

        // List of URLs to simulate multiple concurrent fetches
        private readonly List<string> _urls = Enumerable.Repeat(url,20).ToList();  // 20 concurrent requests

        [Benchmark(Baseline = true)]
        public async Task DefaultAsyncAwait()
        {
            await RunAsync(false);  // Default async/await behavior (without ConfigureAwait(false))
        }

        [Benchmark]
        public async Task OptimizedWithConfigureAwaitFalse()
        {
            await RunAsync(true);  // Optimized behavior with ConfigureAwait(false)
        }

        private async Task RunAsync(bool useConfigureAwait)
        {
            // Fetch data concurrently from multiple URLs
            var fetchTasks = _urls.Select(url => _dataService.FetchDataAsync(url)).ToList();

            // Wait for all fetch tasks to complete
            var fetchedData = await Task.WhenAll(fetchTasks).ConfigureAwait(useConfigureAwait);

            // Process each fetched dataset concurrently
            var processTasks = fetchedData.Select(data => _dataProcessor.ProcessDataAsync(data)).ToList();

            var processedData = await Task.WhenAll(processTasks).ConfigureAwait(useConfigureAwait);

            // Write all processed datasets to a file concurrently
            var writeTasks = processedData.Select((data, index) =>
                _fileWriter.WriteToFileAsync(data, $"{filePath}-{index}.txt")).ToList();

            await Task.WhenAll(writeTasks).ConfigureAwait(useConfigureAwait);
        }
    }


}
