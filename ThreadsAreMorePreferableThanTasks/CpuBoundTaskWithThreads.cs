namespace ThreadsAreMorePreferableThanTasks
{
    public class CpuBoundTaskWithThreads
    {
        private List<Thread> _threads = new List<Thread>();

        public void StartTasks(int numberOfThreads)
        {
            for (int i = 0; i < numberOfThreads; i++)
            {
                var thread = new Thread(() => ComputePrimes(1_000_000));
            }

            foreach (var thread in _threads)
            {
                thread.Join();
            }
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
