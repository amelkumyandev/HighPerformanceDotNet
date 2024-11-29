namespace ThreadsAreMorePreferableThanTasks
{
    public class CpuBoundTaskWithTasks
    {
        public async Task StartTasksAsync(int numberOfTasks)
        {
            var tasks = new List<Task>();
            for (int i = 0; i < numberOfTasks; i++)
            {
                tasks.Add(Task.Run(() => ComputePrimes(1_000_000)));
            }

            await Task.WhenAll(tasks);  // Ensure all tasks complete
        }

        private void ComputePrimes(int upperLimit)
        {
            List<int> primes = new List<int>();
            for (int i = 2; i <= upperLimit; i++)
            {
                bool isPrime = true;
                for (int j = 2; j * j <= i; j++)
                {
                    if (i % j == 0)
                    {
                        isPrime = false;
                        break;
                    }
                }
                if (isPrime)
                {
                    primes.Add(i);
                }
            }
        }
    }

}
