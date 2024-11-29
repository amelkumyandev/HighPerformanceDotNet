using System.Text;
using BenchmarkDotNet.Attributes;

namespace AsynchronousPatterns
{
    public class AsyncBenchmark
    {
        [Benchmark]
        public async Task AsyncCpuBoundPattern()
        {
            var cpuTask1 = GenerateLargeTextAsync();
            var cpuTask2 = CalculatePrimesAsync(10_000);

            // Perform both tasks in parallel
            await Task.WhenAll(cpuTask1, cpuTask2);
        }

        [Benchmark]
        public async Task SequentialCpuBoundPattern()
        {
            // Perform text generation and prime calculation sequentially
            await GenerateLargeTextAsync();
            await CalculatePrimesAsync(10_000);
        }

        [Benchmark]
        public void BlockingCpuBoundPattern()
        {
            // Block the current thread by using .Result and .Wait()
            var textResult = GenerateLargeTextAsync().Result;
            var primesResult = CalculatePrimesAsync(10_000).Result;
        }

        [Benchmark]
        public async Task ConfigureAwaitFalseCpuBoundPattern()
        {
            // Start both tasks (do not await here yet)
            var cpuTask1 = GenerateLargeTextAsync();
            var cpuTask2 = CalculatePrimesAsync(10_000);

            // Await both tasks with ConfigureAwait(false)
            var textResult = await cpuTask1.ConfigureAwait(false);
            var primesResult = await cpuTask2.ConfigureAwait(false);
        }

        private async Task<string> GenerateLargeTextAsync()
        {
            // Simulate CPU-bound text generation
            return await Task.Run(() =>
            {
                var sb = new StringBuilder();
                for (int i = 0; i < 100_000; i++)
                {
                    sb.Append("This is some large generated text. ");
                }
                return sb.ToString();
            });
        }

        private async Task<int> CalculatePrimesAsync(int max)
        {
            // Simulate a CPU-bound prime number calculation
            return await Task.Run(() =>
            {
                int primeCount = 0;
                for (int i = 2; i < max; i++)
                {
                    if (IsPrime(i)) primeCount++;
                }
                return primeCount;
            });
        }

        private bool IsPrime(int number)
        {
            for (int i = 2; i < number; i++)
            {
                if (number % i == 0) return false;
            }
            return true;
        }
    }
}
