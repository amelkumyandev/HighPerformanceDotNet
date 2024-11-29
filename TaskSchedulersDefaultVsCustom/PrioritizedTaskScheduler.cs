namespace TaskSchedulersDefaultVsCustom
{
    public class PrioritizedTaskScheduler : TaskScheduler
    {
        private readonly LinkedList<Task> _highPriorityTasks = new LinkedList<Task>();
        private readonly LinkedList<Task> _lowPriorityTasks = new LinkedList<Task>();

        protected override IEnumerable<Task> GetScheduledTasks()
        {
            lock (_highPriorityTasks)
            {
                return _highPriorityTasks.Concat(_lowPriorityTasks).ToArray();
            }
        }

        protected override void QueueTask(Task task)
        {
            lock (_highPriorityTasks)
            {
                if (task.CreationOptions == TaskCreationOptions.PreferFairness)
                {
                    _lowPriorityTasks.AddLast(task);
                }
                else
                {
                    _highPriorityTasks.AddLast(task);
                }
            }
            ThreadPool.QueueUserWorkItem(_ => StartNextTask(), null);
        }

        private void StartNextTask()
        {
            Task taskToExecute = null;
            lock (_highPriorityTasks)
            {
                if (_highPriorityTasks.Count > 0)
                {
                    taskToExecute = _highPriorityTasks.First.Value;
                    _highPriorityTasks.RemoveFirst();
                }
                else if (_lowPriorityTasks.Count > 0)
                {
                    taskToExecute = _lowPriorityTasks.First.Value;
                    _lowPriorityTasks.RemoveFirst();
                }
            }

            if (taskToExecute != null)
            {
                TryExecuteTask(taskToExecute);
            }
        }

        protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
        {
            return TryExecuteTask(task);
        }
    }

}
