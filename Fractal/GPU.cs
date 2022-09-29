using ComputeSharp;

[AutoConstructor]
public readonly partial struct DrawMandelbrotSet : IComputeShader
{
    public readonly IReadWriteNormalizedTexture2D<float4> texture;
    public readonly float x1, x2, y1, y2;

    public void Execute()
    {
        //Цена деления = (x2 - x1) / texture.Width
        //Нужное число = x1 + (индекс * цена деления)
        //0..1, 5
        //Цд = (1 - 0)/5 = 0.2
        //N = 0 + (3 * 0.2) = 0.6

        //a = x / масштаб * 1.777f(константа 16:9 для правильной отрисовки)
        //float c_real = (float)(ThreadIds.X - (texture.Width / zoom / 2.0f)) / (float)(texture.Width / zoom / 4.0f) * 1.777f; //Добавить смещение при увеличении

        float c_real = x1 + (ThreadIds.X * ((x2 - x1) / texture.Width)) * 1.777f;

        //a = y / масштаб
        //float c_imaginary = (float)(ThreadIds.Y - (texture.Height / zoom / 2.0f)) / (float)(texture.Height / zoom / 4.0f);
        float c_imaginary = y1 + (ThreadIds.Y * ((y2 - y1) / texture.Height));

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
                texture[ThreadIds.XY].R = 0f + (float)iterations / 100 * 1.5f;
                texture[ThreadIds.XY].G = 0f + (float)iterations / 100 * 2f;
                texture[ThreadIds.XY].B = 0f + (float)iterations / 100 * 4f;
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
        } while (iterations < 1000);
    }
}