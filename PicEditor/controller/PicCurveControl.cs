using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Media;

namespace PicEditor.controller
{
    class PicCurveControl : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private System.Windows.Visibility visi = System.Windows.Visibility.Collapsed;
        private readonly double[][] y = new double[4][];
        private readonly Geometry[] rgb = new Geometry[4];
        private int channel = 0;
        private readonly int[][] path_data = new int[4][];

        public Geometry RGBPath
        {
            get { return rgb[0]; }
            set
            {
                rgb[0] = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("RGBPath"));
            }
        }
        public Geometry RedPath
        {
            get { return rgb[1]; }
            set
            {
                rgb[1] = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("RedPath"));
            }
        }
        public Geometry GreenPath 
        {
            get { return rgb[2]; }
            set
            {
                rgb[2] = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("GreenPath"));
            }
        }
        public Geometry BluePath 
        {
            get { return rgb[3]; }
            set
            {
                rgb[3] = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("BluePath"));
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
        public double Y1 
        {
            get { return y[channel][0]; }
            set
            {
                y[channel][0] = value < 0 ? 0 : (value > 255 ? 255 : value);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Y1"));
            }
        }
        public double Y2
        {
            get { return y[channel][1]; }
            set
            {
                y[channel][1] = value < 0 ? 0 : (value > 255 ? 255 : value);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Y2"));
            }
        }
        public double Y3
        {
            get { return y[channel][2]; }
            set
            {
                y[channel][2] = value < 0 ? 0 : (value > 255 ? 255 : value);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Y3"));
            }
        }
        public double Y4
        {
            get { return y[channel][3]; }
            set
            {
                y[channel][3] = value < 0 ? 0 : (value > 255 ? 255 : value);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Y4"));
            }
        }
        public double Y5
        {
            get { return y[channel][4]; }
            set
            {
                y[channel][4] = value < 0 ? 0 : (value > 255 ? 255 : value);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Y5"));
            }
        }
        public double Y6
        {
            get { return y[channel][5]; }
            set
            {
                y[channel][5] = value < 0 ? 0 : (value > 255 ? 255 : value);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Y6"));
            }
        }
        public double Y7
        {
            get { return y[channel][6]; }
            set
            {
                y[channel][6] = value < 0 ? 0 : (value > 255 ? 255 : value);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Y7"));
            }
        }
        public double Y8
        {
            get { return y[channel][7]; }
            set
            {
                y[channel][7] = value < 0 ? 0 : (value > 255 ? 255 : value);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Y8"));
            }
        }
        public double Y9
        {
            get { return y[channel][8]; }
            set
            {
                y[channel][8] = value < 0 ? 0 : (value > 255 ? 255 : value);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Y9"));
            }
        }

        public PicCurveControl()
        {
            InitData();
        }
        public void UpdatePath(int n)
        {
            //string path = "M 0," + y[n][0] + " Q 32," + y[n][1] + " 64," + y[n][2] + " 96," + y[n][3] + " 128," + y[n][4] + " 160," + y[n][5] + " 192," + y[n][6] + " 224," + y[n][7] + " 255," + y[n][8];
            string path = "M 0," + y[n][0] + " C 0," + y[n][0] + " 32," + y[n][1] + " 64," + y[n][2] + " 96," + y[n][3] + " 128," + y[n][4] + " 160," + y[n][5] + " 192," + y[n][6] + " 224," + y[n][7] + " 255," + y[n][8];
            var converter = TypeDescriptor.GetConverter(typeof(Geometry));
            if (n == 0)
                RGBPath = (Geometry)converter.ConvertFrom(path);
            else if (n == 1)
                RedPath = (Geometry)converter.ConvertFrom(path);
            else if (n == 2)
                GreenPath = (Geometry)converter.ConvertFrom(path);
            else if (n == 3)
                BluePath = (Geometry)converter.ConvertFrom(path);
        }
        public void ChangeChannel(int n)
        {
            channel = n;
            Y1 = y[n][0];
            Y2 = y[n][1];
            Y3 = y[n][2];
            Y4 = y[n][3];
            Y5 = y[n][4];
            Y6 = y[n][5];
            Y7 = y[n][6];
            Y8 = y[n][7];
            Y9 = y[n][8];
        }
        public int[][] GetPathData()
        {
            var flatten = rgb[channel].GetFlattenedPathGeometry();
            //每个Path控件有且仅有一条完整的曲线所以直接取0，不需要遍历
            PathFigure pf = flatten.Figures[0];
            PathSegment ps = pf.Segments[0];
            PolyLineSegment se = ps as PolyLineSegment;
            for (int i = 0, j = 1; i < 256; i++)
            {
                for (; j < se.Points.Count; j++)
                {
                    if (se.Points[j].X == i)
                    {
                        path_data[channel][i] = 255 - (int)se.Points[j].Y;
                        break;
                    }
                    else if (se.Points[j].X > i)
                    {
                        double t1 = (i - se.Points[j - 1].X);
                        double t2 = (se.Points[j].Y - se.Points[j - 1].Y);
                        double t3 = (se.Points[j].X - se.Points[j - 1].X);
                        path_data[channel][i] = (int)(se.Points[j - 1].Y + t1 * t2 / t3);
                        path_data[channel][i] = path_data[channel][i] < 0 ? 255 : (path_data[channel][i] > 255 ? 0 : (255 - path_data[channel][i]));
                        break;
                    }
                }
            }
            int[][] data = new int[4][];
            for (int i = 0; i < 4; i++)
                data[i] = new int[256];
            for (int i = 0; i < 256; i++)
            {
                data[0][i] = path_data[1][path_data[0][i]];
                data[1][i] = path_data[2][path_data[0][i]];
                data[2][i] = path_data[3][path_data[0][i]];
            }
            return data;
        }
        public int[][] ResetChannel(int n)
        {
            Y1 = 255;
            Y2 = 224;
            Y3 = 192;
            Y4 = 160;
            Y5 = 128;
            Y6 = 96;
            Y7 = 64;
            Y8 = 32;
            Y9 = 0;
            for (int i = 0; i < 256; i++)
                path_data[n][i] = i;
            string path = "M 0,255 255,0";
            var converter = TypeDescriptor.GetConverter(typeof(Geometry));
            if (n == 0)
                RGBPath = (Geometry)converter.ConvertFrom(path);
            else if (n == 1)
                RedPath = (Geometry)converter.ConvertFrom(path);
            else if (n == 2)
                GreenPath = (Geometry)converter.ConvertFrom(path);
            else if (n == 3)
                BluePath = (Geometry)converter.ConvertFrom(path);
            int[][] data = new int[4][];
            for (int i = 0; i < 4; i++)
                data[i] = new int[256];
            for (int i = 0; i < 256; i++)
            {
                data[0][i] = path_data[1][path_data[0][i]];
                data[1][i] = path_data[2][path_data[0][i]];
                data[2][i] = path_data[3][path_data[0][i]];
            }
            return data;
        }
        public void Reset()
        {
            InitData();
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Y1"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Y2"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Y3"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Y4"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Y5"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Y6"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Y7"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Y8"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Y9"));
        }
        private void InitData()
        {
            for (int i = 0; i < 4; i++)
            {
                y[i] = new double[9];
                y[i][0] = 255;
                y[i][1] = 224;
                y[i][2] = 192;
                y[i][3] = 160;
                y[i][4] = 128;
                y[i][5] = 96;
                y[i][6] = 64;
                y[i][7] = 32;
                y[i][8] = 0;
                path_data[i] = new int[256];
                for (int j = 0; j < 256; j++)
                    path_data[i][j] = j;
            }
            string path = "M 0,255 255,0";
            var converter = TypeDescriptor.GetConverter(typeof(Geometry));
            RGBPath = RedPath = GreenPath = BluePath = (Geometry)converter.ConvertFrom(path);
        }
    }
}
