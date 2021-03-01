using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace PicEditor.controller
{
    class PicChannelMixerControl : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private double r, rg, rb, gr, g, gb, br, bg, b;
        private System.Windows.Visibility visi = System.Windows.Visibility.Collapsed;

        public double R
        {
            get { return Math.Round(r,2); }
            set
            {
                r = value < -2 ? -2 : (value > 2? 2: value);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("R"));
            }
        }
        public double Rg
        {
            get { return Math.Round(rg, 2); }
            set
            {
                rg = value < -2 ? -2 : (value > 2 ? 2 : value);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Rg"));
            }
        }
        public double Rb
        {
            get { return Math.Round(rb, 2); }
            set
            {
                rb = value < -2 ? -2 : (value > 2 ? 2 : value);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Rb"));
            }
        }
        public double Gr
        {
            get { return Math.Round(gr, 2); }
            set
            {
                gr = value < -2 ? -2 : (value > 2 ? 2 : value);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Gr"));
            }
        }
        public double G
        {
            get { return Math.Round(g, 2); }
            set
            {
                g = value < -2 ? -2 : (value > 2 ? 2 : value);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("G"));
            }
        }
        public double Gb
        {
            get { return Math.Round(gb, 2); }
            set
            {
                gb = value < -2 ? -2 : (value > 2 ? 2 : value);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Gb"));
            }
        }
        public double Br
        {
            get { return Math.Round(br, 2); }
            set
            {
                br = value < -2 ? -2 : (value > 2 ? 2 : value);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Br"));
            }
        }
        public double Bg
        {
            get { return Math.Round(bg, 2); }
            set
            {
                bg = value < -2 ? -2 : (value > 2 ? 2 : value);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Bg"));
            }
        }
        public double B
        {
            get { return Math.Round(b, 2); }
            set
            {
                b = value < -2 ? -2 : (value > 2 ? 2 : value);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("B"));
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
        public PicChannelMixerControl()
        {
            Reset();
        }
        public void Reset()
        {
            R = G = B = 1;
            Rg = Rb = Gr = Gb = Br = Bg = 0;
        }
    }
}
