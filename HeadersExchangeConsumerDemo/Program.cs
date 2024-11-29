using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Consumer
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var factory = new ConnectionFactory { HostName = "127.0.0.1" };
            using var connection = await factory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();

            var bindingHeaders = new Dictionary<string, object>
              {
                  { "x-match", "all" },
                  { "format", "pdf" },
                  { "type", "report" }
              };

            // Declare the headers exchange
            await channel.ExchangeDeclareAsync(
              exchange: "headers_exchange",
              type: ExchangeType.Headers);

            // Declare a queue
            var queueName = "headers_queue";
            await channel.QueueDeclareAsync(
              queue: queueName,
              durable: false,
              exclusive: false,
              autoDelete: false,
              arguments: bindingHeaders
              );

            // Bind the queue to the exchange with headers
            await channel.QueueBindAsync(
              queue: queueName,
              exchange: "headers_exchange",
              routingKey: string.Empty, // Routing key is ignored
              arguments: null);

            Console.WriteLine(" [*] Waiting for messages.");

            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.ReceivedAsync += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine($" [x] Received {message}");
                await Task.Yield();
            };

            await channel.BasicConsumeAsync(
              queue: queueName,
              autoAck: true,
              consumer: consumer);

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }
    }
}