using PicEditor.Model.EditMode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace PicEditor.View.Panel
{
    internal class PropertyPanel : Border
    {
        private Border? viewProperty = null;

        public EditMode SelectedMode
        {
            get => (EditMode)GetValue(SelectedModeProperty);
            set => SetValue(SelectedModeProperty, value);
        }

        public PropertyPanel()
        {
        }

        private static void SelectedModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PropertyPanel self && self != null)
            {
                EditMode mode = (EditMode)e.NewValue;
                if (mode == EditMode.View)
                {
                    if (self.viewProperty == null)
                    {
                        self.viewProperty = new Border();
                    }
                    self.Child = self.viewProperty;
                }
            }
        }

        public static readonly DependencyProperty SelectedModeProperty = DependencyProperty.Register("SelectedMode", typeof(EditMode), typeof(PropertyPanel), new PropertyMetadata(EditMode.View, new PropertyChangedCallback(SelectedModeChanged)));
    }
}
