namespace TaskSchedulersDefaultVsCustom
{
    public class LimitedConcurrencyTaskScheduler : TaskScheduler
    {
        private readonly LinkedList<Task> _tasks = new LinkedList<Task>();
        private readonly int _maxDegreeOfParallelism;
        private int _runningTasks = 0;

        public LimitedConcurrencyTaskScheduler(int maxDegreeOfParallelism)
        {
            _maxDegreeOfParallelism = maxDegreeOfParallelism;
        }

        protected override IEnumerable<Task> GetScheduledTasks()
        {
            lock (_tasks)
            {
                return _tasks.ToArray();
            }
        }

        protected override void QueueTask(Task task)
        {
            lock (_tasks)
            {
                _tasks.AddLast(task);
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
            lock (_tasks)
            {
                if (_tasks.Count > 0)
                {
                    taskToExecute = _tasks.First.Value;
                    _tasks.RemoveFirst();
                }
            }

            if (taskToExecute != null)
            {
                ThreadPool.QueueUserWorkItem(_ =>
                {
                    TryExecuteTask(taskToExecute);
                    lock (_tasks)
                    {
                        _runningTasks--;
                        if (_tasks.Count > 0)
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

        public override int MaximumConcurrencyLevel => _maxDegreeOfParallelism;
    }

}
