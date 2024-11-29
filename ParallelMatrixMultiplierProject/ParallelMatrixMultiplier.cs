using System.Runtime.InteropServices;

namespace ParallelMatrixMultiplierProject
{
    public class ParallelMatrixMultiplier
    {
        [DllImport("kernel32.dll")]
        public static extern IntPtr GetCurrentThread();

        [DllImport("kernel32.dll")]
        public static extern UIntPtr SetThreadAffinityMask(IntPtr hThread, UIntPtr dwThreadAffinityMask);

        public static void SetThreadAffinity(int coreId)
        {
            IntPtr thread = GetCurrentThread();
            UIntPtr affinityMask = new UIntPtr((uint)(1 << coreId));  // Bind thread to a specific core
            SetThreadAffinityMask(thread, affinityMask);
        }

        public static double[,] MultiplyMatrices(double[,] matrixA, double[,] matrixB)
        {
            int rowsA = matrixA.GetLength(0);
            int colsA = matrixA.GetLength(1);
            int colsB = matrixB.GetLength(1);
            double[,] result = new double[rowsA, colsB];

            Parallel.For(0, rowsA, i =>
            {
                SetThreadAffinity(i % Environment.ProcessorCount);  // Set thread affinity for each row
                for (int j = 0; j < colsB; j++)
                {
                    double sum = 0;
                    for (int k = 0; k < colsA; k++)
                    {
                        sum += matrixA[i, k] * matrixB[k, j];
                    }
                    result[i, j] = sum;
                }
            });

            return result;
        }
    }
}
