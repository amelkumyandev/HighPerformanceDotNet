namespace PoolingBenchmark.ObjectPooling
{
    public class ObjectPool<T> where T : new()
    {
        private readonly Stack<T> _pool = new Stack<T>();

        public T Rent()
        {
            return _pool.Count > 0 ? _pool.Pop() : new T();
        }

        public void Return(T obj)
        {
            _pool.Push(obj);  // Return the object to the pool
        }
    }

    public class PooledObject
    {
        public int[] Data { get; set; }

        public PooledObject()
        {
            Data = new int[1000];  // Simulate large internal state
        }

        public void Reset()
        {
            Array.Clear(Data, 0, Data.Length);  // Reset object state before reuse
        }
    }
}
