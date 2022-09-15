﻿using System.Numerics;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Fractals
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private double complexPower = 2;
        private WriteableBitmap wb;
        private Int32Rect rect;
        private byte[] pixels;

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Создаётся битмап размером с окно (экран)
            wb = new WriteableBitmap((int)Width,
                (int)Height, 96, 96, PixelFormats.Bgr32, null);

            // Задаётся рабочая область окна
            rect = new Int32Rect(0, 0, (int)Width, (int)Height);

            //Массив байтов буффера изображения(bitmap)
            pixels = new byte[(int)Width * (int)Height * wb.Format.BitsPerPixel / 8];

            Draw();
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Right)
            {
                complexPower += 0.05;
                System.Array.Clear(pixels, 0, pixels.Length);
                Draw();
            }
            if (e.Key == System.Windows.Input.Key.Left)
            {
                complexPower -= 0.05;
                System.Array.Clear(pixels, 0, pixels.Length);
                Draw();
            }
        }

        private void Draw()
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    //a = x / масштаб * 1.777f(константа 16:9 для правильной отрисовки)
                    double a = (double)(x - (Width / 2)) / (double)(Width / 4) * 1.777f;

                    //a = y / масштаб
                    double b = (double)(y - (Height / 2)) / (double)(Height / 4);

                    Complex c = new Complex(a, b);
                    Complex z = new Complex(0, 0);

                    int iterations = 0;
                    do
                    {
                        iterations++;
                        z = Complex.Pow(z, complexPower) + c; // z = z ^ complexPower + c

                        double P = System.Math.Sqrt(System.Math.Pow(z.Real - 1d / 4d, 2d) + z.Imaginary * z.Imaginary);
                        double O = System.Math.Atan2(z.Imaginary, z.Real - 1d / 4d);
                        double Pc = 1d / 2d - 1d / 2d * System.Math.Cos(O);

                        if (iterations < 2.0d && (P <= Pc && complexPower == 2))
                            break;

                        if (z.Magnitude > 2d)
                        {
                            //Поиск индексов байтов какого-либо пикселя bitmap
                            int pixelOffset = (x + y * wb.PixelWidth) * wb.Format.BitsPerPixel / 8;
                            pixels[pixelOffset] = (byte)(System.Math.Abs(255 - iterations * 8 * 1.25));
                            pixels[pixelOffset + 1] = (byte)(System.Math.Abs(255 - iterations * 8 * 1.5));
                            pixels[pixelOffset + 2] = (byte)(System.Math.Abs(255 - iterations * 8 * 1.75));
                            break;
                        }
                    } while (iterations < 100);
                }
            }
            wb.FromByteArray(pixels);
            img.Source = wb;
        }
    }
}