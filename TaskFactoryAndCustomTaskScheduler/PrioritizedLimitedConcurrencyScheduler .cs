namespace TaskFactoryAndCustomTaskScheduler
{
    public class PrioritizedLimitedConcurrencyScheduler : TaskScheduler
    {
        private readonly LinkedList<Task> _highPriorityTasks = new LinkedList<Task>();
        private readonly LinkedList<Task> _lowPriorityTasks = new LinkedList<Task>();
        private readonly int _maxDegreeOfParallelism;
        private int _runningTasks = 0;

        public PrioritizedLimitedConcurrencyScheduler(int maxDegreeOfParallelism)
        {
            _maxDegreeOfParallelism = maxDegreeOfParallelism;
        }

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

                if (_runningTasks < _maxDegreeOfParallelism)
                {
                    _runningTasks++;
                    StartNextTask();
                }
            }
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
                ThreadPool.QueueUserWorkItem(_ =>
                {
                    TryExecuteTask(taskToExecute);
                    lock (_highPriorityTasks)
                    {
                        _runningTasks--;
                        if (_highPriorityTasks.Count + _lowPriorityTasks.Count > 0)
                        {
                            _runningTasks++;
                            StartNextTask();
                        }
                    }
                });
            }
        }

        protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
        {
            return false; // No inlining allowed
        }
    }

}
