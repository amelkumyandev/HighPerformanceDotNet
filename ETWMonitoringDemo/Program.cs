using System;
using System.Threading;

namespace ETWMonitoringDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            MyEventSource.Log.AppStarted();
            Console.WriteLine("Starting ETW Monitoring Demo...");

            // Simulate work
            for (int i = 0; i < 5; i++)
            {
                MyEventSource.Log.TaskStarted(i);
                PerformTask(i);
                MyEventSource.Log.TaskCompleted(i);
            }

            Console.WriteLine("Demo completed.");
        }

        static void PerformTask(int taskId)
        {
            try
            {
                Console.WriteLine($"Starting task {taskId}...");
                Thread.Sleep(5000); // Simulate I/O-bound work

                double result = 0;
                for (int i = 0; i < 1_000_000; i++)
                {
                    result += Math.Sqrt(i);
                }

                Console.WriteLine($"Completed task {taskId}.");
            }
            catch (Exception ex)
            {
                MyEventSource.Log.Error(ex.Message);
            }
        }
    }
}
