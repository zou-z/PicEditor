using PicEditor.controller;
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
    /// PicColorScaleWindow.xaml 的交互逻辑
    /// </summary>
    public partial class PicColorScaleWindow : Window
    {
        private readonly BitmapSource bs;
        private string pressed = "NULL";
        private readonly int[][] data = new int[4][];
        private readonly int[] max = new int[4];
        private readonly PCSControl pCSControl = new PCSControl();

        public PicColorScaleWindow(BitmapSource bs)
        {
            InitializeComponent();
            this.bs = bs;
            for (int i = 0; i < 4; i++)
                data[i] = new int[256];
            Content.DataContext = pCSControl;

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

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ChartPath.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.ApplicationIdle, (Action)(() =>
            {
                int ChartWidth = 300;
                int ChartHeight = 100;
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
                    RightColor.Color = Colors.White;
                else if (Channel.SelectedIndex == 1)
                    RightColor.Color = Color.FromRgb(255, 0, 0);
                else if (Channel.SelectedIndex == 2)
                    RightColor.Color = Color.FromRgb(0, 255, 0);
                else if (Channel.SelectedIndex == 3)
                    RightColor.Color = Color.FromRgb(0, 0, 255);
            }));
        }

        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && pressed == "NULL")
                this.DragMove();
        }

        private void Close(object sender, RoutedEventArgs e)
        {
            Cancel();
        }

        private void Path_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            pressed = (sender as Path).Name;
        }

        private void StackPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Point p = e.GetPosition(Chart);
                if (pressed == "IB")
                    pCSControl.IB = p.X;
                else if (pressed == "IG")
                    pCSControl.IG = p.X;
                else if (pressed == "IW")
                    pCSControl.IW = p.X;
                else if (pressed == "OB")
                    pCSControl.OB = p.X;
                else if (pressed == "OW")
                    pCSControl.OW = p.X;
            }
            else if (e.LeftButton == MouseButtonState.Released)
            {
                if (pressed != "NULL")
                {
                    pressed = "NULL";
                    Start();
                }
            }
        }
        private void Cancel()
        {
            MainWindow mainWindow = this.Owner as MainWindow;
            mainWindow.PicColorCallback();
            this.Close();
        }

        private void Confirm(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = this.Owner as MainWindow;
            mainWindow.PicColorCallback(null, "色阶");
            this.Close();
        }

        private void Cancel(object sender, RoutedEventArgs e)
        {
            Cancel();
        }

        private void Reset(object sender, RoutedEventArgs e)
        {
            pCSControl.Reset();
            MainWindow mainWindow = this.Owner as MainWindow;
            mainWindow.PicColorCallback();
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                Start();
        }

        private void Start()
        {
            MainWindow mainWindow = this.Owner as MainWindow;
            pCSControl.Visi = Visibility.Visible;
            double[] data = new double[5];
            pCSControl.GetData(data);
            mainWindow.PicColorScale(bs, this, Channel.SelectedIndex, data[0], data[1], data[2], data[3], data[4]);
        }
        public void Back()
        {
            pCSControl.Visi = Visibility.Collapsed;
        }
    }
}
