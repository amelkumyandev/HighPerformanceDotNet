using ParallelMatrixMultiplierProject;

double[,] matrixA = GenerateMatrix(1000, 1000);
double[,] matrixB = GenerateMatrix(1000, 1000);

double[,] result = ParallelMatrixMultiplier.MultiplyMatrices(matrixA, matrixB);

Console.WriteLine("Matrix multiplication completed.");

static double[,] GenerateMatrix(int rows, int cols)
{
    Random random = new Random();
    double[,] matrix = new double[rows, cols];

    for (int i = 0; i < rows; i++)
    {
        for (int j = 0; j < cols; j++)
        {
            matrix[i, j] = random.NextDouble();
        }
    }

    return matrix;
}