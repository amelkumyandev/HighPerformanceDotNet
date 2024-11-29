using System.Buffers;
using System.Runtime.CompilerServices;

namespace RealTimeDataProcessing
{
    public class DataPoint
    {
        public DateTime Timestamp { get; set; }
        public double Value { get; set; }
    }

    class Program
    {
        static async Task Main(string[] args)
        {
            using var cts = new CancellationTokenSource();

            // Cancel the operation after 10 seconds
            cts.CancelAfter(TimeSpan.FromSeconds(10));

            try
            {
                var dataStream = GenerateDataAsync(cts.Token);
                await ProcessDataInBatchesAsync(dataStream, cts.Token);
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Data processing was canceled.");
            }
        }

        public static async IAsyncEnumerable<DataPoint> GenerateDataAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var random = new Random();

            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(10, cancellationToken); // Simulate data generation interval

                var dataPoint = new DataPoint
                {
                    Timestamp = DateTime.UtcNow,
                    Value = random.NextDouble()
                };

                yield return dataPoint;
            }
        }

        public static async Task ProcessDataInBatchesAsync(IAsyncEnumerable<DataPoint> dataStream, CancellationToken cancellationToken = default)
        {
            const int batchSize = 100;
            var pool = ArrayPool<DataPoint>.Shared;
            var batch = pool.Rent(batchSize);
            int index = 0;

            try
            {
                await foreach (var dataPoint in dataStream.WithCancellation(cancellationToken))
                {
                    batch[index++] = dataPoint;

                    if (index == batchSize)
                    {
                        ProcessBatch(batch, index);
                        index = 0;
                    }
                }

                if (index > 0)
                {
                    ProcessBatch(batch, index);
                }
            }
            finally
            {
                pool.Return(batch, clearArray: true);
            }
        }

        static void ProcessBatch(DataPoint[] batch, int count)
        {
            // Efficiently process the batch
            double sum = 0;
            double max = double.MinValue;
            double min = double.MaxValue;

            for (int i = 0; i < count; i++)
            {
                var value = batch[i].Value;
                sum += value;
                if (value > max) max = value;
                if (value < min) min = value;
            }
            double average = sum / count;

            Console.WriteLine($"Processed batch of {count} items. Avg: {average:F4}, Max: {max:F4}, Min: {min:F4}");
        }
    }
}
