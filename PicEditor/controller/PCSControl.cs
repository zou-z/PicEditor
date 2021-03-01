using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace PicEditor.controller
{
    class PCSControl : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private double ib = 0, ig = 150, iw = 300, ob = 0, ow = 300;
        private int ibt = 0, iwt = 255, obt = 0, owt = 255;
        private double igt = 1.0;
        private System.Windows.Visibility visi = System.Windows.Visibility.Collapsed;

        public double IB
        {
            get { return ib; }
            set
            {
                ib = value < 0 ? 0 : (value > iw ? iw : value);
                ibt = (int)(255 * ib / 300);
                if (igt >= 1)
                    ig = ib + (10 - igt) / 18 * (iw - ib);
                else
                    ig = ib + (1.9 - igt) / 1.8 * (iw - ib);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IB"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IBT"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IG"));
            }
        }
        public double IG
        {
            get { return ig; }
            set
            {
                ig = value < ib ? ib : (value > iw ? iw : value);
                igt = (ig - ib) / (iw - ib);
                if (igt < 0.5)
                    igt = Math.Round(10 - 18 * igt, 2);
                else
                    igt = Math.Round(1.9 - 1.8 * igt, 2);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IG"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IGT"));
            }
        }
        public double IW
        {
            get { return iw; }
            set
            {
                iw = value > 300 ? 300 : (value < ib ? ib : value);
                iwt = (int)(255 * iw / 300);
                if (igt >= 1)
                    ig = ib + (10 - igt) / 18 * (iw - ib);
                else
                    ig = ib + (1.9 - igt) / 1.8 * (iw - ib);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IW"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IWT"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IG"));
            }
        }
        public double OB
        {
            get { return ob; }
            set
            {
                ob = value < 0 ? 0 : (value > ow ? ow : value);
                obt = (int)(255 * ob / 300);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("OB"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("OBT"));
            }
        }
        public double OW
        {
            get { return ow; }
            set
            {
                ow = value > 300 ? 300 : (value < ob ? ob : value);
                owt = (int)(255 * ow / 300);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("OW"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("OWT"));
            }
        }

        public int IBT
        {
            get { return ibt; }
            set
            {
                ibt = value < 0 ? 0 : (value > iwt ? iwt : value);
                ib = 300.0 * ibt / 255;
                if (igt >= 1)
                    ig = ib + (10 - igt) / 18 * (iw - ib);
                else
                    ig = ib + (1.9 - igt) / 1.8 * (iw - ib);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IBT"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IB"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IG"));
            }
        }
        public int IWT
        {
            get { return iwt; }
            set
            {
                iwt = value > 255 ? 255 : (value < ibt ? ibt : value);
                iw = 300.0 * iwt / 255;
                if (igt >= 1)
                    ig = ib + (10 - igt) / 18 * (iw - ib);
                else
                    ig = ib + (1.9 - igt) / 1.8 * (iw - ib);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IWT"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IW"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IG"));
            }
        }
        public int OBT
        {
            get { return obt; }
            set
            {
                obt = value < 0 ? 0 : (value > owt ? owt : value);
                ob = 300.0 * obt / 255;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("OBT"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("OB"));
            }
        }
        public int OWT
        {
            get { return owt; }
            set
            {
                owt = value > 255 ? 255 : (value < obt ? obt : value);
                ow = 300.0 * owt / 255;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("OWT"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("OW"));
            }
        }
        public double IGT
        {
            get { return igt; }
            set
            {
                igt = value < 0.1 ? 0.1 : (value > 10 ? 10 : value);
                if (igt >= 1)
                    ig = ib + (10 - igt) / 18 * (iw - ib);
                else
                    ig = ib + (1.9 - igt) / 1.8 * (iw - ib);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IGT"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IG"));
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
        public void GetData(double[] data)
        {
            data[0] = ibt;
            data[1] = igt;
            data[2] = iwt;
            data[3] = obt;
            data[4] = owt;
        }
        public void Reset()
        {
            ib = 0; ig = 150; iw = 300; ob = 0; ow = 300;
            ibt = 0; iwt = 255; obt = 0; owt = 255; igt = 1.0;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IB"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IG"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IW"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("OB"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("OW"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IBT"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IGT"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IWT"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("OBT"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("OWT"));
        }
    }
}
