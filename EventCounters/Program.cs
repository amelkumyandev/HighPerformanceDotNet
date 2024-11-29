using EventCounters;

var handler = new RequestHandler();

while (true)
{
    await handler.HandleRequestAsync();
    // Simulate varying request intervals
    await Task.Delay(new Random().Next(500, 1000));
}