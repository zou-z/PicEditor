using System.Windows;
using System.Windows.Controls;

namespace PicEditor.Basic.Control
{
    public partial class NumberTextBox : TextBox
    {
        public string Header
        {
            get => (string)GetValue(HeaderProperty);
            set => SetValue(HeaderProperty, value);
        }

        public NumberTextBox()
        {
            InitializeComponent();
        }


        private void Enlarge_Click(object sender, RoutedEventArgs e)
        {
            var index = SelectionStart;
            if (double.TryParse(Text, out double number))
            {
                Text = (number + 1).ToString();
                SelectionStart = index > Text.Length ? Text.Length : index;
            }
        }

        private void Reduce_Click(object sender, RoutedEventArgs e)
        {
            var index = SelectionStart;
            if (double.TryParse(Text, out double number))
            {
                Text = (number - 1).ToString();
                SelectionStart = index > Text.Length ? Text.Length : index;
            }
        }

        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register("Header", typeof(string), typeof(NumberTextBox), new PropertyMetadata("Header"));
    }
}
