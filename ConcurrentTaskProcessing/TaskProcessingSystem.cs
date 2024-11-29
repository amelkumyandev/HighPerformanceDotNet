using System.Collections.Concurrent;

namespace ConcurrentTaskProcessing
{
    public class TaskProcessingSystem
    {
        // ConcurrentBag to store logs of all processed tasks
        private ConcurrentBag<string> _processedTaskLogs = new ConcurrentBag<string>();

        // ConcurrentDictionary to act as a cache for task lookups
        private ConcurrentDictionary<int, string> _taskCache = new ConcurrentDictionary<int, string>();

        // Simulate receiving and processing tasks
        public async Task ProcessTasksConcurrently(int taskCount)
        {
            // Simulate receiving tasks in parallel
            Parallel.For(0, taskCount, async (i) =>
            {
                // Check if the task was already processed (fast lookup in the dictionary)
                if (!_taskCache.ContainsKey(i))
                {
                    string result = await ProcessTaskAsync(i);

                    // Add to the task cache
                    _taskCache.TryAdd(i, result);

                    // Log the result in the ConcurrentBag
                    _processedTaskLogs.Add($"Task {i}: {result}");
                }
            });
        }

        // Simulate task processing
        private Task<string> ProcessTaskAsync(int taskId)
        {
            // Simulate some processing time
            Task.Delay(500).Wait();

            // Return a processed result
            return Task.FromResult($"Processed Task {taskId}");
        }

        // Print all the processed logs (from the ConcurrentBag)
        public void PrintLogs()
        {
            foreach (var log in _processedTaskLogs)
            {
                Console.WriteLine(log);
            }
        }

        // Lookup task result from the ConcurrentDictionary
        public string LookupTaskResult(int taskId)
        {
            if (_taskCache.TryGetValue(taskId, out string result))
            {
                return result;
            }
            return $"Task {taskId} not found in cache.";
        }
    }

}
