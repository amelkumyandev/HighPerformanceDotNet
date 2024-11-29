using System.Net.Sockets;
using System.Buffers;

namespace FileTransferClient
{
    class Program
    {
        private const string ServerAddress = "127.0.0.1";
        private const int Port = 5000;

        static async Task Main(string[] args)
        {
            string saveFilePath = "receivedfile.txt"; // Destination file path

            try
            {
                using var client = new TcpClient();
                await client.ConnectAsync(ServerAddress, Port);
                Console.WriteLine("Connected to server.");

                using var networkStream = client.GetStream();
                await ReceiveFileAsync(networkStream, saveFilePath);
                Console.WriteLine("File received successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private static async Task ReceiveFileAsync(NetworkStream networkStream, string saveFilePath)
        {
            using var fileStream = new FileStream(saveFilePath, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize: 8192, useAsync: true);
            var bufferPool = ArrayPool<byte>.Shared;
            byte[] buffer = bufferPool.Rent(8192);

            try
            {
                int bytesRead;
                while ((bytesRead = await networkStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                {
                    await fileStream.WriteAsync(buffer, 0, bytesRead);
                }
            }
            finally
            {
                bufferPool.Return(buffer);
            }
        }
    }
}
