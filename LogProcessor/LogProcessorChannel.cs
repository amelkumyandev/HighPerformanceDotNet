using System.Threading.Channels;

namespace LogProcessor
{
    public class LogProcessorChannel
    {
        public Channel<string> _logChannel;

        public LogProcessorChannel(int capacity)
        {
            // Bounded channel to apply backpressure
            _logChannel = Channel.CreateBounded<string>(capacity);
        }

        public async Task ProduceLogAsync(string logMessage)
        {
            await _logChannel.Writer.WriteAsync(logMessage);
            Console.WriteLine($"Produced log: {logMessage}");
        }

        public async Task ConsumeLogsAsync()
        {
            await foreach (var log in _logChannel.Reader.ReadAllAsync())
            {
                Console.WriteLine($"Consumed log: {log}");
                // Simulate writing to database (I/O-bound task)
                await Task.Delay(100); // Simulating DB write
            }
        }
    }
}
