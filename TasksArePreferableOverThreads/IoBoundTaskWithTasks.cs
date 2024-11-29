using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TasksArePreferableOverThreads
{
    public class IoBoundTaskWithTasks
    {
        public async Task StartTasksAsync(int numberOfTasks)
        {
            var tasks = new List<Task>();
            for (int i = 0; i < numberOfTasks; i++)
            {
                tasks.Add(Task.Run(() => PerformIoTask()));
            }

            await Task.WhenAll(tasks);  // Ensure all tasks complete
        }

        private void PerformIoTask()
        {
            // Generate a unique file name using a timestamp
            string uniqueFileName = $"output_{DateTime.Now:yyyyMMddHHmmssfff}_{Guid.NewGuid()}.txt";
            string contentToWrite = $"This is an actual I/O task, writing to {uniqueFileName}.";

            // Writing to a file
            using (StreamWriter writer = new StreamWriter(uniqueFileName, true)) // 'true' appends to the file if it exists
            {
                writer.WriteLine(contentToWrite);
            }

        }

    }

}
