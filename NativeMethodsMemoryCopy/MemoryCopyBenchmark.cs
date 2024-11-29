using System.Runtime.InteropServices;
using BenchmarkDotNet.Attributes;
namespace NativeMethodsMemoryCopy
{


    public class UnmanagedMemoryCopyBenchmark
    {
        private const int BufferSize = 1024 * 1024 * 500; // 500 MB buffer size
        private IntPtr unmanagedSource;
        private IntPtr unmanagedDestination;
        private byte[] managedSource;
        private byte[] managedDestination;

        [GlobalSetup]
        public void Setup()
        {
            // Allocate unmanaged memory for source and destination
            unmanagedSource = Marshal.AllocHGlobal(BufferSize);
            unmanagedDestination = Marshal.AllocHGlobal(BufferSize);

            // Fill the unmanaged source with sample data
            byte[] data = new byte[BufferSize];
            for (int i = 0; i < BufferSize; i++)
                data[i] = (byte)(i % 256);

            // Copy data from managed to unmanaged memory as a setup step
            Marshal.Copy(data, 0, unmanagedSource, BufferSize);

            // Prepare managed arrays for comparison
            managedSource = new byte[BufferSize];
            managedDestination = new byte[BufferSize];
            Array.Copy(data, managedSource, BufferSize);
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            Marshal.FreeHGlobal(unmanagedSource);
            Marshal.FreeHGlobal(unmanagedDestination);
        }

        [Benchmark]
        public void UnmanagedToUnmanagedCopy()
        {
            // Copy from unmanaged source to unmanaged destination
            NativeMethods.RtlMoveMemory(unmanagedDestination, unmanagedSource, (uint)BufferSize);
        }

        [Benchmark]
        public void ManagedArrayCopy()
        {
            // Copy data within managed arrays
            Array.Copy(managedSource, managedDestination, BufferSize);
        }
    }

    public class NativeMethods
    {
        [DllImport("kernel32.dll", EntryPoint = "RtlMoveMemory", SetLastError = false)]
        public static extern void RtlMoveMemory(IntPtr dest, IntPtr src, uint count);
    }


}
