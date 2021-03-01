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
using System.Windows.Threading;

namespace PicEditor.window
{
    /// <summary>
    /// AboutWindow.xaml 的交互逻辑
    /// </summary>
    public partial class AboutWindow : Window
    {
        public AboutWindow()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            PicEditorVersion.Text = "v " + Application.ResourceAssembly.GetName().Version.ToString();
            OSInfo.Dispatcher.BeginInvoke(DispatcherPriority.Background, (Action)(() =>
            {
                try
                {
                    System.Diagnostics.Process process = new System.Diagnostics.Process();
                    process.StartInfo.FileName = "cmd";
                    process.StartInfo.Arguments = "/c ver";
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.RedirectStandardOutput = true;
                    process.StartInfo.CreateNoWindow = true;
                    process.Start();
                    string res = process.StandardOutput.ReadToEnd();
                    process.WaitForExit();
                    process.Close();
                    OSInfo.Text = "OS : Windows " + res.Substring(res.LastIndexOf(' ') + 1, res.LastIndexOf(']') - res.LastIndexOf(' ') - 1);
                }
                catch (Exception)
                {
                    OSInfo.Text = "OS : 获取失败";
                }
            }));
        }
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                this.Close();
        }
        private void Close(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void Path_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                this.DragMove();
        }
        private void GoToHomePage(object sender, MouseButtonEventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", (sender as TextBox).Text);
        }
    }
}
