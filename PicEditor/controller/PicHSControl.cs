using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace PicEditor.controller
{
    class PicHSControl : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private double h = 0, s = 0, v = 0;
        private System.Windows.Visibility visi = System.Windows.Visibility.Collapsed;

        public double H
        {
            get { return (int)h; }
            set
            {
                h = value < -180 ? -180 : (value > 180 ? 180 : value);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("H"));
            }
        }
        public double S
        {
            get { return (int)s; }
            set
            {
                s = value < -100 ? -100 : (value > 100 ? 100 : value);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("S"));
            }
        }
        public double V
        {
            get { return (int)v; }
            set
            {
                v = value < -100 ? -100 : (value > 100 ? 100 : value);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("V"));
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
            h = s = v = 0;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("H"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("S"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("V"));
        }
    }
}
