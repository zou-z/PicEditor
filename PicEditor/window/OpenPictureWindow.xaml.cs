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
    /// OpenPictureWindow.xaml 的交互逻辑
    /// </summary>
    public partial class OpenPictureWindow : Window
    {
        private string target = null;
        public OpenPictureWindow(string target)
        {
            InitializeComponent();
            this.target = target;
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            URL.Focus();
            URL.Select(URL.Text.Length, 0);
        }
        public void SetValue(String url)
        {
            URL.Text = url;
            Tip.Text = "打开失败！此链接不是图片链接";
        }
        private void OpenPicture()
        {
            if (URL.Text == null || URL.Text == "")
            {
                Tip.Text = "链接不能为空!";
                return;
            }
            this.Close();
            MainWindow mainWindow = this.Owner as MainWindow;
            if (target == "OpenPictureByURL")
                mainWindow.OpenPictureByURL(URL.Text);
            else if(target== "AddSecPicByURL")
                mainWindow.AddSecPicByURL(URL.Text);
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenPicture();
        }
        private void URL_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                OpenPicture();
            else if (e.Key == Key.Escape)
                this.Close();
        }
    }
}
