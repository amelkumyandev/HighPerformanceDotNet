namespace TasksArePreferableOverThreads
{
    public  class IoBoundTaskWithThreads
    {
        private List<Thread> _threads =  new List<Thread>();

        public void StartTasks(int numberOfThreads)
        {
            for (int i = 0; i < numberOfThreads; i++)
            {
                var thread = new Thread(() => PerformIoTask());
                thread.Start();
                _threads.Add(thread);
            }

            foreach (var thread in _threads)
            {
                thread.Join();
            }
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
