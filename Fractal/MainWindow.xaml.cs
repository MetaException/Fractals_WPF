using ComputeSharp;
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

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

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Draw();
        }

        private void Draw()
        {
            // Load a texture from a specified image, and decode it in the BGRA32 format
            using var texture = GraphicsDevice.GetDefault().AllocateReadWriteTexture2D<Bgra32, float4>((int)Width, (int)Height);

            // Run our shader on the texture we just loaded
            GraphicsDevice.GetDefault().For(texture.Width, texture.Height, new DrawMandelbrotSet(texture));

            // Save the processed image by overwriting the original image
            texture.Save("D:\\myImage.jpg");

            img.Source = new BitmapImage(new Uri("D:\\myImage.jpg")); //Переделать
        }
    }
}