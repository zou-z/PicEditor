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
    /// TSWindow.xaml 的交互逻辑
    /// </summary>
    public partial class PicTSWindow : Window
    {
        private readonly BitmapSource bs;
        private readonly PicTSControl picTSControl = new PicTSControl();
        public PicTSWindow(BitmapSource bs)
        {
            InitializeComponent();
            this.bs = bs;
            Content.DataContext = picTSControl;
            TNum.AddHandler(Slider.MouseLeftButtonUpEvent, new MouseButtonEventHandler(Slider_MouseLeftButtonUp), true);
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
            mainWindow.PicColorCallback(null, "色调分离");
            this.Close();
        }

        private void Cancel(object sender, RoutedEventArgs e)
        {
            Cancel();
        }

        private void Reset(object sender, RoutedEventArgs e)
        {
            picTSControl.Reset();
            MainWindow mainWindow = this.Owner as MainWindow;
            mainWindow.PicColorCallback();
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                Start();
        }
        private void Slider_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Start();
        }
        private void Cancel()
        {
            MainWindow mainWindow = this.Owner as MainWindow;
            mainWindow.PicColorCallback();
            this.Close();
        }
        private void Start()
        {
            MainWindow mainWindow = this.Owner as MainWindow;
            if (picTSControl.Num == 256)
            {
                mainWindow.PicColorCallback(bs);
                return;
            }
            picTSControl.Visi = Visibility.Visible;
            mainWindow.PicTS(bs, this, (int)picTSControl.Num);
        }
        public void Back()
        {
            picTSControl.Visi = Visibility.Collapsed;
        }
    }
}
