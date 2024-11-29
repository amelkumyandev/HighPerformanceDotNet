using ComputeSharp;
using MatrixMultiplicationComputeSharp;

float[] array = [.. Enumerable.Range(1, 100)];

// Create the graphics buffer
using ReadWriteBuffer<float> gpuBuffer = GraphicsDevice.GetDefault().AllocateReadWriteBuffer(array);

// Run the shader
GraphicsDevice.GetDefault().For(100, new MultiplyByTwo(gpuBuffer));

// Print the initial matrix
MatrixMultiplicationComputeSharp.Formatting.PrintMatrix(array, 10, 10, "BEFORE");

// Get the data back
gpuBuffer.CopyTo(array);

// Print the updated matrix
MatrixMultiplicationComputeSharp.Formatting.PrintMatrix(array, 10, 10, "AFTER");

/// <summary>
/// The sample kernel that multiplies all items by two.
/// </summary>
[ThreadGroupSize(DefaultThreadGroupSizes.X)]
[GeneratedComputeShaderDescriptor]
internal readonly partial struct MultiplyByTwo(ReadWriteBuffer<float> buffer) : IComputeShader
{
    /// <inheritdoc/>
    public void Execute()
    {
        buffer[ThreadIds.X] *= 2;
    }
}

