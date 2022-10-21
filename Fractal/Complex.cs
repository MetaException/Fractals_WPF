using ComputeSharp;

namespace Fractal
{
    public struct Complex
    {
        public float Real; // X
        public float Imaginary; // Y

        public static Complex Zero() => FromValue(0, 0);

        public static Complex One() => FromValue(1, 0);

        public static Complex FromValue(float r, float i)
        {
            Complex toReturn;
            toReturn.Real = r;
            toReturn.Imaginary = i;
            return toReturn;
        }

        public static Complex Add(Complex left, Complex right)
        {
            Complex toReturn;
            toReturn.Real = left.Real + right.Real;
            toReturn.Imaginary = left.Imaginary + right.Imaginary;
            return toReturn;
        }

        public static Complex Multiply(Complex left, Complex right)
        {
            Complex toReturn;
            toReturn.Real = (left.Real * right.Real) - (left.Imaginary * right.Imaginary);
            toReturn.Imaginary = (left.Imaginary * right.Real) + (left.Real * right.Imaginary);
            return toReturn;
        }

        public static float Abs(Complex value) =>
            Hlsl.Sqrt(value.Real * value.Real + value.Imaginary * value.Imaginary);

        /*public static Complex Pow(Complex value, Complex power)
        {
            if (power.Real == 0 && power.Imaginary == 0) { return Complex.One(); }
            if (value.Real == 0 && value.Imaginary == 0) { return Complex.Zero(); }

            var a = value.Real;
            var b = value.Imaginary;
            var c = power.Real;
            var d = power.Imaginary;

            var rho = Complex.Abs(value);
            var theta = Hlsl.Atan2(b, a);
            var newRho = c * theta + d * Hlsl.Log(rho);

            var t = Hlsl.Pow(rho, c) * Hlsl.Pow(2.718282f, -d * theta);
            return FromValue(t * Hlsl.Cos(newRho), t * Hlsl.Sin(newRho));
        }*/
    }
}