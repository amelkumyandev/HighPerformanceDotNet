using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using ILGPU;
using ILGPU.Runtime;

namespace GPUImageProcessing
{
    class Program
    {
        static void Main(string[] args)
        {
            // Load the image
            Bitmap bitmap = new Bitmap("input.png");
            int width = bitmap.Width;
            int height = bitmap.Height;

            // Lock the bitmap's bits
            BitmapData bmpData = bitmap.LockBits(
                new Rectangle(0, 0, width, height),
                ImageLockMode.ReadOnly,
                PixelFormat.Format32bppArgb);

            int bytes = Math.Abs(bmpData.Stride) * height;
            byte[] pixelData = new byte[bytes];
            Marshal.Copy(bmpData.Scan0, pixelData, 0, bytes);
            bitmap.UnlockBits(bmpData);

            // Create an ILGPU context and accelerator
            using (var context = Context.Create(builder => builder.AllAccelerators()))
            {
                // Select the preferred device (GPU preferred)
                Device device = context.GetPreferredDevice(preferCPU: false);
                using (var accelerator = device.CreateAccelerator(context))
                {
                    Console.WriteLine($"Running on {accelerator.AcceleratorType} accelerator.");
                    accelerator.PrintInformation();

                    // Allocate memory on the GPU
                    var inputBuffer = accelerator.Allocate1D<byte>(pixelData.Length);
                    var outputBuffer = accelerator.Allocate1D<byte>(pixelData.Length);

                    // Copy input data to GPU
                    inputBuffer.CopyFromCPU(pixelData);

                    // Compile the grayscale kernel
                    var grayscaleKernel = accelerator.LoadAutoGroupedStreamKernel<
                        Index1D, ArrayView<byte>, ArrayView<byte>>(GrayscaleKernel);

                    // Launch the kernel
                    int numPixels = pixelData.Length / 4; // 4 bytes per pixel (RGBA)
                    grayscaleKernel(numPixels, inputBuffer.View, outputBuffer.View);

                    accelerator.Synchronize();

                    // Retrieve the result from the GPU
                    byte[] result = outputBuffer.GetAsArray1D();

                    // Save the result image
                    Bitmap resultBitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
                    BitmapData resultData = resultBitmap.LockBits(
                        new Rectangle(0, 0, width, height),
                        ImageLockMode.WriteOnly,
                        PixelFormat.Format32bppArgb);

                    Marshal.Copy(result, 0, resultData.Scan0, result.Length);
                    resultBitmap.UnlockBits(resultData);

                    resultBitmap.Save("output_grayscale.png", ImageFormat.Png);

                    // Dispose GPU buffers
                    inputBuffer.Dispose();
                    outputBuffer.Dispose();
                }
            }

            Console.WriteLine("Grayscale conversion completed.");
        }

        // GPU Kernel for Grayscale conversion
        static void GrayscaleKernel(Index1D index, ArrayView<byte> input, ArrayView<byte> output)
        {
            int i = index * 4; // 4 components per pixel (RGBA)

            byte b = input[i];     // Blue component
            byte g = input[i + 1]; // Green component
            byte r = input[i + 2]; // Red component

            // Compute the grayscale value using the luminance formula
            byte gray = (byte)(0.299f * r + 0.587f * g + 0.114f * b);

            // Set the output pixel's RGB components to the grayscale value
            output[i] = gray;           // Blue
            output[i + 1] = gray;       // Green
            output[i + 2] = gray;       // Red
            output[i + 3] = input[i + 3]; // Alpha (transparency)
        }
    }
}
