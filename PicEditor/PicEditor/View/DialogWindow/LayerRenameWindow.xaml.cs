using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PicEditor.View.DialogWindow
{
    internal partial class LayerRenameWindow : Window
    {
        public LayerRenameWindow(string defaultText)
        {
            InitializeComponent();
            Owner = Application.Current.MainWindow;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            ResizeMode = ResizeMode.NoResize;
            textBox.Text = defaultText;
        }

        public string GetResult()
        {
            return textBox.Text;
        }

        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            string name = GetResult();
            if (string.IsNullOrWhiteSpace(name))
            {
                tipText.Text = "名称不能为空";
                return;
            }
            if (name.Length > 20)
            {
                tipText.Text = "名称的长度不能超过20个字符";
                return;
            }
            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
