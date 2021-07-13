using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Threading;
using Un4seen.Bass;

namespace Analizer
{
    public partial class MainWindow : Window
    {
        int rectangleWidth = 1;
        int rectangleCount = 512;
        List<Rectangle> samples;
        string filePath;
        int stream;
        DispatcherTimer timer;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CreateTimer();

            //инициализация аудиопотока
            Bass.BASS_Init(-1, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero);

            CreateVisualizer();
        }

        private void CreateTimer()
        {
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(100000);

            float[] fft = new float[1024];

            //int count = 0;

            timer.Tick += (s, e) =>
            {
                //count++;
                //if (count == 1000)
                //{
                //    count++;
                //}

                Bass.BASS_ChannelGetData(stream, fft, (int)BASSData.BASS_DATA_FFT2048);

                //double avg;
                //double value;
                //int k = 1;
                //int[] indexes = new int[] { 0, 10, 25, 120, 240, 360, 450, 511};
                for (int i = 0; i < rectangleCount; i++)
                {
                    //samples[i].Height = fft[i] * 5000;

                    //samples[i].Height = Math.Sqrt(fft[i]) * 500;

                    #region
                    //double x = (Math.Atan(fft[i] * 50) / 1000) * Math.Atan(i) * 50000;
                    //double x = fft[i] * 1500 * (i / (rectangleCount / 4) + 1);
                    //double x = Math.Atan(fft[i] * 2) * 1500 * (i / (rectangleCount / 4) + 1);
                    double x = Math.Sqrt(fft[i]) * 500;
                    //double x = Math.Log(fft[i] + 1) * 100;

                    //if (i < 2)
                    //{
                    //    samples[i].Height = (x + samples[i + 1].Height + samples[i + 2].Height) / 5;
                    //}
                    //else if (i > rectangleCount - 3)
                    //{
                    //    samples[i].Height = (x + samples[i - 1].Height + samples[i - 2].Height) / 5;
                    //}
                    //else
                    //{
                    //    samples[i].Height = (x + samples[i - 1].Height + samples[i + 1].Height + samples[i - 2].Height + samples[i + 2].Height) / 5;
                    //}
                    #endregion

                    if (i < 2)
                    {
                        samples[i].Height = (x + samples[i + 1].Height + samples[i + 2].Height) / 3;
                    }
                    else if (i > rectangleCount - 3)
                    {
                        samples[i].Height = (x + samples[i - 1].Height + samples[i - 2].Height) / 3;
                    }
                    else
                    {
                        samples[i].Height = (x + samples[i - 1].Height + samples[i + 1].Height) / 3;
                    }

                    #region
                    //value = fft[i] * 100;

                    //for (int d = 1; d < indexes.Length; d++)
                    //{
                    //    if (i < indexes[d] && i > indexes[d - 1])
                    //    {
                    //        switch (d)
                    //        {
                    //            case 1:
                    //                k = 3;
                    //                break;
                    //            case 2:
                    //                k = 7;
                    //                break;
                    //            default:
                    //                k = (d - 1) * 10;
                    //                break;
                    //        }

                    //        break;
                    //    }
                    //}

                    //value *= k;

                    //if (i < 2)
                    //{
                    //    avg = (value + samples[i + 1].Height + samples[i + 2].Height) / 5;
                    //}
                    //else if (i > rectangleCount - 3)
                    //{
                    //    avg = (value + samples[i - 1].Height + samples[i - 2].Height) / 5;
                    //}
                    //else
                    //{
                    //    avg = (value + samples[i - 1].Height + samples[i + 1].Height + samples[i - 2].Height + samples[i + 2].Height) / 5;
                    //}

                    //samples[i].Height = avg;
                    #endregion

                    //if (i < 2)
                    //{
                    //    samples[i].Height = (fft[i] * 2000 + samples[i + 1].Height + samples[i + 2].Height) / 3;
                    //}
                    //else if (i > rectangleCount - 3)
                    //{
                    //    samples[i].Height = (fft[i] * 2000 + samples[i - 1].Height + samples[i - 2].Height) / 3;
                    //}
                    //else
                    //{
                    //    samples[i].Height = (fft[i] * 2000 + samples[i - 1].Height + samples[i + 1].Height + samples[i - 2].Height + samples[i + 2].Height) / 5;
                    //}

                    //samples[i].Height = (Math.Atan(fft[i] * 200) / 200) * Math.Atan(i * 100) * 10000;
                }
            };
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

        private void RunFile()
        {
            if (stream != 0)
            {
                Bass.BASS_StreamFree(stream);
            }

            stream = Bass.BASS_StreamCreateFile(filePath, 0, 0, BASSFlag.BASS_DEFAULT);

            if (stream != 0)
            {
                Bass.BASS_ChannelPlay(stream, false);
            }
            else
            {
                return;
            }

            timer.Stop();
            timer.Start();
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

        private void btnOpenFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "music |*.mp3";
            dialog.ShowDialog();
            filePath = dialog.FileName;
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            if (filePath.Length > 0)
            {
                RunFile();
            }
        }

        private void btnPause_Click(object sender, RoutedEventArgs e)
        {
            if (Bass.BASS_ChannelIsActive(stream) == BASSActive.BASS_ACTIVE_PLAYING)
            {
                Bass.BASS_ChannelPause(stream);
                timer.Stop();
            }
            else if (Bass.BASS_ChannelIsActive(stream) == BASSActive.BASS_ACTIVE_PAUSED)
            {
                Bass.BASS_ChannelPlay(stream, false);
                timer.Start();
            }
        }
    }
}
