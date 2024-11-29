using System.Buffers;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Security.Cryptography;

namespace FileTransferServer
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var server = new FileTransferServer(port: 5000);
            await server.StartAsync();
        }
    }

    public class FileTransferServer
    {
        private const int BufferSize = 81920;
        private readonly TcpListener listener;
        private readonly ConcurrentDictionary<string, long> fileOffsets = new();

        public FileTransferServer(int port)
        {
            listener = new TcpListener(IPAddress.Any, port);
        }

        public async Task StartAsync()
        {
            listener.Start();
            Console.WriteLine("Server started. Listening for connections...");

            while (true)
            {
                var client = await listener.AcceptTcpClientAsync();
                _ = HandleClientAsync(client); // Fire-and-forget
            }
        }

        private async Task HandleClientAsync(TcpClient client)
        {
            Console.WriteLine("Client connected.");
            using var networkStream = client.GetStream();

            try
            {
                // Receive file metadata
                var metadataLengthBuffer = new byte[4];
                await networkStream.ReadAsync(metadataLengthBuffer, 0, 4);
                int metadataLength = BitConverter.ToInt32(metadataLengthBuffer, 0);

                var metadataBuffer = new byte[metadataLength];
                await networkStream.ReadAsync(metadataBuffer, 0, metadataLength);
                var metadataJson = Encoding.UTF8.GetString(metadataBuffer);
                var metadata = System.Text.Json.JsonSerializer.Deserialize<FileMetadata>(metadataJson);

                // Determine the starting offset
                long offset = 0;
                if (fileOffsets.ContainsKey(metadata.FileName))
                {
                    offset = fileOffsets[metadata.FileName];
                }
                else
                {
                    fileOffsets[metadata.FileName] = 0;
                }

                // Send the offset back to the client
                var offsetBytes = BitConverter.GetBytes(offset);
                await networkStream.WriteAsync(offsetBytes, 0, offsetBytes.Length);

                // Prepare to receive the file
                var filePath = Path.Combine("ReceivedFiles", metadata.FileName);
                Directory.CreateDirectory("ReceivedFiles");

                using var fileStream = new FileStream(
                    filePath,
                    FileMode.OpenOrCreate,
                    FileAccess.ReadWrite,
                    FileShare.None,
                    BufferSize,
                    useAsync: true);

                // Set the file stream to the correct position
                fileStream.Seek(offset, SeekOrigin.Begin);

                var bufferPool = ArrayPool<byte>.Shared;
                var buffer = bufferPool.Rent(BufferSize);

                try
                {
                    long totalBytesReceived = offset;
                    int bytesRead;
                    while (totalBytesReceived < metadata.FileSize &&
                           (bytesRead = await networkStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                    {
                        await fileStream.WriteAsync(buffer, 0, bytesRead);
                        totalBytesReceived += bytesRead;
                        fileOffsets[metadata.FileName] = totalBytesReceived;
                    }
                }
                finally
                {
                    bufferPool.Return(buffer);
                }

                // Remove the offset tracking once transfer is complete
                fileOffsets.TryRemove(metadata.FileName, out _);

                // Verify data integrity
                fileStream.Seek(0, SeekOrigin.Begin);
                string receivedFileHash = ComputeHash(fileStream);

                if (metadata.FileHash == receivedFileHash)
                {
                    Console.WriteLine($"File '{metadata.FileName}' received and verified successfully.");
                    // Send acknowledgment
                    byte[] ack = Encoding.UTF8.GetBytes("SUCCESS");
                    await networkStream.WriteAsync(ack, 0, ack.Length);
                }
                else
                {
                    Console.WriteLine($"File '{metadata.FileName}' verification failed.");
                    byte[] nack = Encoding.UTF8.GetBytes("FAILURE");
                    await networkStream.WriteAsync(nack, 0, nack.Length);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error handling client: {ex.Message}");
            }
            finally
            {
                client.Close();
                Console.WriteLine("Client disconnected.");
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
