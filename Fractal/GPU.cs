using ComputeSharp;

[AutoConstructor]
public readonly partial struct DrawMandelbrotSet : IComputeShader
{
    public readonly IReadWriteNormalizedTexture2D<float4> texture;
    public readonly float2 buttomRight, topRight;
    public readonly int maxIterations;

    public void Execute()
    {
        float2 c = new float2(
            buttomRight.X + (ThreadIds.X * ((topRight.X - buttomRight.X) / texture.Width)) * 1.777f,
            buttomRight.Y + (ThreadIds.Y * ((topRight.Y - buttomRight.Y) / texture.Height))
        );

        float2 z = new float2(0f, 0f);

        int iterations = 0;
        do
        {
            iterations++;

            float2 zNext = new float2(
                (z.X * z.X) - (z.Y * z.Y) + c.X,
                (z.X * z.Y * 2) + c.Y
            );
            //z = z * z + c;

            if (Hlsl.Pow(zNext.X, 2f) + Hlsl.Pow(zNext.Y, 2f) >= 4.0f)
            {
                texture[ThreadIds.XY].R = 0f + (float)iterations / maxIterations * 10 * 1.5f;
                texture[ThreadIds.XY].G = 0f + (float)iterations / maxIterations * 10 * 2f;
                texture[ThreadIds.XY].B = 0f + (float)iterations / maxIterations * 10 * 4f;
                break;
            }
            if (iterations < 2)
            {
                float P = Hlsl.Sqrt(Hlsl.Pow(zNext.X - 1f / 4f, 2) + zNext.Y * zNext.Y);
                float O = Hlsl.Atan2(zNext.Y, zNext.X - 1f / 4f);
                float Pc = 1f / 2f - 1f / 2f * Hlsl.Cos(O);
                if (P <= Pc)
                    break;
            }
            z = zNext;
        } while (iterations < maxIterations);
    }
}