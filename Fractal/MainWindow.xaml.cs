﻿using CommunityToolkit.Diagnostics;
using ComputeSharp;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Memory;
using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TerraFX.Interop.Windows;
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

            Memory<ImageSharpBgra32> pixelMemory = new Memory<ImageSharpBgra32>(new ImageSharpBgra32[(int)Width * (int)Height * PixelFormats.Bgra32.BitsPerPixel / 8]);
            Span<Bgra32> span = MemoryMarshal.Cast<ImageSharpBgra32, Bgra32>(pixelMemory.Span);
            texture.CopyTo(span);

            //Оптимизировать
            byte[] PixelData = MemoryMarshal.AsBytes(span).ToArray();
            WriteableBitmap wb = new WriteableBitmap((int)Width, (int)Height, 96, 96,  PixelFormats.Bgr32, null);
            wb.WritePixels(new Int32Rect(0,0,(int)Width, (int)Height), PixelData, (wb.PixelWidth * wb.Format.BitsPerPixel) / 8, 0);
            img.Source = wb;
        }
    }
}