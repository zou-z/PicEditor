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
using PicEditor.controller;
using PicEditor.core;

namespace PicEditor.window
{
    /// <summary>
    /// BACWindow.xaml 的交互逻辑
    /// </summary>
    public partial class PicBACWindow : Window
    {
        private readonly BitmapSource bs;
        private readonly PicBACControl picBACControl = new PicBACControl();

        public PicBACWindow(BitmapSource bs)
        {
            InitializeComponent();
            this.bs = bs;
            Content.DataContext = picBACControl;
            Brightness.AddHandler(Slider.MouseLeftButtonUpEvent, new MouseButtonEventHandler(Slider_MouseLeftButtonUp), true);
            Contrast.AddHandler(Slider.MouseLeftButtonUpEvent, new MouseButtonEventHandler(Slider_MouseLeftButtonUp), true);
        }

        private void Close(object sender, RoutedEventArgs e)
        {
            Cancel();
        }

        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                this.DragMove();
        }
        private void Confirm(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = this.Owner as MainWindow;
            mainWindow.PicColorCallback(null, "亮度/对比度");
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
   
        private void Slider_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Start();
        }

        private void Reset(object sender, RoutedEventArgs e)
        {
            picBACControl.Reset();
            MainWindow mainWindow = this.Owner as MainWindow;
            mainWindow.PicColorCallback();
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key==Key.Enter)
                Start();
        }

        private void Start()
        {
            MainWindow mainWindow = this.Owner as MainWindow;
            picBACControl.Visi = Visibility.Visible;
            mainWindow.PicBAC(bs, this, picBACControl.B, picBACControl.C);
        }

        public void Back()
        {
            picBACControl.Visi = Visibility.Collapsed;
        }
    }
}
