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

namespace PicEditor.window
{
    /// <summary>
    /// ColorWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ColorWindow : Window
    {
        private readonly string title;
        private string pressed_sign = "NULL";
        private string target;
        private readonly ColorWindowControl colorWindowControl = new ColorWindowControl();

        public ColorWindow(string title, Brush origin_color,string target)
        {
            InitializeComponent();
            this.title = title;
            colorWindowControl.NewColor = OldColor.Fill = origin_color;
            this.target = target;
        }
        
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Title = ColorWinTitle.Text = title;
            WindowContent.DataContext = colorWindowControl;
            colorWindowControl.Init(OldColor.Fill.ToString().Substring(1, 8));
        }

        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && pressed_sign == "NULL")
                this.DragMove();
        }

        private void Close(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ColorBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point p = e.GetPosition(ColorBar);
            colorWindowControl.Top = (int)(p.Y - 0.5);
            if (pressed_sign == "NULL")
                pressed_sign = "ColorBar";
        }

        private void ColorRect_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point p = e.GetPosition(ColorRect);
            colorWindowControl.X = (int)p.X;
            colorWindowControl.Y = (int)p.Y;
            if (pressed_sign == "NULL")
                pressed_sign = "ColorRect";
        }

        private void WindowContent_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (pressed_sign == "ColorBar")
                {
                    Point p = e.GetPosition(ColorBar);
                    colorWindowControl.Top = (int)(p.Y - 0.5);
                }
                else if (pressed_sign == "ColorRect")
                {
                    Point p = e.GetPosition(ColorRect);
                    colorWindowControl.X = (int)p.X;
                    colorWindowControl.Y = (int)p.Y;
                }
            }
            else if (e.LeftButton == MouseButtonState.Released && pressed_sign != "NULL")
            {
                pressed_sign = "NULL";
            }
        }

        private void Rectangle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Rectangle rect = sender as Rectangle;
            colorWindowControl.HEX = rect.Fill.ToString().Substring(1, 8);
        }

        private void Confirm(object sender, RoutedEventArgs e)
        {
            this.Close();
            MainWindow mainWindow = this.Owner as MainWindow;
            Color color = Color.FromArgb((byte)colorWindowControl.A, (byte)colorWindowControl.R, (byte)colorWindowControl.G, (byte)colorWindowControl.B);
            if (target == "FgColor")
            {
                mainWindow.SetFgColor(color);
            }
            else if (target == "TextBgColor")
            {
                mainWindow.SetTextBgColor(color);
            }
            else if(target== "PicSynFillColor")
            {
                mainWindow.SetPicSynFillColor(color);
            }
        }
        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                colorWindowControl.UpdateHex((sender as TextBox).Text);
            }
        }
    }
}
