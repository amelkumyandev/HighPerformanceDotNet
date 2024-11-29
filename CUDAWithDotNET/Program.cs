using BenchmarkDotNet.Running;
using CudaWithDotNetGPU;

namespace CUDAWithDotNET
{
    public class Program
    {
        public static void Main()
        {
            BenchmarkRunner.Run<VectorAddition>();
        }
    }
}
