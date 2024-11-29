using System.Net;
using System.Net.Sockets;
using System.Buffers;

namespace FileTransferServer
{
    class Program
    {
        private const int Port = 5000;

        static async Task Main(string[] args)
        {
            string filePath = "largefile.txt"; // Replace with your file path
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"File {filePath} not found.");
                return;
            }

            var listener = new TcpListener(IPAddress.Any, Port);
            listener.Start();
            Console.WriteLine($"Server listening on port {Port}");

            while (true)
            {
                var client = await listener.AcceptTcpClientAsync();
                Console.WriteLine("Client connected.");
                _ = Task.Run(() => HandleClientAsync(client, filePath));
            }
        }

        private static async Task HandleClientAsync(TcpClient client, string filePath)
        {
            try
            {
                using var networkStream = client.GetStream();
                await foreach (var chunk in ReadFileChunksAsync(filePath, 8192))
                {
                    await networkStream.WriteAsync(chunk);
                }
                Console.WriteLine("File sent successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            finally
            {
                client.Close();
            }
        }

        private static async IAsyncEnumerable<Memory<byte>> ReadFileChunksAsync(string filePath, int chunkSize)
        {
            using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: chunkSize, useAsync: true);
            var bufferPool = ArrayPool<byte>.Shared;
            byte[] buffer = bufferPool.Rent(chunkSize);

            try
            {
                int bytesRead;
                while ((bytesRead = await fileStream.ReadAsync(buffer, 0, chunkSize)) > 0)
                {
                    yield return new Memory<byte>(buffer, 0, bytesRead);
                }
            }
            finally
            {
                bufferPool.Return(buffer);
            }
        }
    }
}
