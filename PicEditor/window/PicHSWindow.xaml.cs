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
    /// PicHSWindow.xaml 的交互逻辑
    /// </summary>
    public partial class PicHSWindow : Window
    {
        private readonly BitmapSource bs;
        private readonly PicHSControl picHSControl = new PicHSControl();

        public PicHSWindow(BitmapSource bs)
        {
            InitializeComponent();
            this.bs = bs;
            Content.DataContext = picHSControl;
            Hue.AddHandler(Slider.MouseLeftButtonUpEvent, new MouseButtonEventHandler(Slider_MouseLeftButtonUp), true);
            Saturation.AddHandler(Slider.MouseLeftButtonUpEvent, new MouseButtonEventHandler(Slider_MouseLeftButtonUp), true);
            Value.AddHandler(Slider.MouseLeftButtonUpEvent, new MouseButtonEventHandler(Slider_MouseLeftButtonUp), true);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Mode.SelectedIndex = 0;
        }

        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                this.DragMove();
        }

        private void Close(object sender, RoutedEventArgs e)
        {
            Cancel();
        }

        private void Confirm(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = this.Owner as MainWindow;
            mainWindow.PicColorCallback(null, "色相/饱和度");
            this.Close();
        }

        private void Cancel(object sender, RoutedEventArgs e)
        {
            Cancel();
        }

        private void Reset(object sender, RoutedEventArgs e)
        {
            picHSControl.Reset();
            MainWindow mainWindow = this.Owner as MainWindow;
            mainWindow.PicColorCallback();
        }
        private void Cancel()
        {
            MainWindow mainWindow = this.Owner as MainWindow;
            mainWindow.PicColorCallback();
            this.Close();
        }
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            VName.Text = Mode.SelectedIndex == 0 ? "明度" : "亮度";
        }

        private void Slider_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Start();
        }
        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                Start();
        }

        private void Start()
        {
            MainWindow mainWindow = this.Owner as MainWindow;
            picHSControl.Visi = Visibility.Visible;
            mainWindow.PicHS(bs, this, Mode.SelectedIndex, picHSControl.H, picHSControl.S / 100, picHSControl.V / 100);
        }
        public void Back()
        {
            picHSControl.Visi = Visibility.Collapsed;
        }
    }
}
