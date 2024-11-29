using System.Buffers;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;

namespace FileTransferClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Usage: dotnet run <filePath>");
                return;
            }

            var client = new FileTransferClient("127.0.0.1", 5000);
            await client.SendFileAsync(args[0]);
        }
    }

    public class FileTransferClient
    {
        private const int BufferSize = 81920;
        private readonly string serverAddress;
        private readonly int serverPort;
        private long offset = 0;
        private bool isPaused = false;

        public FileTransferClient(string address, int port)
        {
            serverAddress = address;
            serverPort = port;
        }

        public async Task SendFileAsync(string filePath)
        {
            var fileName = Path.GetFileName(filePath);
            var fileSize = new FileInfo(filePath).Length;

            using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, BufferSize, useAsync: true);

            // Compute file hash
            string fileHash = ComputeHash(fileStream);
            fileStream.Seek(0, SeekOrigin.Begin);

            var metadata = new FileMetadata
            {
                FileName = fileName,
                FileSize = fileSize,
                FileHash = fileHash
            };

            var metadataJson = System.Text.Json.JsonSerializer.Serialize(metadata);
            var metadataBytes = Encoding.UTF8.GetBytes(metadataJson);
            var metadataLengthBytes = BitConverter.GetBytes(metadataBytes.Length);

            using var client = new TcpClient();
            await client.ConnectAsync(serverAddress, serverPort);

            using var networkStream = client.GetStream();

            // Send metadata length and metadata
            await networkStream.WriteAsync(metadataLengthBytes, 0, metadataLengthBytes.Length);
            await networkStream.WriteAsync(metadataBytes, 0, metadataBytes.Length);

            // Receive offset from server
            var offsetBuffer = new byte[8];
            await networkStream.ReadAsync(offsetBuffer, 0, offsetBuffer.Length);
            offset = BitConverter.ToInt64(offsetBuffer, 0);

            // Seek to the offset
            fileStream.Seek(offset, SeekOrigin.Begin);

            // Start sending data
            var bufferPool = ArrayPool<byte>.Shared;
            var buffer = bufferPool.Rent(BufferSize);

            try
            {
                int bytesRead;
                Console.WriteLine("Press 'P' to pause/resume the transfer.");
                var inputTask = Task.Run(() => MonitorInput());

                while ((bytesRead = await fileStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                {
                    if (isPaused)
                    {
                        Console.WriteLine("Transfer paused. Press 'P' to resume.");
                        await Task.Run(() => WaitForResume());
                    }

                    await networkStream.WriteAsync(buffer, 0, bytesRead);
                    offset += bytesRead;
                }

                await inputTask; // Ensure the input task completes
            }
            finally
            {
                bufferPool.Return(buffer);
            }

            // Receive acknowledgment
            var ackBuffer = new byte[1024];
            int ackBytes = await networkStream.ReadAsync(ackBuffer, 0, ackBuffer.Length);
            var ack = Encoding.UTF8.GetString(ackBuffer, 0, ackBytes);

            if (ack == "SUCCESS")
            {
                Console.WriteLine($"File '{fileName}' sent and verified successfully.");
            }
            else
            {
                Console.WriteLine($"File '{fileName}' transfer failed.");
            }

            client.Close();
        }

        private void MonitorInput()
        {
            while (true)
            {
                var key = Console.ReadKey(intercept: true);
                if (key.Key == ConsoleKey.P)
                {
                    isPaused = !isPaused;
                    if (!isPaused)
                    {
                        lock (this)
                        {
                            Monitor.Pulse(this);
                        }
                    }
                }
            }
        }

        private void WaitForResume()
        {
            lock (this)
            {
                while (isPaused)
                {
                    Monitor.Wait(this);
                }
            }
        }

        private string ComputeHash(Stream stream)
        {
            stream.Seek(0, SeekOrigin.Begin);
            using var sha256 = SHA256.Create();
            byte[] hash = sha256.ComputeHash(stream);
            return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        }

        private class FileMetadata
        {
            public string FileName { get; set; }
            public long FileSize { get; set; }
            public string FileHash { get; set; }
        }
    }
}
