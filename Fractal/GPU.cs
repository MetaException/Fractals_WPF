using ComputeSharp;
using Fractal;

[AutoConstructor]
public readonly partial struct DrawMandelbrotSet : IComputeShader
{
    public readonly IReadWriteNormalizedTexture2D<float4> texture;
    public readonly Complex buttomLeft, topRight;
    public readonly int maxIterations;

    public void Execute()
    {
        Complex c = Complex.FromValue(
            buttomLeft.Real + (ThreadIds.X * ((topRight.Real - buttomLeft.Real) / texture.Width)) * 1.777f,
            buttomLeft.Imaginary + (ThreadIds.Y * ((topRight.Imaginary - buttomLeft.Imaginary) / texture.Height))
        );

        Complex z = Complex.FromValue(0f, 0f);

        int iterations = 0;
        do
        {
            iterations++;
            //z = z * z + c
            z = Complex.Add(Complex.Multiply(z, z), c);

            if (Complex.Abs(z) > 2.0f)
            {
                texture[ThreadIds.XY].R = 0f + (float)iterations / maxIterations * 10 * 1.5f;
                texture[ThreadIds.XY].G = 0f + (float)iterations / maxIterations * 10 * 2f;
                texture[ThreadIds.XY].B = 0f + (float)iterations / maxIterations * 10 * 4f;
                break;
            }
            if (iterations < 2)
            {
                float P = Hlsl.Sqrt(Hlsl.Pow(z.Real - 1f / 4f, 2) + z.Imaginary * z.Imaginary);
                float O = Hlsl.Atan2(z.Imaginary, z.Real - 1f / 4f);
                float Pc = 1f / 2f - 1f / 2f * Hlsl.Cos(O);
                if (P <= Pc)
                    break;
            }
        } while (iterations < maxIterations);
    }
}