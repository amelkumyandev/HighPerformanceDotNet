using System.Diagnostics.Tracing;

namespace ETWMonitoringDemo
{
    [EventSource(Name = "ETWMonitoringDemo-MyEventSource")]
    public sealed class MyEventSource : EventSource
    {
        public static MyEventSource Log = new MyEventSource();

        [Event(1, Message = "Application started.", Level = EventLevel.Informational)]
        public void AppStarted()
        {
            WriteEvent(1);
        }

        [Event(2, Message = "Starting task {0}.", Level = EventLevel.Informational)]
        public void TaskStarted(int taskId)
        {
            WriteEvent(2, taskId);
        }

        [Event(3, Message = "Completed task {0}.", Level = EventLevel.Informational)]
        public void TaskCompleted(int taskId)
        {
            WriteEvent(3, taskId);
        }

        [Event(4, Message = "Error: {0}", Level = EventLevel.Error)]
        public void Error(string message)
        {
            WriteEvent(4, message);
        }
    }
}
