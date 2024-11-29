namespace EventCounters
{
    public class RequestHandler
    {
        public async Task HandleRequestAsync()
        {
            var startTime = DateTime.UtcNow;

            // Simulate request processing
            await Task.Delay(new Random().Next(50, 150));

            var processingTime = (DateTime.UtcNow - startTime).TotalMilliseconds;
            MyEventSource.Log.RequestProcessed(processingTime);
        }
    }
}
