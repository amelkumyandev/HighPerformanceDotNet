namespace OptimizedMatrixMultiplierVsNonOptimizedMatrixMultiplier
{
    public class NonOptimizedMatrixMultiplier
    {
        public static int[,] MultiplyMatrices(int[,] matrixA, int[,] matrixB)
        {
            int aRows = matrixA.GetLength(0);
            int aCols = matrixA.GetLength(1);
            int bCols = matrixB.GetLength(1);
            int[,] result = new int[aRows, bCols];

            // Parallelizing the outer loop for row calculation
            Parallel.For(0, aRows, row =>
            {
                for (int col = 0; col < bCols; col++)
                {
                    int sum = 0;
                    for (int k = 0; k < aCols; k++)
                    {
                        sum += matrixA[row, k] * matrixB[k, col];
                    }
                    result[row, col] = sum;
                }
            });

            return result;
        }
    }
}
