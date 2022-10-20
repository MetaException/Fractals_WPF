using ComputeSharp;
using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ImageSharpBgra32 = SixLabors.ImageSharp.PixelFormats.Bgra32;

namespace Fractal
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

        //public float x1 = -0.5555998601048486f, x2 = -0.5418662845040672f, y1 = -0.49153696023859084f, y2 = -0.5052705358393723f;

        //public float x1 = -3.5f, x2 = 0.5f, y1 = -2f, y2 = 2f;

        private float2 bottomLeft = new float2(-3.5f, -2f);
        private float2 topRight = new float2(0.5f, 2f);

        public int maxIterations = 10000;

        private WriteableBitmap _wb;

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _wb = new WriteableBitmap((int)Width, (int)Height, 96, 96, PixelFormats.Bgr32, null);
            Draw();
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Left)
            {
                float dx = (topRight.X - bottomLeft.X) / (float)Width * 1.777f;
                topRight.X -= dx * 5f;
                bottomLeft.X -= dx * 5f;
                Draw();
            }
            if (e.Key == System.Windows.Input.Key.Right)
            {
                float dx = (topRight.X - bottomLeft.X) / (float)Width * 1.777f;
                topRight.X += dx * 5f;
                bottomLeft.X += dx * 5f;
                Draw();
            }
            if (e.Key == System.Windows.Input.Key.Up)
            {
                float dy = (topRight.Y - bottomLeft.Y) / (float)Height;
                topRight.Y -= dy * 5f;
                bottomLeft.Y -= dy * 5f;
                Draw();
            }
            if (e.Key == System.Windows.Input.Key.Down)
            {
                float dy = (topRight.Y - bottomLeft.Y) / (float)Height;
                topRight.Y += dy * 5f;
                bottomLeft.Y += dy * 5f;
                Draw();
            }
            if (e.Key == System.Windows.Input.Key.E)
            {
                float dx = (topRight.X - bottomLeft.X) / (float)Width * 1.777f;
                float dy = (topRight.Y - bottomLeft.Y) / (float)Height;
                bottomLeft.X += dx * 5f;
                bottomLeft.Y += dy * 5f;
                topRight.X -= dx * 5f;
                topRight.Y -= dy * 5f;
                Draw();
            }
            if (e.Key == System.Windows.Input.Key.Q)
            {
                float dx = (topRight.X - bottomLeft.X) / (float)Width * 1.777f;
                float dy = (topRight.Y - bottomLeft.Y) / (float)Height;
                bottomLeft.X -= dx * 5f;
                bottomLeft.Y -= dy * 5f;
                topRight.X += dx * 5f;
                topRight.Y += dy * 5f;
                Draw();
            }
        }

        private void Draw()
        {
            Memory<ImageSharpBgra32> pixelMemory = new Memory<ImageSharpBgra32>(new ImageSharpBgra32[(int)Width * (int)Height]);
            Span<Bgra32> span = MemoryMarshal.Cast<ImageSharpBgra32, Bgra32>(pixelMemory.Span);

            // Load a texture from a specified image, and decode it in the BGRA32 format
            using ReadWriteTexture2D<Bgra32, float4> texture = GraphicsDevice.GetDefault().AllocateReadWriteTexture2D<Bgra32, float4>(span, (int)Width, (int)Height);

            // Run our shader on the texture we just loaded
            GraphicsDevice.GetDefault().For(texture.Width, texture.Height, new DrawMandelbrotSet(texture, bottomLeft, topRight, maxIterations));
            texture.CopyTo(span);

            byte[] pixelData = MemoryMarshal.AsBytes(span).ToArray();
            img.Source = _wb.FromByteArray(pixelData);
        }
    }
}