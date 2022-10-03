using ComputeSharp;

[AutoConstructor]
public readonly partial struct DrawMandelbrotSet : IComputeShader
{
    public readonly IReadWriteNormalizedTexture2D<float4> texture;
    public readonly float x1, x2, y1, y2;
    public readonly int maxIterations;

    public void Execute()
    {
        float c_real = x1 + (ThreadIds.X * ((x2 - x1) / texture.Width)) * 1.777f;
        float c_imaginary = y1 + (ThreadIds.Y * ((y2 - y1) / texture.Height));
        
        float z_real = 0.0f;
        float z_imaginary = 0.0f;

        int iterations = 0;
        do
        {
            iterations++;
            float z_real2 = (z_real * z_real) - (z_imaginary * z_imaginary) + c_real;
            float z_imaginary2 = (z_real * z_imaginary * 2) + c_imaginary;
            //z = z * z + c;

            if (z_real2 * z_real2 + z_imaginary2 * z_imaginary2 > 4.0f)
            {
                texture[ThreadIds.XY].R = 0f + (float)iterations / maxIterations * 10 * 1.5f;
                texture[ThreadIds.XY].G = 0f + (float)iterations / maxIterations * 10 * 2f;
                texture[ThreadIds.XY].B = 0f + (float)iterations / maxIterations * 10 * 4f;
                break;
            }
            if (iterations < 2)
            {
                float P = Hlsl.Sqrt(Hlsl.Pow(z_real2 - 1f / 4f, 2) + z_imaginary2 * z_imaginary2);
                float O = Hlsl.Atan2(z_imaginary2, z_real2 - 1f / 4f);
                float Pc = 1f / 2f - 1f / 2f * Hlsl.Cos(O);
                if (P <= Pc)
                    break;
            }
            z_real = z_real2;
            z_imaginary = z_imaginary2;
        } while (iterations < maxIterations);
    }
}