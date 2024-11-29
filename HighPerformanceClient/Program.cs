using System.Net.Sockets;
using System.Text;

namespace HighPerformanceClient
{
    class Program
    {
        private static bool isRunning = true;
        private static readonly SemaphoreSlim sendSemaphore = new SemaphoreSlim(1, 1);
        private static readonly int retryDelayMs = 2000;

        private static TcpClient client = new TcpClient();
        private static NetworkStream stream;
        private static bool isConnected = false;

        static async Task Main(string[] args)
        {
            Console.WriteLine("Client started. Type 'DISCONNECT' to quit.");

            while (isRunning)
            {
                // Read user input
                string message = "Test Messages";

                // Handle "DISCONNECT" command to stop reconnections
                if (message.Equals("DISCONNECT", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("[Client] Disconnect command received. Closing client permanently.");
                    isRunning = false;
                    break;
                }

                // Attempt to send message, reconnecting as needed
                await SendMessageWithReconnectAsync(message);
            }

            Console.WriteLine("Press ENTER to exit.");
            Console.ReadLine();
        }

        private static async Task SendMessageWithReconnectAsync(string message)
        {
            while (isRunning)
            {
                try
                {
                    if (!isConnected || !client.Connected)
                    {
                        client = new TcpClient();
                        await client.ConnectAsync("localhost", 11000);
                        stream = client.GetStream();
                        isConnected = true;
                        Console.WriteLine("[Client] Connected to server.");
                    }

                    await sendSemaphore.WaitAsync();
                    try
                    {
                        byte[] data = Encoding.UTF8.GetBytes(message);
                        await stream.WriteAsync(data, 0, data.Length);
                        Console.WriteLine($"[Client] Sent: {message}");

                        // Optional: Wait for response
                        await ReceiveResponseAsync(stream);
                    }
                    finally
                    {
                        sendSemaphore.Release();
                    }

                    break; // Exit loop on successful send
                }
                catch (SocketException ex)
                {
                    Console.WriteLine($"[Client] Socket error: {ex.Message}. Retrying in {retryDelayMs} ms...");
                    isConnected = false;
                    await Task.Delay(retryDelayMs);
                }
                catch (IOException ex)
                {
                    Console.WriteLine($"[Client] IO Exception: {ex.Message}. Retrying in {retryDelayMs} ms...");
                    isConnected = false;
                    await Task.Delay(retryDelayMs);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Client] Exception: {ex.Message}. Retrying in {retryDelayMs} ms...");
                    isConnected = false;
                    await Task.Delay(retryDelayMs);
                }
            }
        }

        private static async Task ReceiveResponseAsync(NetworkStream stream)
        {
            byte[] buffer = new byte[1024];

            try
            {
                int byteCount = await stream.ReadAsync(buffer, 0, buffer.Length);
                if (byteCount > 0)
                {
                    string receivedText = Encoding.UTF8.GetString(buffer, 0, byteCount);
                    Console.WriteLine($"[Server]: {receivedText}");
                }
                else
                {
                    Console.WriteLine("[Client] No response from server.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Client] Receive error: {ex.Message}");
            }
        }
    }
}
