using ConcurrentTaskProcessing;

var taskProcessingSystem = new TaskProcessingSystem();

// Process 10 tasks concurrently
await taskProcessingSystem.ProcessTasksConcurrently(10);

// Print the logs of all processed tasks
taskProcessingSystem.PrintLogs();

// Check the result of a specific task
Console.WriteLine(taskProcessingSystem.LookupTaskResult(3));