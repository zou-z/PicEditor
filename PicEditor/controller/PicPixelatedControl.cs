using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace PicEditor.controller
{
    class PicPixelatedControl : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private double size = 1;
        private System.Windows.Visibility visi = System.Windows.Visibility.Collapsed;

        public double Size
        {
            get { return (int)size; }
            set
            {
                size = value < 0 ? 0 : (value > 500 ? 500 : value);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Size"));
            }
        }
        public System.Windows.Visibility Visi
        {
            get { return visi; }
            set
            {
                visi = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Visi"));
            }
        }
    }
}
