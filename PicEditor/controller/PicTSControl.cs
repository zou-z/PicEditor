using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace PicEditor.controller
{
    class PicTSControl : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private double num = 256;
        private System.Windows.Visibility visi = System.Windows.Visibility.Collapsed;

        public double Num
        {
            get { return (int)num; }
            set
            {
                num = value < 2 ? 2 : (value > 256 ? 256 : value);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Num"));
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
        public void Reset()
        {
            num = 256;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Num"));
        }
    }
}
