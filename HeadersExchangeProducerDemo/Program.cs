using System.Text;
using RabbitMQ.Client;

namespace Producer
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var factory = new ConnectionFactory { HostName = "127.0.0.1" };
            using var connection = await factory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();

            // Declare the headers exchange
            await channel.ExchangeDeclareAsync(
                exchange: "headers_exchange",
                type: ExchangeType.Headers);

            var messages = new List<(string, IDictionary<string, object>)>
            {
                ("Message with format=pdf and type=report", new Dictionary<string, object> { { "format", "pdf" }, { "type", "report" } }),
                ("Message with format=doc and type=report", new Dictionary<string, object> { { "format", "doc" }, { "type", "report" } }),
                ("Message with format=pdf and type=log", new Dictionary<string, object> { { "format", "pdf" }, { "type", "log" } })
            };

            foreach (var (message, headers) in messages)
            {
                var body = Encoding.UTF8.GetBytes(message);

                var properties = new BasicProperties();
                properties.Headers = headers;

                await channel.BasicPublishAsync(
                    exchange: "headers_exchange",
                    routingKey: string.Empty, // Routing key is ignored for headers exchange
                    true,
                    basicProperties: properties,
                    body: body);

                Console.WriteLine(" [x] Sent {0}", message);
            }

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }
    }
}
