using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace PicEditor.controller
{
    class PicSharpenControl : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private double sharpen = 0;
        private System.Windows.Visibility visi = System.Windows.Visibility.Collapsed;

        public double Sharpen
        {
            get { return (int)sharpen; }
            set
            {
                sharpen = value < 0 ? 0 : (value > 100 ? 100 : value);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Sharpen"));
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
