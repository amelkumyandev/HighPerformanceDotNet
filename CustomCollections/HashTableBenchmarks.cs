using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;

namespace CustomCollections
{
    [MemoryDiagnoser]
    public class HashTableBenchmarks
    {
        private Dictionary<int, int> dictionary;
        private CustomHashTable<int, int> customHashTable;
        private int[] randomKeys;
        private const int NumElements = 100000;
        private Random rand;

        [GlobalSetup]
        public void Setup()
        {
            dictionary = new Dictionary<int, int>(NumElements);
            customHashTable = new CustomHashTable<int, int>();
            rand = new Random();

            randomKeys = new int[NumElements];

            // Insert elements into both structures
            for (int i = 0; i < NumElements; i++)
            {
                dictionary.Add(i, i);
                customHashTable.Add(i, i);
                randomKeys[i] = i;  // Use sequential keys for lookup to ensure they're present
            }
        }

        [Benchmark]
        public void Dictionary_Lookup()
        {
            // Lookup a random key from the dictionary
            var randomKey = randomKeys[rand.Next(0, NumElements)];
            var value = dictionary[randomKey];
        }

        [Benchmark]
        public void CustomHashTable_Lookup()
        {
            // Lookup a random key from the custom hash table
            var randomKey = randomKeys[rand.Next(0, NumElements)];
            if (customHashTable.TryGetValue(randomKey, out var value))
            {
                // Key found
            }
            else
            {
                // Handle the case where the key is not found (this should not happen with sequential keys)
            }
        }

        // Cleanup after each iteration (optional if memory concerns arise)
        [IterationCleanup]
        public void CleanupAfterIteration()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }
    }
}
