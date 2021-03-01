using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace PicEditor.controller
{
    class PicMeanFilterControl : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private double radius = 0;
        private System.Windows.Visibility visi = System.Windows.Visibility.Collapsed;

        public double Radius
        {
            get { return (int)radius; }
            set
            {
                radius = value < 0 ? 0 : (value > 30 ? 30 : value);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Radius"));
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
