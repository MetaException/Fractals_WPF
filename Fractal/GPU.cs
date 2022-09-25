using ComputeSharp;

[AutoConstructor]
public readonly partial struct DrawMandelbrotSet : IComputeShader
{
    public readonly IReadWriteNormalizedTexture2D<float4> texture;

    public void Execute()
    {
        //a = x / масштаб * 1.777f(константа 16:9 для правильной отрисовки)
        float c_real = (float)(ThreadIds.X - (texture.Width / 2.0f)) / (float)(texture.Width / 4.0f) * 1.777f;

        //a = y / масштаб
        float c_imaginary = (float)(ThreadIds.Y - (texture.Height / 2.0f)) / (float)(texture.Height / 4.0f);

        float z_real = 0.0f; 
        float z_imaginary = 0.0f;

        int iterations = 0;
        do
        {
            iterations++;
            float z_real2 = (z_real * z_real) - (z_imaginary * z_imaginary) + c_real;
            float z_imaginary2 = (z_real * z_imaginary * 2) + c_imaginary;
            //z = z * z + c; // z = z ^ complexPower + c

            if (z_real2 * z_real2 + z_imaginary2 * z_imaginary2 >= 4.0f)
            {
                texture[ThreadIds.XY].RGB = 0 + (float)iterations / 64f;
                break;
            }
            if (iterations < 2)
            {
                float P = Hlsl.Sqrt(Hlsl.Pow(z_real2 - 1f / 4f, 2f) + z_imaginary2 * z_imaginary2);
                float O = Hlsl.Atan2(z_imaginary2, z_real2 - 1f / 4f);
                float Pc = 1f / 2f - 1f / 2f * Hlsl.Cos(O);
                if (P <= Pc)
                    break;
            }
            z_real = z_real2;
            z_imaginary = z_imaginary2;
        } while (iterations < 100);
    }
}