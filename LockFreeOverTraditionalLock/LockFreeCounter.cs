namespace LockFreeOverTraditionalLock
{
    public class LockFreeCounter
    {
        private int _count = 0;

        public void Increment()
        {
            Interlocked.Increment(ref _count); // Atomic increment
        }

        public int GetCount()
        {
            return _count;
        }
    }
}
