using RabbitMQ.Client;
using System.Text;

namespace Producer
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var factory = new ConnectionFactory { HostName = "127.0.0.1" };
            using var connection = await factory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();


            // Declare a queue to send messages to
           await  channel.QueueDeclareAsync(
                                         queue: "direct_queue",
                                         durable: false,
                                         exclusive: false,
                                         autoDelete: false,
                                         arguments: null);

           List<string> messages = new List<string>{"Hello World1!", "Hello World2!", "Hello World13" };
            foreach (var message in messages)
            {
                var body = Encoding.UTF8.GetBytes(message);

                await channel.BasicPublishAsync(exchange: string.Empty, routingKey: "direct_queue", body: body);

                Console.WriteLine(" [x] Sent {0}", message);
            }
          
            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }
    }
}
