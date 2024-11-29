using System.Diagnostics.Tracing;

namespace EventCounters
{

    [EventSource(Name = "MyCompany.MyApp.EventSource")]
    public sealed class MyEventSource : EventSource
    {
        public static readonly MyEventSource Log = new MyEventSource();

        private EventCounter _requestProcessingTimeCounter;
        private IncrementingEventCounter _requestRateCounter;

        private MyEventSource()
        {
            _requestProcessingTimeCounter = new EventCounter("request-processing-time", this)
            {
                DisplayName = "Request Processing Time",
                DisplayUnits = "ms"
            };

            _requestRateCounter = new IncrementingEventCounter("request-rate", this)
            {
                DisplayName = "Request Rate",
                DisplayUnits = "req/sec"
            };
        }

        public void RequestProcessed(double processingTime)
        {
            _requestProcessingTimeCounter.WriteMetric(processingTime);
            _requestRateCounter.Increment();
        }

        protected override void Dispose(bool disposing)
        {
            _requestProcessingTimeCounter?.Dispose();
            _requestRateCounter?.Dispose();
            base.Dispose(disposing);
        }
    }

}
