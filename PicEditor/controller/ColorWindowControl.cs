using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PicEditor.controller
{
    class ColorWindowControl : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private Bitmap ColorRectBitmap;
        private Bitmap ColorBarBitmap;
        private const int ColorRectWidth = 300;
        private const int ColorRectHeight = 300;
        private const int ColorBarWidth = 30;
        private const int ColorBarHeight = 300;

        private ImageSource colorRect;
        private ImageSource colorBar;
        private double top;
        private double x;
        private double y;
        private readonly int[] RGBA = new int[4];
        private string hex;
        private System.Windows.Media.Brush newColor;

        public ImageSource ColorRect
        {
            get { return colorRect; }
            set
            {
                colorRect = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ColorRect"));
            }
        }
        public ImageSource ColorBar
        {
            get { return colorBar; }
            set
            {
                colorBar = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ColorBar"));
            }
        }
        public double Top
        {
            get { return top; }
            set
            {
                top = value < 0 ? 0 : (value >= ColorBarHeight ? ColorBarHeight - 1 : value);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Top"));
                System.Drawing.Color color = ColorBarBitmap.GetPixel(0, (int)top);
                SetColorRectBg(color.R, color.G, color.B);
                color = ColorRectBitmap.GetPixel((int)x, (int)y);
                NewColor = new SolidColorBrush(System.Windows.Media.Color.FromRgb(color.R, color.G, color.B));
                RGBA[0] = color.R;RGBA[1] = color.G;RGBA[2] = color.B;RGBA[3] = color.A;
                hex = DecToHex(RGBA[3]) + DecToHex(RGBA[0]) + DecToHex(RGBA[1]) + DecToHex(RGBA[2]);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("R"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("G"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("B"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("A"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("HEX"));
            }
        }
        public double X
        {
            get { return x; }
            set
            {
                x = value < 0 ? 0 : (value >= ColorRectWidth ? ColorRectWidth - 1 : value);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("X"));
            }
        }
        public double Y
        {
            get { return y; }
            set
            {
                y = value < 0 ? 0 : (value >= ColorRectHeight ? ColorRectHeight - 1 : value);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Y"));
                System.Drawing.Color color = ColorRectBitmap.GetPixel((int)x, (int)y);
                NewColor = new SolidColorBrush(System.Windows.Media.Color.FromRgb(color.R, color.G, color.B));
                RGBA[0] = color.R; RGBA[1] = color.G; RGBA[2] = color.B; RGBA[3] = color.A;
                hex = DecToHex(RGBA[3]) + DecToHex(RGBA[0]) + DecToHex(RGBA[1]) + DecToHex(RGBA[2]);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("R"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("G"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("B"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("A"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("HEX"));
            }
        }
        public int R
        {
            get { return RGBA[0]; }
            set
            {
                RGBA[0] = value < 0 ? 0 : (value > 255 ? 255 : value);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("R"));
                SetColorRectAndColorBar();
                NewColor = new SolidColorBrush(System.Windows.Media.Color.FromArgb((byte)RGBA[3], (byte)RGBA[0], (byte)RGBA[1], (byte)RGBA[2]));
                hex = DecToHex(RGBA[3]) + DecToHex(RGBA[0]) + DecToHex(RGBA[1]) + DecToHex(RGBA[2]);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("HEX"));
            }
        }
        public int G
        {
            get { return RGBA[1]; }
            set
            {
                RGBA[1] = value < 0 ? 0 : (value > 255 ? 255 : value);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("G"));
                SetColorRectAndColorBar();
                NewColor = new SolidColorBrush(System.Windows.Media.Color.FromArgb((byte)RGBA[3], (byte)RGBA[0], (byte)RGBA[1], (byte)RGBA[2]));
                hex = DecToHex(RGBA[3]) + DecToHex(RGBA[0]) + DecToHex(RGBA[1]) + DecToHex(RGBA[2]);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("HEX"));
            }
        }
        public int B
        {
            get { return RGBA[2]; }
            set
            {
                RGBA[2] = value < 0 ? 0 : (value > 255 ? 255 : value);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("B"));
                SetColorRectAndColorBar();
                NewColor = new SolidColorBrush(System.Windows.Media.Color.FromArgb((byte)RGBA[3], (byte)RGBA[0], (byte)RGBA[1], (byte)RGBA[2]));
                hex = DecToHex(RGBA[3]) + DecToHex(RGBA[0]) + DecToHex(RGBA[1]) + DecToHex(RGBA[2]);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("HEX"));
            }
        }
        public int A
        {
            get { return RGBA[3]; }
            set
            {
                RGBA[3] = value < 0 ? 0 : (value > 255 ? 255 : value);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("A"));
                NewColor = new SolidColorBrush(System.Windows.Media.Color.FromArgb((byte)RGBA[3], (byte)RGBA[0], (byte)RGBA[1], (byte)RGBA[2]));
                hex = DecToHex(RGBA[3]) + DecToHex(RGBA[0]) + DecToHex(RGBA[1]) + DecToHex(RGBA[2]);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("HEX"));
            }
        }
        public string HEX
        {
            get { return hex; }
            set
            {
                if (value.Length < 8)
                {
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("HEX"));
                    return;
                }
                hex = value.Substring(0, 8).ToUpper();
                RGBA[0] = HexToDec(hex.Substring(2, 2).ToUpper());
                RGBA[1] = HexToDec(hex.Substring(4, 2).ToUpper());
                RGBA[2] = HexToDec(hex.Substring(6, 2).ToUpper());
                RGBA[3] = HexToDec(hex.Substring(0, 2).ToUpper());
                SetColorRectAndColorBar();
                NewColor = new SolidColorBrush(System.Windows.Media.Color.FromArgb((byte)RGBA[3], (byte)RGBA[0], (byte)RGBA[1], (byte)RGBA[2]));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("R"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("G"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("B"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("A"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("HEX"));
            }
        }
        public System.Windows.Media.Brush NewColor
        {
            get { return newColor; }
            set
            {
                newColor = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("NewColor"));
            }
        }

        public ColorWindowControl()
        {
            SetColorBarBg();
        }
        public void Init(string color)
        {
            HEX = color;
        }
        public void UpdateHex(string value)
        {
            HEX = value;
        }
        public void SetColorRectBg(int R=255,int G=0,int B=0)
        {
            if (R == 0 && G == 0 && B == 0)
                R = 255;
            ColorRectBitmap = new Bitmap(ColorRectWidth, ColorRectHeight);
            for (int x = 0; x < ColorRectWidth; x++)
            {
                int R0 = (int)(255 - x / (ColorRectWidth - 1.0) * (255 - R));
                int G0 = (int)(255 - x / (ColorRectWidth - 1.0) * (255 - G));
                int B0 = (int)(255 - x / (ColorRectWidth - 1.0) * (255 - B));
                for (int y = 0; y < ColorRectHeight; y++)
                {
                    double v = 1 - y / (ColorRectHeight - 1.0);
                    System.Drawing.Color color = System.Drawing.Color.FromArgb(255, (int)(R0 * v), (int)(G0 * v), (int)(B0 * v));
                    ColorRectBitmap.SetPixel(x, y, color);
                }
            }
            ColorRect = BitmapToBitmapSource(ColorRectBitmap);
        }
        private void SetColorBarBg()
        {
            ColorBarBitmap = new Bitmap(ColorBarWidth, ColorBarHeight);
            int per = ColorBarHeight / 6, R = 255, G = 0, B = 0;
            for (int y = 0; y < ColorBarHeight; y++)
            {
                int p = y / per;
                int offset = (int)((y % per) * 255.0 / per);
                switch (p)
                {
                    case 0:
                        R = 255; G = offset; B = 0; break;
                    case 1:
                        R = 255 - offset; G = 255; B = 0; break;
                    case 2:
                        R = 0; G = 255; B = offset; break;
                    case 3:
                        R = 0; G = 255 - offset; B = 255; break;
                    case 4:
                        R = offset; G = 0; B = 255; break;
                    case 5:
                        R = 255; G = 0; B = 255 - offset; break;
                    case 6:
                        R = 255; G = 0; B = 0; break;
                }
                for (int x = 0; x < ColorBarWidth; x++)
                {
                    System.Drawing.Color color = System.Drawing.Color.FromArgb(255, R, G, B);
                    ColorBarBitmap.SetPixel(x, y, color);
                }
            }
            ColorBar = BitmapToBitmapSource(ColorBarBitmap);
        }
        private void SetColorRectAndColorBar()
        {
            //ColorRect右上角像素值
            int[] color = new int[3]; 
            //获取RGB三通道的大小顺序信息
            int[] v = GetSortInfo();
            //根据最大值计算出ColorRect中y坐标
            y = ColorRectHeight * (1 - RGBA[v[0]] / 255.0);
            //根据最小值和y坐标计算出x坐标
            double temp = y == ColorRectHeight ? 0 : (RGBA[v[1]] / (1 - y / ColorRectHeight));
            x = (255 - temp) * ColorRectWidth / 255;
            //根据x,y坐标计算ColorRect右上角中间像素值,在设置ColorRect右上角像素值
            temp = y == ColorRectHeight ? 0 : (RGBA[v[2]] / (1 - y / ColorRectHeight));
            color[v[2]] = x == 0 ? 0 : ((int)(255 - (255 - temp) * ColorRectWidth / x));
            color[v[0]] = 255;
            color[v[1]] = 0;
            //更新ColorRect背景
            SetColorRectBg(color[0], color[1], color[2]);
            //根据ColorRect右上角像素值计算出ColorBar的top值
            temp = ColorBarHeight / 6.0 / 255 * color[v[2]];
            if (v[0] == 0 && v[1] == 2) top = temp;
            else if (v[0] == 1 && v[1] == 2) top = ColorBarHeight / 6.0 * 2 - temp;
            else if (v[0] == 1 && v[1] == 0) top = ColorBarHeight / 6.0 * 2 + temp;
            else if (v[0] == 2 && v[1] == 0) top = ColorBarHeight / 6.0 * 4 - temp;
            else if (v[0] == 2 && v[1] == 1) top = ColorBarHeight / 6.0 * 4 + temp;
            else if (v[0] == 0 && v[1] == 1) top = ColorBarHeight / 6.0 * 6 - temp;
            x = x < 0 ? 0 : (x >= ColorRectWidth ? ColorRectWidth - 1 : x);
            y = y < 0 ? 0 : (y >= ColorRectHeight ? ColorRectHeight - 1 : y);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("X"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Y"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Top"));
        }
        private BitmapSource BitmapToBitmapSource(Bitmap bitmap)
        {
            using MemoryStream ms = new MemoryStream();
            bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            BitmapFrame bf = BitmapFrame.Create(ms, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
            return bf;
        }
        private string DecToHex(int dec)
        {
            char[] num = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F' };
            string hex = "";
            while (dec > 0)
            {
                hex = num[dec % 16] + hex;
                dec /= 16;
            }
            return hex == "" ? "00" : (hex.Length == 1 ? "0" + hex : hex);
        }
        private int HexToDec(string hex)
        {
            char[] num = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F' };
            int dec = 0;
            for(int i = 0; i < 2; i++)
            {
                for(int j = 0; j < 16; j++)
                {
                    if (hex[i] == num[j])
                    {
                        dec += (int)Math.Pow(16,1-i) * j;
                        break;
                    }
                }
            }
            return dec;
        }
        private int[] GetSortInfo()
        {
            //存储的信息按顺序分别为：最大值下标、最小值下标、中间值下标
            int[] v = new int[3] { 0, 0, 0 };
            for(int i = 1; i < 3; i++)
            {
                v[0] = RGBA[v[0]] < RGBA[i] ? i : v[0];
                v[1] = RGBA[v[1]] > RGBA[i] ? i : v[1];
            }
            v[1] = v[0] == v[1] ? 1 : v[1]; //避免在三个值相同时出错
            v[2] = v[0] * v[1] != 0 ? 0 : (v[0] + v[1] == 1 ? 2 : 1);
            return v;
        }
    }
}
/**
    颜色窗口主要组成成分
    (1)ColorRect
    (2)ColorBar
    (3)ColorText
    影响关系
        ColorRect->ColorText     
        ColorBar->ColorRect
                ->ColorText
        ColorText->ColorRect
                 ->ColorBar
*/
