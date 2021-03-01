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
    /// PicResizeWindow.xaml 的交互逻辑
    /// </summary>
    public partial class PicResizeWindow : Window
    {
        private int width;
        private int height;
        public PicResizeWindow(int width,int height)
        {
            InitializeComponent();
            this.width = width;
            this.height = height;
            W.Text = width.ToString();
            H.Text = height.ToString();
        }
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Confirm();
            }
            else if (e.Key == Key.Escape)
            {
                this.Close();
            }
        }

        private void Close(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Confirm(object sender, RoutedEventArgs e)
        {
            Confirm();
        }
        private void Confirm()
        {
            if (W.Tag.ToString() == "" && H.Tag.ToString() == "")
            {
                this.Close();
                MainWindow mainWindow = this.Owner as MainWindow;
                mainWindow.PicResize(int.Parse(W.Text), int.Parse(H.Text));
            }
        }
        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                this.DragMove();
        }
        private void ToInt(object sender, TextChangedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            try
            {
                tb.Background = new SolidColorBrush(Color.FromRgb(48, 48, 48));
                int i = int.Parse(tb.Text);
                if (i <= 0)
                {
                    tb.Background = new SolidColorBrush(Colors.Red);
                    tb.Tag = "Error";
                    return;
                }
                tb.Tag = "";
            }
            catch (Exception)
            {
                tb.Background = new SolidColorBrush(Colors.Red);
                tb.Tag = "Error";
            }
        }
        private void CalcProportion(object sender, RoutedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (tb.Tag.ToString() == "" && KeepProportion.IsChecked == true)
            {
                int i = int.Parse(tb.Text);
                if (tb.Name == "W")
                    H.Text = (i * height / width).ToString();
                else if (tb.Name == "H")
                    W.Text = (i * width / height).ToString();
            }
        }
    }
}
