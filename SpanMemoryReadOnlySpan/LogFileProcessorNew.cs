using System.Text;

namespace SpanMemoryReadOnlySpan
{
    public class LogFileProcessorNew
    {
        private const int BufferSize = 8192; // 8 KB buffer for reading the file

        public async Task ProcessLogFileAsync(string filePath)
        {
            Memory<byte> buffer = new byte[BufferSize];

            await using FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite);

            while (true)
            {
                int bytesRead = await fileStream.ReadAsync(buffer);
                if (bytesRead == 0) break;  // End of file

                // Pass the buffer to a non-async method that works with ReadOnlySpan<byte>
                ProcessBuffer(buffer.Slice(0, bytesRead).Span);

                // Writing the modified data back to the file
                await fileStream.WriteAsync(buffer.Slice(0, bytesRead));
            }
        }

        // Non-async method to process the buffer using ReadOnlySpan<byte>
        private void ProcessBuffer(Span<byte> buffer)
        {
            // Parse the buffer using ReadOnlySpan<byte>
            ReadOnlySpan<byte> readOnlySpan = buffer;
            string timestamp = Encoding.UTF8.GetString(readOnlySpan.Slice(0, 10));
            Console.WriteLine($"Timestamp (New): {timestamp}");

            // Modify the buffer in-place using Span<byte>
            ModifyBuffer(buffer);
        }

        private void ModifyBuffer(Span<byte> buffer)
        {
            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] ^= 0xFF; // Bitwise XOR with 0xFF (flip bits)
            }
        }
    }
}
