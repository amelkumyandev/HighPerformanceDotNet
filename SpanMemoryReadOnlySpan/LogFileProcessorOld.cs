using System.Text;

namespace SpanMemoryReadOnlySpan
{
    public class LogFileProcessorOld
    {
        private const int BufferSize = 8192; // 8 KB buffer for reading the file

        public async Task ProcessLogFileAsync(string filePath)
        {
            byte[] buffer = new byte[BufferSize];

            using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite))
            {
                while (true)
                {
                    int bytesRead = await fileStream.ReadAsync(buffer, 0, buffer.Length);
                    if (bytesRead == 0) break;  // End of file

                    // Parse the buffer using string allocations
                    string content = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    string timestamp = content.Substring(0, 10);
                    Console.WriteLine($"Timestamp (Old): {timestamp}");

                    // Modify the buffer (allocates new array)
                    byte[] modifiedBuffer = ModifyBufferOld(buffer, bytesRead);
                    await fileStream.WriteAsync(modifiedBuffer, 0, modifiedBuffer.Length);
                }
            }
        }

        private byte[] ModifyBufferOld(byte[] buffer, int length)
        {
            byte[] newBuffer = new byte[length];
            for (int i = 0; i < length; i++)
            {
                newBuffer[i] = (byte)(buffer[i] ^ 0xFF); // Bitwise XOR with 0xFF (flip bits)
            }
            return newBuffer;
        }
    }
}
