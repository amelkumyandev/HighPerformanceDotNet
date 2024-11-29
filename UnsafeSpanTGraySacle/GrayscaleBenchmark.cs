using BenchmarkDotNet.Attributes;

namespace UnsafeSpanTGraySacle
{
    [MemoryDiagnoser]
    public class GrayscaleBenchmark
    {
        private byte[] pixels;

        [GlobalSetup]
        public void Setup()
        {
            // Simulate a 19200x10800 image with 4 bytes per pixel (RGBA)
            pixels = new byte[19200 * 10800 * 4];
            new Random(42).NextBytes(pixels);
        }

        [Benchmark(Baseline = true)]
        public void Standard()
        {
           Grayscale.GrayscaleStandard(pixels);
        }

        [Benchmark]
        public void Advanced()
        {
            Grayscale.GrayscaleAdvanced(pixels);
        }
    }

}
