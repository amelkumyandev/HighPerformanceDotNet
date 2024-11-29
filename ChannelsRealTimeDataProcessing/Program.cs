using ChannelsRealTimeDataProcessing;
using System.Threading.Channels;

class Program
{
    static async Task Main(string[] args)
    {
        // Set up cancellation support
        var cts = new CancellationTokenSource();
        Console.CancelKeyPress += (sender, eventArgs) =>
        {
            eventArgs.Cancel = true;
            Console.WriteLine("Cancellation requested...");
            cts.Cancel();
        };

        // Define channel options
        var producerChannelOptions = new BoundedChannelOptions(100)
        {
            FullMode = BoundedChannelFullMode.Wait
        };
        var stage1ChannelOptions = new BoundedChannelOptions(100)
        {
            FullMode = BoundedChannelFullMode.Wait
        };
        var stage2ChannelOptions = new BoundedChannelOptions(100)
        {
            FullMode = BoundedChannelFullMode.Wait
        };

        // Create channels for each stage
        var producerChannel = Channel.CreateBounded<DataItem>(producerChannelOptions);
        var stage1Channel = Channel.CreateBounded<DataItem>(stage1ChannelOptions);
        var stage2Channel = Channel.CreateBounded<DataItem>(stage2ChannelOptions);

        // Start Producers
        int numberOfProducers = 3;
        int itemsPerProducer = 50;
        var producerTasks = new List<Task>();
        for (int i = 0; i < numberOfProducers; i++)
        {
            int producerId = i + 1;
            producerTasks.Add(ProduceDataAsync(producerChannel.Writer, producerId, itemsPerProducer, cts.Token));
        }

        // Start Stage 1 Processing with multiple consumers
        int stage1Processors = 2;
        var stage1Tasks = new List<Task>();
        for (int i = 0; i < stage1Processors; i++)
        {
            stage1Tasks.Add(ProcessStage1Async(producerChannel.Reader, stage1Channel.Writer, cts.Token));
        }

        // Start Stage 2 Processing with multiple consumers
        int stage2Processors = 2;
        var stage2Tasks = new List<Task>();
        for (int i = 0; i < stage2Processors; i++)
        {
            stage2Tasks.Add(ProcessStage2Async(stage1Channel.Reader, stage2Channel.Writer, cts.Token));
        }

        // Start Consumer
        var consumerTask = ConsumeDataAsync(stage2Channel.Reader, cts.Token);

        // Wait for all tasks to complete
        await Task.WhenAll(producerTasks);
        await Task.WhenAll(stage1Tasks);
        await Task.WhenAll(stage2Tasks);

        // Signal completion to the consumer
        stage2Channel.Writer.Complete();
        await consumerTask;

        Console.WriteLine("Processing complete.");
    }

    // Producer Method
    public static async Task ProduceDataAsync(ChannelWriter<DataItem> writer, int producerId, int itemsToProduce, CancellationToken cancellationToken)
    {
        try
        {
            for (int i = 0; i < itemsToProduce; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var dataItem = new DataItem
                {
                    Id = i + (producerId - 1) * itemsToProduce,
                    RawData = $"Producer {producerId} - Data {i}"
                };

                await writer.WriteAsync(dataItem, cancellationToken);
                Console.WriteLine($"Producer {producerId} produced item {dataItem.Id}");

                // Simulate variable production rates
                await Task.Delay(TimeSpan.FromMilliseconds(new Random().Next(50, 150)), cancellationToken);
            }
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine($"Producer {producerId} was canceled.");
        }
        finally
        {
            writer.Complete();
        }
    }

    // Stage 1 Processing Method
    public static async Task ProcessStage1Async(ChannelReader<DataItem> inputReader, ChannelWriter<DataItem> outputWriter, CancellationToken cancellationToken)
    {
        try
        {
            await foreach (var item in inputReader.ReadAllAsync(cancellationToken))
            {
                cancellationToken.ThrowIfCancellationRequested();

                // Simulate processing
                item.ProcessedDataStage1 = item.RawData.ToUpperInvariant();
                Console.WriteLine($"Stage 1 processed item {item.Id}");

                await outputWriter.WriteAsync(item, cancellationToken);

                // Simulate processing time
                await Task.Delay(TimeSpan.FromMilliseconds(new Random().Next(50, 150)), cancellationToken);
            }
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Stage 1 processing was canceled.");
        }
        finally
        {
            outputWriter.Complete();
        }
    }

    // Stage 2 Processing Method
    public static async Task ProcessStage2Async(ChannelReader<DataItem> inputReader, ChannelWriter<DataItem> outputWriter, CancellationToken cancellationToken)
    {
        try
        {
            await foreach (var item in inputReader.ReadAllAsync(cancellationToken))
            {
                cancellationToken.ThrowIfCancellationRequested();

                // Simulate processing
                item.ProcessedDataStage2 = $"{item.ProcessedDataStage1} - Processed at {DateTime.UtcNow:O}";
                Console.WriteLine($"Stage 2 processed item {item.Id}");

                await outputWriter.WriteAsync(item, cancellationToken);

                // Simulate processing time
                await Task.Delay(TimeSpan.FromMilliseconds(new Random().Next(50, 150)), cancellationToken);
            }
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Stage 2 processing was canceled.");
        }
        finally
        {
            outputWriter.Complete();
        }
    }

    // Consumer Method
    public static async Task ConsumeDataAsync(ChannelReader<DataItem> reader, CancellationToken cancellationToken)
    {
        try
        {
            await foreach (var item in reader.ReadAllAsync(cancellationToken))
            {
                cancellationToken.ThrowIfCancellationRequested();

                // Output the final data
                Console.WriteLine($"Consumer received item {item.Id}: {item.ProcessedDataStage2}");

                // Simulate consumption time
                await Task.Delay(TimeSpan.FromMilliseconds(new Random().Next(50, 150)), cancellationToken);
            }
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Consumer was canceled.");
        }
    }
}