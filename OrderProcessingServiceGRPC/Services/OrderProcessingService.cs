using System.Collections.Concurrent;
using Grpc.Core;
using OrderProcessingServiceGRPC;

public class OrderProcessingService : OrderProcessing.OrderProcessingBase
{
    private static ConcurrentDictionary<int, OrderStatusUpdate> orderStatuses = new ConcurrentDictionary<int, OrderStatusUpdate>();

    private static ConcurrentDictionary<int, List<IServerStreamWriter<OrderStatusUpdate>>> orderStatusSubscribers = new ConcurrentDictionary<int, List<IServerStreamWriter<OrderStatusUpdate>>>();

    private static ConcurrentDictionary<int, List<IServerStreamWriter<ChatMessage>>> chatRooms = new ConcurrentDictionary<int, List<IServerStreamWriter<ChatMessage>>>();

    public override async Task<OrderResponse> PlaceOrder(OrderRequest request, ServerCallContext context)
    {
        // Process the order (simulate processing)
        var response = new OrderResponse
        {
            Success = true,
            Message = $"Order {request.OrderId} placed successfully.",
            EstimatedDeliveryDays = 5
        };

        // Update order status
        var statusUpdate = new OrderStatusUpdate
        {
            OrderId = request.OrderId,
            Status = "Order Placed",
            UpdatedAt = DateTime.UtcNow.ToString("o")
        };

        orderStatuses[request.OrderId] = statusUpdate;

        // Notify subscribers if any
        if (orderStatusSubscribers.TryGetValue(request.OrderId, out var subscribers))
        {
            List<IServerStreamWriter<OrderStatusUpdate>> subscribersCopy;

            lock (subscribers)
            {
                subscribersCopy = new List<IServerStreamWriter<OrderStatusUpdate>>(subscribers);
            }

            foreach (var subscriber in subscribersCopy)
            {
                try
                {
                    await subscriber.WriteAsync(statusUpdate);
                }
                catch
                {
                    // Handle write failures, possibly remove subscriber
                }
            }
        }

        return response;
    }

    public override async Task<OrderStatusUpdate> UpdateOrderStatus(UpdateOrderStatusRequest request, ServerCallContext context)
    {
        var statusUpdate = new OrderStatusUpdate
        {
            OrderId = request.OrderId,
            Status = request.Status,
            UpdatedAt = DateTime.UtcNow.ToString("o")
        };

        // Update the status in the dictionary
        orderStatuses[request.OrderId] = statusUpdate;

        // Notify subscribers if any
        if (orderStatusSubscribers.TryGetValue(request.OrderId, out var subscribers))
        {
            List<IServerStreamWriter<OrderStatusUpdate>> subscribersCopy;

            lock (subscribers)
            {
                subscribersCopy = new List<IServerStreamWriter<OrderStatusUpdate>>(subscribers);
            }

            foreach (var subscriber in subscribersCopy)
            {
                try
                {
                    await subscriber.WriteAsync(statusUpdate);
                }
                catch
                {
                    // Handle write failures, possibly remove subscriber
                }
            }
        }

        return statusUpdate;
    }

    public override async Task TrackOrderStatus(TrackOrderRequest request, IServerStreamWriter<OrderStatusUpdate> responseStream, ServerCallContext context)
    {
        var subscribers = orderStatusSubscribers.GetOrAdd(request.OrderId, _ => new List<IServerStreamWriter<OrderStatusUpdate>>());

        lock (subscribers)
        {
            subscribers.Add(responseStream);
        }

        // Send current status if available
        if (orderStatuses.TryGetValue(request.OrderId, out var status))
        {
            await responseStream.WriteAsync(status);
        }

        try
        {
            // Keep the stream open until cancellation
            await Task.Delay(Timeout.Infinite, context.CancellationToken);
        }
        catch (TaskCanceledException)
        {
            // Stream was cancelled
        }
        finally
        {
            // Remove subscriber on cancellation
            lock (subscribers)
            {
                subscribers.Remove(responseStream);
            }
        }
    }

    public override async Task<BulkOrderResponse> UploadBulkOrders(IAsyncStreamReader<OrderRequest> requestStream, ServerCallContext context)
    {
        int totalOrders = 0;
        var responses = new List<OrderResponse>();

        while (await requestStream.MoveNext())
        {
            var request = requestStream.Current;
            var response = await PlaceOrder(request, context);
            responses.Add(response);
            totalOrders++;
        }

        return new BulkOrderResponse
        {
            TotalOrdersProcessed = totalOrders,
            OrderResponses = { responses }
        };
    }

    public override async Task CustomerSupportChat(IAsyncStreamReader<ChatMessage> requestStream, IServerStreamWriter<ChatMessage> responseStream, ServerCallContext context)
    {
        // Extract chat room ID from headers
        int chatRoomId = 0;
        var chatRoomIdHeader = context.RequestHeaders.GetValue("chatroom-id");
        if (!string.IsNullOrEmpty(chatRoomIdHeader) && int.TryParse(chatRoomIdHeader, out var parsedChatRoomId))
        {
            chatRoomId = parsedChatRoomId;
        }
        else
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Chatroom ID is required and must be an integer."));
        }

        var chatParticipants = chatRooms.GetOrAdd(chatRoomId, _ => new List<IServerStreamWriter<ChatMessage>>());
        lock (chatParticipants)
        {
            chatParticipants.Add(responseStream);
        }

        try
        {
            // Read incoming messages and broadcast to participants
            while (await requestStream.MoveNext())
            {
                var message = requestStream.Current;

                // Broadcast the message
                List<IServerStreamWriter<ChatMessage>> participantsSnapshot;
                lock (chatParticipants)
                {
                    participantsSnapshot = new List<IServerStreamWriter<ChatMessage>>(chatParticipants);
                }

                foreach (var participant in participantsSnapshot)
                {
                    try
                    {
                        await participant.WriteAsync(message);
                    }
                    catch
                    {
                        // Handle write failures, possibly remove participant
                    }
                }
            }
        }
        finally
        {
            // Remove participant on disconnection
            lock (chatParticipants)
            {
                chatParticipants.Remove(responseStream);
            }
        }
    }
}
