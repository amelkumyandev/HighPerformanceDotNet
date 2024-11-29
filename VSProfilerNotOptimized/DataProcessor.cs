using System.Text;

namespace VSProfilerNotOptimized
{
    public class DataProcessor
    {
        public void ProcessData()
        {
            for (int i = 0; i < 1000000; i++)
            {
                var data = GenerateData(i);
                var result = ProcessItem(data);
            }
        }

        private string GenerateData(int id)
        {
            StringBuilder sb = new StringBuilder(id % 1000);
            sb.Append('A', id % 1000);
            return sb.ToString();
        }

        private string ProcessItem(string data)
        {
            StringBuilder sb = new StringBuilder(data.Length);
            foreach (var ch in data)
            {
                sb.Append(char.ToUpper(ch));
            }
            return sb.ToString();
        }
    }

}
