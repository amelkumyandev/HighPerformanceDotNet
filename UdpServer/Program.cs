using System.Net.Sockets;
using System.Text;

namespace UdpServer
{
    class Program
    {
        private const int ListenPort = 11000;

        static async Task Main(string[] args)
        {
            using var udpClient = new UdpClient(ListenPort);

            Console.WriteLine($"Server is listening on port {ListenPort}");

            while (true)
            {
                try
                {
                    var receivedResult = await udpClient.ReceiveAsync();
                    string receivedData = Encoding.UTF8.GetString(receivedResult.Buffer);

                    Console.WriteLine($"Received: {receivedData} from {receivedResult.RemoteEndPoint}");

                    // Prepare response
                    string responseData = $"Echo: {receivedData}";
                    byte[] responseBytes = Encoding.UTF8.GetBytes(responseData);

                    await udpClient.SendAsync(responseBytes, responseBytes.Length, receivedResult.RemoteEndPoint);

                    Console.WriteLine($"Sent: {responseData} to {receivedResult.RemoteEndPoint}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
        }
    }
}
