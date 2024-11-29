using BenchmarkDotNet.Attributes;
using System.Runtime.InteropServices;


namespace POHBenchmarking
{
    public class PinnedObjectTest
    {
        private const

        int BufferSize = 1024;

        [Benchmark]
        public void ProcessWithoutPOH()
        {
            for (int i = 0; i < 1000; i++)
            {
                byte[] buffer = new byte[BufferSize];
                GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);

                try
                {
                    // Simulate processing the pinned buffer
                    ProcessPinnedBuffer(buffer);
                }
                finally
                {
                    handle.Free(); // Release pinned object
                }
            }
        }

        [Benchmark]
        public void ProcessWithPOH()
        {
            List<GCHandle> pinnedHandles = new List<GCHandle>();

            for (int i = 0; i < 1000; i++)
            {
                byte[] buffer = new byte[BufferSize];
                GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
                pinnedHandles.Add(handle);

                // Simulate processing the pinned buffer
                ProcessPinnedBuffer(buffer);
            }

            foreach (var handle in pinnedHandles)
            {
                handle.Free();
            }
        }

        private void ProcessPinnedBuffer(byte[] buffer)
        {
            // Simulate data processing
            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = (byte)(buffer[i] + 1);
            }
        }

    }
}
