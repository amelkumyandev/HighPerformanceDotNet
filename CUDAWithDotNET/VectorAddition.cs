using ILGPU;
using ILGPU.Runtime;
using ILGPU.Runtime.Cuda;
using BenchmarkDotNet.Attributes;

namespace CudaWithDotNetGPU
{
    public class VectorAddition
    {
        private const int Length = 1 << 26; // Adjust based on testing needs

        private float[] a;
        private float[] b;
        private float[] cpuResult;
        private float[] gpuResult;

        private Context context;
        private Accelerator accelerator;
        private Action<Index1D, ArrayView<float>, ArrayView<float>, ArrayView<float>> gpuKernel;

        private MemoryBuffer1D<float, Stride1D.Dense> bufferA;
        private MemoryBuffer1D<float, Stride1D.Dense> bufferB;
        private MemoryBuffer1D<float, Stride1D.Dense> bufferResult;

        [GlobalSetup]
        public void Setup()
        {
            // Initialize data
            a = new float[Length];
            b = new float[Length];
            cpuResult = new float[Length];
            gpuResult = new float[Length];

            for (int i = 0; i < Length; i++)
            {
                a[i] = i;
                b[i] = Length - i;
            }

            // Initialize ILGPU context and accelerator
            context = Context.Create(builder => builder.Cuda());
            accelerator = context.CreateCudaAccelerator(0);

            // Compile the GPU kernel
            gpuKernel = accelerator.LoadAutoGroupedStreamKernel<Index1D, ArrayView<float>, ArrayView<float>, ArrayView<float>>(GPUVectorAdd);

            // Allocate GPU buffers once and reuse them
            bufferA = accelerator.Allocate1D(a);
            bufferB = accelerator.Allocate1D(b);
            bufferResult = accelerator.Allocate1D<float>(Length);
        }

        [Benchmark]
        public void CPUVectorAdd()
        {
            for (int i = 0; i < Length; i++)
            {
                cpuResult[i] = a[i] + b[i];
            }
        }

        [Benchmark]
        public void GPUVectorAdd()
        {
            // Launch kernel
            gpuKernel(new Index1D(Length), bufferA.View, bufferB.View, bufferResult.View);
            accelerator.Synchronize();
        }

        // Optimized GPU Kernel
        public static void GPUVectorAdd(Index1D index, ArrayView<float> a, ArrayView<float> b, ArrayView<float> result)
        {
            result[index] = a[index] + b[index];
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            // Dispose of GPU resources
            bufferA.Dispose();
            bufferB.Dispose();
            bufferResult.Dispose();
            accelerator.Dispose();
            context.Dispose();
        }
    }
}
