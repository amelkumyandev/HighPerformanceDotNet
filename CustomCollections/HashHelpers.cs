namespace CustomCollections
{
    public static class HashHelpers
    {
        // A list of prime numbers to use as hash table sizes. The idea is to avoid hash collisions
        // and reallocate the hash table to prime-sized arrays.
        private static readonly int[] primes =
        [
        3, 7, 11, 17, 23, 29, 37, 47, 59, 71, 89, 107, 131, 163, 197, 239,
        293, 353, 431, 521, 631, 761, 919, 1103, 1327, 1597, 1931, 2333, 2801,
        3371, 4049, 4861, 5839, 7013, 8419, 10103, 12143, 14591, 17519, 21023,
        25229, 30293, 36353, 43627, 52361, 62851, 75521, 90613, 108631, 130363,
        156437, 187751, 225307, 270371, 324449, 389357, 467237, 560689, 672827,
        807403, 968897, 1162687, 1395263, 1674319, 2009191, 2411033, 2893249,
        3471899, 4166287, 4999559, 5999471, 7199369
    ];

        // Returns the next prime number that is larger than the given minimum value
        public static int GetPrime(int min)
        {
            if (min < 0)
            {
                throw new ArgumentException("Capacity overflow.");
            }

            foreach (int prime in primes)
            {
                if (prime >= min)
                {
                    return prime;
                }
            }

            // If no prime is found (which is highly unlikely due to the large array of primes),
            // fall back to this function which generates primes dynamically.
            return GenerateNextPrime(min);
        }

        // Dynamically generate the next prime number greater than min.
        private static int GenerateNextPrime(int min)
        {
            for (int i = (min | 1); i < int.MaxValue; i += 2)
            {
                if (IsPrime(i))
                {
                    return i;
                }
            }

            return min;
        }

        // Check if a number is prime.
        private static bool IsPrime(int candidate)
        {
            if ((candidate & 1) == 0)
            {
                return candidate == 2;
            }

            int limit = (int)Math.Sqrt(candidate);
            for (int divisor = 3; divisor <= limit; divisor += 2)
            {
                if (candidate % divisor == 0)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
