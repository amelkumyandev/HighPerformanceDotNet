namespace ParallelismAndSIMD
{
    public static class SimpleDataProcessing
    {
        public static double[] ProcessData(double[] data)
        {
            double[] result = new double[data.Length];
            for (int i = 0; i < data.Length; i++)
            {
                result[i] = ComplexComputation(data[i]);
            }
            return result;
        }

        private static double ComplexComputation(double input)
        {
            // Intensive computation
            return Math.Pow(Math.Sin(input), 2) + Math.Pow(Math.Cos(input), 2);
        }
    }
}
