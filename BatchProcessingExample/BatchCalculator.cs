namespace BatchProcessingExample
{
    public class BatchCalculator
    {
        private readonly string csvFilePath = Path.Combine(Directory.GetCurrentDirectory(), "results.csv");
        private readonly object fileLock = new object();

        public BatchCalculator()
        {
            // Initialize CSV file
            if (!File.Exists(csvFilePath))
            {
                File.WriteAllText(csvFilePath, "Result\n");
            }
        }

        public void ProcessDataSequentially(List<double> data)
        {
            foreach (var item in data)
            {
                double result = ComplexCalculation(item);
                StoreResult(result);
            }
        }

        public void ProcessDataInBatches(List<double> data, int batchSize)
        {
            for (int i = 0; i < data.Count; i += batchSize)
            {
                var batch = data.GetRange(i, Math.Min(batchSize, data.Count - i));
                var results = new List<double>(batch.Count);

                foreach (var item in batch)
                {
                    results.Add(ComplexCalculation(item));
                }

                StoreBatchResults(results);
            }
        }

        private double ComplexCalculation(double input)
        {
            // Simulate a complex calculation
            double result = 0;
            for (int i = 0; i < 1000; i++)
            {
                result += Math.Sqrt(input) * Math.Sin(i) / (Math.Cos(i + input) + 1);
            }
            return result;
        }

        private void StoreResult(double result)
        {
            // Write the result to CSV
            lock (fileLock)
            {
                File.AppendAllText(csvFilePath, $"{result}\n");
            }
        }

        private void StoreBatchResults(List<double> results)
        {
            // Write batch results to CSV
            lock (fileLock)
            {
                using (var writer = new StreamWriter(csvFilePath, append: true))
                {
                    foreach (var result in results)
                    {
                        writer.WriteLine(result);
                    }
                }
            }
        }
    }
}
