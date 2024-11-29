namespace IoBoundOptimized
{
    public class FileDownloader
    {
        public static async Task DownloadFilesOptimized(List<string> urls, int maxConcurrency)
        {
            using var httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromMinutes(10)
            };

            using var semaphore = new SemaphoreSlim(maxConcurrency);
            var tasks = new List<Task>();

            foreach (var url in urls)
            {
                await semaphore.WaitAsync();

                tasks.Add(Task.Run(async () =>
                {
                    try
                    {
                     //   Console.WriteLine($"Downloading {url}");
                        using var response = await httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
                        response.EnsureSuccessStatusCode();

                        using var stream = await response.Content.ReadAsStreamAsync();
                        await ProcessDataAsync(stream);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error downloading {url}: {ex.Message}");
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                }));
            }

            await Task.WhenAll(tasks);
        }

        public static async Task ProcessDataAsync(Stream stream)
        {
            byte[] buffer = new byte[8192];
            int bytesRead;
            int totalBytesRead = 0;
            int checksum = 0;

            while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
            {
                totalBytesRead += bytesRead;

                // Process the buffer
                for (int i = 0; i < bytesRead; i++)
                {
                    checksum = (checksum + buffer[i]) % 256;
                }
            }

          //  Console.WriteLine($"Processed {totalBytesRead} bytes with checksum: {checksum}");
        }

        public static async Task DownloadFilesSequentially(List<string> urls)
        {
            using var httpClient = new HttpClient();
            foreach (var url in urls)
            {
           //     Console.WriteLine($"Downloading {url}");
                var data = await httpClient.GetByteArrayAsync(url);
                ProcessData(data);
            }
        }

        public static void ProcessData(byte[] data)
        {
            // Simulate data processing by calculating checksum
            int checksum = 0;
            foreach (var b in data)
            {
                checksum = (checksum + b) % 256;
            }

         //   Console.WriteLine($"Processed data with checksum: {checksum}");
        }
    }
}
