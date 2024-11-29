using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace OptimizedMatrixMultiplierVsNonOptimizedMatrixMultiplier
{
    public class OptimizedMatrixMultiplier
    {
        // Struct with padding to avoid cache line sharing
        [StructLayout(LayoutKind.Explicit, Size = 128)]
        public struct PaddedResult
        {
            [FieldOffset(0)]
            public int Value;
        }

        // Using PaddedResult array for direct manipulation
        public static PaddedResult[,] MultiplyMatrices(int[,] matrixA, int[,] matrixB)
        {
            int aRows = matrixA.GetLength(0);
            int aCols = matrixA.GetLength(1);
            int bCols = matrixB.GetLength(1);
            PaddedResult[,] result = new PaddedResult[aRows, bCols]; // Directly work with padded structure

            // Get the number of logical processors (physical and hyperthreads)
            int logicalProcessors = Environment.ProcessorCount;

            // Parallelism with Thread Affinity
            Parallel.For(0, aRows, new ParallelOptions { MaxDegreeOfParallelism = logicalProcessors }, row =>
            {
                int coreId = row % logicalProcessors; // Assign thread affinity based on logical core ID
                SetThreadAffinity(coreId);  // Set the thread affinity to a specific logical core (both physical and hyper-threads)

                try
                {
                    ProcessRow(matrixA, matrixB, result, row);
                }
                finally
                {
                    // Reset thread affinity is optional (depends on application behavior)
                    ResetThreadAffinity();
                }
            });

            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ProcessRow(int[,] matrixA, int[,] matrixB, PaddedResult[,] result, int row)
        {
            int aCols = matrixA.GetLength(1);
            int bCols = matrixB.GetLength(1);

            for (int col = 0; col < bCols; col++)
            {
                int sum = 0; // Local sum to avoid unnecessary memory accesses
                for (int k = 0; k < aCols; k++)
                {
                    sum += matrixA[row, k] * matrixB[k, col];
                }

                // Padding to prevent false sharing, directly assigning to padded result
                result[row, col].Value = sum;
            }
        }

        // Set thread affinity to a specific core using Windows API (supporting hyper-threading)
        private static void SetThreadAffinity(int coreId)
        {
            IntPtr mask = new IntPtr(1 << coreId);  // Create affinity mask for the specific logical core
            IntPtr thread = GetCurrentThread();     // Get current thread handle
            SetThreadAffinityMask(thread, mask);    // Set thread affinity to the logical core
        }

        // Optional: Reset thread affinity if necessary
        private static void ResetThreadAffinity()
        {
            IntPtr thread = GetCurrentThread();
            SetThreadAffinityMask(thread, new IntPtr(0xFFFFFFFF)); // Reset affinity to any core
        }

        // P/Invoke to use Windows API for setting thread affinity
        [DllImport("kernel32.dll")]
        private static extern IntPtr GetCurrentThread();

        [DllImport("kernel32.dll")]
        private static extern IntPtr SetThreadAffinityMask(IntPtr hThread, IntPtr dwThreadAffinityMask);
    }
}
