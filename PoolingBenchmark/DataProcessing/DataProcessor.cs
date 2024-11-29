namespace PoolingBenchmark.DataProcessing
{
    public class DataProcessor
    {
        public void ProcessData(int[] buffer)
        {
            // Simulate some processing work with the buffer
            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = i * 2;  // Dummy processing logic
            }
        }
    }
}
