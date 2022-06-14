using PicEditor.ViewModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PicEditor.View.Panel
{
    internal partial class InsertPropertyPanel : UserControl
    {
        public double RealWidth
        {
            get => (double)GetValue(RealWidthProperty);
            set => SetValue(RealWidthProperty, value);
        }

        public double RealHeight
        {
            get => (double)GetValue(RealHeightProperty);
            set => SetValue(RealHeightProperty, value);
        }

        public double WhRatio => (double)GetValue(WhRatioProperty);

        public InsertPropertyPanel()
        {
            InitializeComponent();
            SetBinding(RealWidthProperty, new Binding("Position.RealWidth") { Mode = BindingMode.TwoWay });
            SetBinding(RealHeightProperty, new Binding("Position.RealHeight") { Mode = BindingMode.TwoWay });
            SetBinding(WhRatioProperty, new Binding("Position.WhRatio") { Mode = BindingMode.OneWay });
        }

        private bool isSettingSize = false;

        private void Width_Changed(object sender, TextChangedEventArgs e)
        {
            if (isSettingSize)
            {
                return;
            }
            if (double.TryParse(widthTextBox.Text, out double number))
            {
                RealWidth = number;
                if (isKeepRatio.IsChecked == true)
                {
                    RealHeight = number / WhRatio;
                }
                widthTextBox.Tag = null;
            }
            else
            {
                widthTextBox.Tag = "Error";
            }
        }

        private void Height_Changed(object sender, TextChangedEventArgs e)
        {
            if (isSettingSize)
            {
                return;
            }
            if (double.TryParse(heightTextBox.Text, out double number))
            {
                RealHeight = number;
                if (isKeepRatio.IsChecked == true)
                {
                    RealWidth = number * WhRatio;
                }
                heightTextBox.Tag = null;
            }
            else
            {
                heightTextBox.Tag = "Error";
            }
        }

        private static void RealWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is InsertPropertyPanel self && self != null)
            {
                self.isSettingSize = true;
                self.widthTextBox.Text = ((double)e.NewValue).ToString();
                self.isSettingSize = false;
            }
        }

        private static void RealHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is InsertPropertyPanel self && self != null)
            {
                self.isSettingSize = true;
                self.heightTextBox.Text = ((double)e.NewValue).ToString();
                self.isSettingSize = false;
            }
        }

        public static readonly DependencyProperty RealWidthProperty = DependencyProperty.Register("RealWidth", typeof(double), typeof(InsertPropertyPanel), new PropertyMetadata(0d, new PropertyChangedCallback(RealWidthChanged)));
        public static readonly DependencyProperty RealHeightProperty = DependencyProperty.Register("RealHeight", typeof(double), typeof(InsertPropertyPanel), new PropertyMetadata(0d, new PropertyChangedCallback(RealHeightChanged)));
        public static readonly DependencyProperty WhRatioProperty = DependencyProperty.Register("WhRatio", typeof(double), typeof(InsertPropertyPanel), new PropertyMetadata(0d));
    }
}
