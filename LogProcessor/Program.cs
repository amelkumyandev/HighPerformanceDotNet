
using LogProcessor;

var processor = new LogProcessorChannel(capacity: 5);

// Start consumers
var consumers = Task.Run(() => processor.ConsumeLogsAsync());

// Simulate log production
for (int i = 1; i <= 10; i++)
{
    await processor.ProduceLogAsync($"Log entry {i}");
}

// Mark the channel as completed
processor._logChannel.Writer.Complete();

// Wait for consumers to finish
await consumers;