using System.Net;
using System.Net.Sockets;
using System.Text;

namespace UdpClientApp
{
    class Program
    {
        private const int ServerPort = 11000;
        private const string ServerAddress = "127.0.0.1"; // Localhost

        static async Task Main(string[] args)
        {
            using var udpClient = new UdpClient();

            Console.WriteLine("Client started. Type messages to send to the server. Type 'exit' to quit.");

            while (true)
            {
                Console.Write("> ");
                string message = Console.ReadLine();

                if (message.Equals("exit", StringComparison.OrdinalIgnoreCase))
                {
                    break;
                }

                try
                {
                    byte[] sendBytes = Encoding.UTF8.GetBytes(message);
                    await udpClient.SendAsync(sendBytes, sendBytes.Length, ServerAddress, ServerPort);

                    Console.WriteLine($"Sent: {message}");

                    var serverEndpoint = new IPEndPoint(IPAddress.Any, 0);
                    var receivedResult = await udpClient.ReceiveAsync();
                    string receivedData = Encoding.UTF8.GetString(receivedResult.Buffer);

                    Console.WriteLine($"Received from server: {receivedData}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }

            Console.WriteLine("Client exited.");
        }
    }
}
