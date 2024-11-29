namespace ConfigureAwaitFalse
{
    public class FileWriter
    {
        public async Task WriteToFileAsync(string[] data, string filePath)
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                foreach (var line in data)
                {
                    await writer.WriteLineAsync(line);
                }
            }
        }
    }
}
