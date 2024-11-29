using BenchmarkDotNet.Attributes;
using PoolingBenchmark.ObjectPooling;

namespace PoolingBenchmark.Benchmarks
{
    [MemoryDiagnoser]
    public class ObjectPoolBenchmark
    {
        private ObjectPool<PooledObject> objectPool = new ObjectPool<PooledObject>();

        [Benchmark]
        public void NewObjectAllocation()
        {
            var obj = new PooledObject();
            obj.Reset();
        }

        [Benchmark]
        public void ObjectPoolAllocation()
        {
            var obj = objectPool.Rent();
            try
            {
                obj.Reset();
            }
            finally
            {
                objectPool.Return(obj);  // Return object to pool
            }
        }
    }
}
