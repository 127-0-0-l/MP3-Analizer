using Microsoft.Win32;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Analizer
{
    public partial class MainWindow : Window
    {
        int rectangleWidth = 1;
        int rectangleCount = 512;
        List<Rectangle> samples;
        WaveIn waveIn = new WaveIn();

        public MainWindow()
        {
            InitializeComponent();
        }

        [DllImport("winmm.dll", EntryPoint = "mciSendStringA", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int mciSendString(string lpstrCommand, string lpstrReturnString, int uReturnLength, int hwndCallback);


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //mciSendString("open new Type waveaudio Alias recsound", "", 0, 0);
            //mciSendString("record recsound", "", 0, 0);
            //MouseLeftButtonDown += MainWindow_MouseLeftButtonDown;
            CreateVisualizer();

            waveIn.DataAvailable += waveIn_DataAvailable;
            waveIn.WaveFormat = new WaveFormat(rectangleCount * 5, 1);
            waveIn.StartRecording();
        }

        private void MainWindow_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            mciSendString("save recsound d:\\result.wav", "", 0, 0);
            mciSendString("close recsound ", "", 0, 0);
            Close();
        }

        void waveIn_DataAvailable(object sender, WaveInEventArgs e)
        {
            for (int i = 0; i < rectangleCount; i++)
            {
                
                double x = e.Buffer[i];
                samples[i].Height = x;
            }
        }

        private void CreateVisualizer()
        {
            samples = new List<Rectangle>(rectangleCount);
            grdVisualizer.Width = rectangleCount * (rectangleWidth + 1) + 1;

            for (int i = 0; i < rectangleCount; i++)
            {
                samples.Add(CreateRectangle(rectangleWidth, 1, i * (rectangleWidth + 1) + 1, 0));
            }

            foreach (var sample in samples)
            {
                grdVisualizer.Children.Add(sample);
            }
        }

        private Rectangle CreateRectangle(int width, int height, int left, int bottom)
        {
            Rectangle rct = new Rectangle();
            rct.VerticalAlignment = VerticalAlignment.Center;
            rct.HorizontalAlignment = HorizontalAlignment.Left;
            rct.Width = width;
            rct.Height = height;
            rct.Margin = new Thickness(left, 0, 0, bottom);
            rct.Fill = colorRectangle.Fill;
            return rct;
        }
    }
}
