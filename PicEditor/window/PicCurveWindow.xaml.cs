using PicEditor.controller;
using PicEditor.core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PicEditor.window
{
    /// <summary>
    /// PicCurveWindow.xaml 的交互逻辑
    /// </summary>
    public partial class PicCurveWindow : Window
    {
        private readonly BitmapSource bs;
        private readonly PicCurveControl picCurveControl = new PicCurveControl();
        private string elli = "NULL";
        private readonly int[][] data = new int[4][];
        private readonly int[] max = new int[4];
        public PicCurveWindow(BitmapSource bs)
        {
            InitializeComponent();
            this.bs = bs;
            for (int i = 0; i < 4; i++)
                data[i] = new int[256];
            Content.DataContext = picCurveControl;
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ChartPath.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.ApplicationIdle, (Action)(() =>
            {
                core.PicProcess picProcess = new core.PicProcess();
                picProcess.GetPixelCount(bs, data[1], data[2], data[3]);
                for (int i = 0; i < 256; i++)
                {
                    data[0][i] = data[1][i] + data[2][i] + data[3][i];
                    max[0] = data[0][i] > max[0] ? data[0][i] : max[0];
                    max[1] = data[1][i] > max[1] ? data[1][i] : max[1];
                    max[2] = data[2][i] > max[2] ? data[2][i] : max[2];
                    max[3] = data[3][i] > max[3] ? data[3][i] : max[3];
                }
                Channel.SelectedIndex = 0;
            }));
        }
        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && elli == "NULL")
                this.DragMove();
        }

        private void Close(object sender, RoutedEventArgs e)
        {
            Cancel();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ChartPath.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.ApplicationIdle, (Action)(() =>
            {
                int ChartWidth = 255;
                int ChartHeight = 255;
                string paths = "M 0," + ChartHeight + " ";
                double WidthOffset = (double)ChartWidth / 256;
                for (int i = 0; i < 256; i++)
                {
                    int y = ChartHeight - ChartHeight * data[Channel.SelectedIndex][i] / max[Channel.SelectedIndex];
                    paths += (i * WidthOffset).ToString() + "," + y.ToString() + " ";
                }
                paths += ChartWidth + "," + ChartHeight + " Z";
                var converter = System.ComponentModel.TypeDescriptor.GetConverter(typeof(Geometry));
                ChartPath.Data = (Geometry)converter.ConvertFrom(paths);
                if (Channel.SelectedIndex == 0)
                    TopColor.Color = Colors.White;
                else if (Channel.SelectedIndex == 1)
                    TopColor.Color = Color.FromRgb(255, 0, 0);
                else if (Channel.SelectedIndex == 2)
                    TopColor.Color = Color.FromRgb(0, 255, 0);
                else if (Channel.SelectedIndex == 3)
                    TopColor.Color = Color.FromRgb(0, 0, 255);
                picCurveControl.ChangeChannel(Channel.SelectedIndex);
            }));
        }

        private void Confirm(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = this.Owner as MainWindow;
            mainWindow.PicColorCallback(null, "曲线");
            this.Close();
        }

        private void Cancel(object sender, RoutedEventArgs e)
        {
            Cancel();
        }
        private void Cancel()
        {
            MainWindow mainWindow = this.Owner as MainWindow;
            mainWindow.PicColorCallback();
            this.Close();
        }
        private void Reset(object sender, RoutedEventArgs e)
        {
            picCurveControl.Reset();
            MainWindow mainWindow = this.Owner as MainWindow;
            mainWindow.PicColorCallback();
        }

        private void ResetChannel(object sender, RoutedEventArgs e)
        {
            int[][] path_data = picCurveControl.ResetChannel(Channel.SelectedIndex);
            PicProcess picProcess = new PicProcess();
            MainWindow mainWindow = this.Owner as MainWindow;
            picCurveControl.Visi = Visibility.Visible;
            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background, (Action)(() =>
            {
                mainWindow.PicColorCallback(picProcess.PicCurve(bs, path_data));
                picCurveControl.Visi = Visibility.Collapsed;
            }));
        }
        private void StackPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (elli == "NULL")
                    return;
                Point p = e.GetPosition(Chart);
                switch (elli)
                {
                    case "Y1":
                        picCurveControl.Y1 = p.Y; break;
                    case "Y2":
                        picCurveControl.Y2 = p.Y; break;
                    case "Y3":
                        picCurveControl.Y3 = p.Y; break;
                    case "Y4":
                        picCurveControl.Y4 = p.Y; break;
                    case "Y5":
                        picCurveControl.Y5 = p.Y; break;
                    case "Y6":
                        picCurveControl.Y6 = p.Y; break;
                    case "Y7":
                        picCurveControl.Y7 = p.Y; break;
                    case "Y8":
                        picCurveControl.Y8 = p.Y; break;
                    case "Y9":
                        picCurveControl.Y9 = p.Y; break;
                }
                picCurveControl.UpdatePath(Channel.SelectedIndex);
            }
            else if (e.LeftButton == MouseButtonState.Released)
            {
                if (elli != "NULL")
                {
                    elli = "NULL";
                    Start();
                }
            }
        }
        
        private void Elli_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            elli = (sender as Ellipse).Name;
        }

        private void Start()
        {
            MainWindow mainWindow = this.Owner as MainWindow;
            picCurveControl.Visi = Visibility.Visible;
            int[][] path_data = picCurveControl.GetPathData();
            mainWindow.PicCurve(bs, this, path_data);
        }
        public void Back()
        {
            picCurveControl.Visi = Visibility.Collapsed;
        }
    }
}
