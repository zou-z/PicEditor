using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace PicEditor.controller
{
    class PicBACControl : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private double b;
        private double c;
        private System.Windows.Visibility visi = System.Windows.Visibility.Collapsed;

        public double B
        {
            get { return (int)b; }
            set
            {
                b = value < -127 ? -127 : (value > 127 ? 127 : value);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("B"));
            }
        }
        public double C
        {
            get { return (int)c; }
            set
            {
                c = value < -127 ? -127 : (value > 127 ? 127 : value);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("C"));
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
            b = c = 0;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("B"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("C"));
        }
    }
}
