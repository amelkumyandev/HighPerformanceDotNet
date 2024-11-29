using BenchmarkDotNet.Running;
using SpanMemoryReadOnlySpan;
public class Program
{
    public static async Task Main(string[] args)
    {
        // Ensure the large log file is created before running benchmarks
        if (!File.Exists("large-log-file.txt"))
        {
            await InitializeLogFileAsync("large-log-file.txt", 100000);
        }

            BenchmarkRunner.Run<LogFileBenchmark>();

        // Run the benchmarks using BenchmarkDotNet
        var summary = BenchmarkRunner.Run<LogFileBenchmark>();

        // Wait for the benchmark summary to complete before finishing the program
        Console.ReadLine();
    }

    private static async Task InitializeLogFileAsync(string filePath, int lineCount)
    {
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            for (int i = 0; i < lineCount; i++)
            {
                await writer.WriteLineAsync($"2024-09-20 12:34:56 Log entry number {i}");
            }
        }
    }
}
