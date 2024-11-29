using System.Threading;
using Grpc.Core;
using Grpc.Net.Client;
using OrderProcessingServiceGRPC;

var token = "valid_token"; // Simulated token

var channel = GrpcChannel.ForAddress("http://localhost:5000");
var client = new OrderProcessing.OrderProcessingClient(channel);

// Set up metadata with token
var headers = new Metadata
{
    { "authorization", $"Bearer {token}" }
};

// Place an order
var orderRequest = new OrderRequest
{
    OrderId = 1,
    CustomerId = 123,
    Address = "123 Main St",
    Items = {
        new OrderItem { ProductId = 1, Quantity = 2 },
        new OrderItem { ProductId = 2, Quantity = 1 }
    }
};

var orderResponse = await client.PlaceOrderAsync(orderRequest, headers);
Console.WriteLine(orderResponse.Message);

// Track order status
using var cts = new CancellationTokenSource();
var statusCall = client.TrackOrderStatus(new TrackOrderRequest { OrderId = 1 }, headers, cancellationToken: cts.Token);

_ = Task.Run(async () =>
{
    try
    {
        await foreach (var statusUpdate in statusCall.ResponseStream.ReadAllAsync(cts.Token))
        {
            Console.WriteLine($"Order Status: {statusUpdate.Status} at {statusUpdate.UpdatedAt}");
        }
    }
    catch (RpcException ex) when (ex.StatusCode == StatusCode.Cancelled)
    {
        Console.WriteLine("Cancelled order tracking.");
    }
});

// Simulate status updates by calling the server's UpdateOrderStatus method
await Task.Delay(2000);
await client.UpdateOrderStatusAsync(new UpdateOrderStatusRequest { OrderId = 1, Status = "Processing" }, headers);
await Task.Delay(2000);
await client.UpdateOrderStatusAsync(new UpdateOrderStatusRequest { OrderId = 1, Status = "Shipped" }, headers);
await Task.Delay(2000);
await client.UpdateOrderStatusAsync(new UpdateOrderStatusRequest { OrderId = 1, Status = "Delivered" }, headers);
await Task.Delay(2000);
cts.Cancel();

// Bulk upload orders
using var bulkCall = client.UploadBulkOrders(headers);
for (int i = 2; i <= 5; i++)
{
    await bulkCall.RequestStream.WriteAsync(new OrderRequest
    {
        OrderId = i,
        CustomerId = 123,
        Address = "123 Main St",
        Items = { new OrderItem { ProductId = i, Quantity = i } }
    });
}
await bulkCall.RequestStream.CompleteAsync();

var bulkResponse = await bulkCall;
Console.WriteLine($"Total Orders Processed: {bulkResponse.TotalOrdersProcessed}");

// Customer support chat
var chatHeaders = new Metadata
{
    { "authorization", $"Bearer {token}" },
    { "chatroom-id", "123" }
};

using var chatCall = client.CustomerSupportChat(chatHeaders);

var readTask = Task.Run(async () =>
{
    await foreach (var chatMessage in chatCall.ResponseStream.ReadAllAsync())
    {
        Console.WriteLine($"[{chatMessage.SentAt}] Support: {chatMessage.Message}");
    }
});

// Simulate sending messages to the chat
for (int i = 0; i < 3; i++)
{
    await chatCall.RequestStream.WriteAsync(new ChatMessage
    {
        SenderId = 123,
        Message = $"Hello, I need help with order {i + 1}",
        SentAt = DateTime.UtcNow.ToString("o")
    });
    await Task.Delay(1000);
}

// Complete the chat
await chatCall.RequestStream.CompleteAsync();
await readTask;

Console.WriteLine("Press any key to exit...");
Console.ReadKey();
