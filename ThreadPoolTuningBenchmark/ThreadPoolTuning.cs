using System.Drawing;

namespace ThreadPoolTuningBenchmark
{
    public class ThreadPoolTuning : IDisposable
    {
        private static readonly HttpClient _httpClient = new HttpClient();

        // Method to scale the I/O threads based on the system's default thread count
        public void ScaleIoThreadPool(double scaleFactor)
        {
            int minWorkerThreads, minCompletionPortThreads;
            ThreadPool.GetMinThreads(out minWorkerThreads, out minCompletionPortThreads);

            // Calculate new I/O thread count based on a scaling factor
            int newIoThreads = (int)(minCompletionPortThreads * scaleFactor);

            Console.WriteLine($"Default Min Completion Port Threads: {minCompletionPortThreads}");
            Console.WriteLine($"Scaled I/O Completion Port Threads: {newIoThreads}");

            // Adjust the I/O completion port threads based on the scaling factor
            ThreadPool.SetMinThreads(minWorkerThreads, newIoThreads);
        }

        // Simulate image processing: download, resize, and save
        public async Task SimulateImageProcessing(int taskCount)
        {
            var tasks = new Task[taskCount];
            for (int i = 0; i < taskCount; i++)
            {
                tasks[i] = Task.Run(async () =>
                {
                    // Step 1: Download image (I/O-bound)
                    HttpResponseMessage response = null;
                    byte[] imageData = null;
                    try
                    {
                        response = await _httpClient.GetAsync("https://via.placeholder.com/150");
                        imageData = await response.Content.ReadAsByteArrayAsync();
                    }
                    finally
                    {
                        response?.Dispose();
                    }

                    // Step 2: Simulate image resizing (CPU-bound)
                    using (var ms = new MemoryStream(imageData))
                    using (var img = Image.FromStream(ms))
                    using (var resizedImg = new Bitmap(img, new Size(100, 100))) // Resize image to 100x100
                    {
                        // Step 3: Save the resized image to disk (I/O-bound)
                        string fileName = $"image_{Guid.NewGuid()}.png";
                        resizedImg.Save(fileName);
                    }
                });
            }

            await Task.WhenAll(tasks);
        }

        // Dispose method to clean up resources
        public void Dispose()
        {
            _httpClient.Dispose();
        }
    }
}
