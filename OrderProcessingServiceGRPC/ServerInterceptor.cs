using Grpc.Core.Interceptors;
using Grpc.Core;

namespace OrderProcessingServiceGRPC
{
    public class ServerInterceptor : Interceptor
    {
        public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
            TRequest request,
            ServerCallContext context,
            UnaryServerMethod<TRequest, TResponse> continuation)
        {
            // Logging
            Console.WriteLine($"Received request for method {context.Method} from {context.Peer}");

            // Authentication (simplified)
            if (!context.RequestHeaders.Any(h => h.Key == "authorization" && h.Value == "Bearer valid_token"))
            {
                throw new RpcException(new Status(StatusCode.Unauthenticated, "Invalid token"));
            }

            return await continuation(request, context);
        }

        // Implement similar methods for streaming interceptors if needed
    }

}
