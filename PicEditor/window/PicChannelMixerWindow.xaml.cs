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
    /// PicChannelMixerWindow.xaml 的交互逻辑
    /// </summary>
    public partial class PicChannelMixerWindow : Window
    {
        private readonly BitmapSource bs;
        private readonly PicChannelMixerControl picChannelMixerControl = new PicChannelMixerControl();
        public PicChannelMixerWindow(BitmapSource bs)
        {
            InitializeComponent();
            this.bs = bs;
            Content.DataContext = picChannelMixerControl;
            R.AddHandler(Slider.MouseLeftButtonUpEvent, new MouseButtonEventHandler(Slider_MouseLeftButtonUp), true);
            Rg.AddHandler(Slider.MouseLeftButtonUpEvent, new MouseButtonEventHandler(Slider_MouseLeftButtonUp), true);
            Rb.AddHandler(Slider.MouseLeftButtonUpEvent, new MouseButtonEventHandler(Slider_MouseLeftButtonUp), true);
            Gr.AddHandler(Slider.MouseLeftButtonUpEvent, new MouseButtonEventHandler(Slider_MouseLeftButtonUp), true);
            G.AddHandler(Slider.MouseLeftButtonUpEvent, new MouseButtonEventHandler(Slider_MouseLeftButtonUp), true);
            Gb.AddHandler(Slider.MouseLeftButtonUpEvent, new MouseButtonEventHandler(Slider_MouseLeftButtonUp), true);
            Br.AddHandler(Slider.MouseLeftButtonUpEvent, new MouseButtonEventHandler(Slider_MouseLeftButtonUp), true);
            Bg.AddHandler(Slider.MouseLeftButtonUpEvent, new MouseButtonEventHandler(Slider_MouseLeftButtonUp), true);
            B.AddHandler(Slider.MouseLeftButtonUpEvent, new MouseButtonEventHandler(Slider_MouseLeftButtonUp), true);
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

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                Start();
        }

        private void Confirm(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = this.Owner as MainWindow;
            mainWindow.PicColorCallback(null, "通道混合器");
            this.Close();
        }

        private void Cancel(object sender, RoutedEventArgs e)
        {
            Cancel();
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
        private void Reset(object sender, RoutedEventArgs e)
        {
            picChannelMixerControl.Reset();
            MainWindow mainWindow = this.Owner as MainWindow;
            mainWindow.PicColorCallback();
        }
        private void Start()
        {
            MainWindow mainWindow = this.Owner as MainWindow;
            double[] red = new double[] { picChannelMixerControl.R, picChannelMixerControl.Rg, picChannelMixerControl.Rb };
            double[] green = new double[] { picChannelMixerControl.Gr, picChannelMixerControl.G, picChannelMixerControl.Gb };
            double[] blue = new double[] { picChannelMixerControl.Br, picChannelMixerControl.Bg, picChannelMixerControl.B };
            picChannelMixerControl.Visi = Visibility.Visible;
            mainWindow.PicChannelMixer(bs, this, red, green, blue);
        }
        public void Back()
        {
            picChannelMixerControl.Visi = Visibility.Collapsed;
        }
    }
}
