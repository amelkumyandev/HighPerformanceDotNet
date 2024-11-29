using System.Numerics;

namespace ParallelismAndSIMD
{
    public static class OptimizedDataProcessing
    {
        public static double[] ProcessDataOptimized(double[] data)
        {
            int length = data.Length;
            double[] result = new double[length];
            int vectorSize = Vector<double>.Count;
            int i = 0;

            // Process data in vectorized chunks
            for (; i <= length - vectorSize; i += vectorSize)
            {
                var inputVector = new Vector<double>(data, i);

                // Perform vectorized operations
                var oneVector = new Vector<double>(1.0);
                var sqrtVector = Vector.SquareRoot(inputVector);
                var logVector = Vector.Log(Vector.Add(inputVector, oneVector));
                var numerator = Vector.Multiply(sqrtVector, logVector);
                var denominator = Vector.Add(inputVector, oneVector);
                var resultVector = Vector.Divide(numerator, denominator);

                resultVector.CopyTo(result, i);
            }

            // Process remaining elements sequentially
            for (; i < length; i++)
            {
                result[i] = ComplexComputation(data[i]);
            }

            return result;
        }

        private static double ComplexComputation(double input)
        {
            return Math.Pow(Math.Sin(input), 2) + Math.Pow(Math.Cos(input), 2);
        }

        // Helper extension for squaring a Vector
        private static Vector<double> Square(this Vector<double> vector)
        {
            return Vector.Multiply(vector, vector);
        }
    }
}
