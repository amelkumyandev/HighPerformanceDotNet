using OrderProcessingServiceGRPC;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddGrpc(options =>
{
    options.Interceptors.Add<ServerInterceptor>();
});

var app = builder.Build();

app.MapGrpcService<OrderProcessingService>();
app.MapGet("/", () => "Use a gRPC client to communicate.");

app.Run();
