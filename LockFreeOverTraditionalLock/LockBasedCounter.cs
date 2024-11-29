namespace LockFreeOverTraditionalLock
{
    public class LockBasedCounter
    {
        private int _count = 0;
        private readonly object _lock = new object();

        public void Increment()
        {
            lock (_lock)
            {
                _count++;
            }
        }

        public int GetCount()
        {
            return _count;
        }
    }
}
