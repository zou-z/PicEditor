using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Windows.Media.Imaging;
/// <summary>
/// PicView内控件后台
/// </summary>
namespace PicEditor.controller
{
    public class PicControl : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public bool CanBeyondArea = false;
        private readonly System.Windows.Controls.Canvas chart = null;
        public BitmapSource LastPic = null;

        private BitmapSource pic = null;
        private double scale;
        private int width;
        private int height;
        private string picOrigin;
        private string picName;
        private string picSize;
        private string picFormat;
        private double picDpiX;
        private double picDpiY;
        private double thickness;
        private double squareSize;
        private double squareMargin;
        private double rectLeft;
        private double rectTop;
        private double rectWidth;
        private double rectHeight;
        private System.Windows.Visibility visible;
        private double realWidth;
        private double realHeight;
        private double elliSize;
        private System.Windows.Thickness elliMargin;
        private double pointX1;
        private double pointY1;
        private double pointX2;
        private double pointY2;
        private double rulerHorizontalLen;
        private double rulerVerticalLen;
        private double rulerLen;
        private double rulerHorizontalAngle;
        private double rulerVerticalAngle;

        public BitmapSource Pic
        {
            get { return pic; }
            set
            {
                pic = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Pic"));
                Width = pic.PixelWidth;
                Height = pic.PixelHeight;
                PicSize = pic.PixelWidth + "×" + pic.PixelHeight + " (像素)";
                PicDpiX = pic.DpiX;
                PicDpiY = pic.DpiY;
                chart.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.ApplicationIdle, (Action)(() => { GetHistogram(200, 120); }));
            }
        }
        public double Scale
        {
            get { return scale; }
            set
            {
                if (scale != value)
                {
                    scale = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Scale"));
                }
                Thickness = 1 / scale;
                SquareSize = 8 / scale;
                ElliSize = 16 / scale;
                RealWidth = scale * width;
                RealHeight = scale * height;
            }
        }
        public int Width
        {
            get { return width; }
            set
            {
                width = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Width"));
            }
        }
        public int Height
        {
            get { return height; }
            set
            {
                height = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Height"));
            }
        }
        public string PicOrigin
        {
            get { return picOrigin; }
            set
            {
                picOrigin = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PicOrigin"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PicOrigin_Split"));
            }
        }
        public string PicName
        {
            get { return picName; }
            set
            {
                picName = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PicName"));
            }
        }
        public string PicSize
        {
            get { return picSize; }
            set
            {
                picSize = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PicSize"));
            }
        }
        public string PicFormat
        {
            get { return picFormat; }
            set
            {
                picFormat = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PicFormat"));
            }
        }
        public double PicDpiX
        {
            get { return picDpiX; }
            set
            {
                picDpiX = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PicDpiX"));
            }
        }
        public double PicDpiY
        {
            get { return picDpiY; }
            set
            {
                picDpiY = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PicDpiY"));
            }
        }
        public double Thickness
        {
            get { return thickness; }
            set
            {
                thickness = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Thickness"));
            }
        }
        public double SquareSize
        {
            get { return squareSize; }
            set
            {
                squareSize = value;
                SquareMargin = -value / 2;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SquareSize"));
            }
        }
        public double SquareMargin
        {
            get { return squareMargin; }
            set
            {
                squareMargin = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SquareMargin"));
            }
        }
        public double RectLeft
        {
            get { return rectLeft; }
            set
            {
                rectLeft = value;
                if (!CanBeyondArea)
                    rectLeft = rectLeft < 0 ? 0 : (rectLeft + rectWidth > width ? width - rectWidth : rectLeft);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("RectLeft"));
            }
        }
        public double RectTop
        {
            get { return rectTop; }
            set
            {
                rectTop = value;
                if (!CanBeyondArea)
                    rectTop = rectTop < 0 ? 0 : (rectTop + rectHeight > height ? height - rectHeight : rectTop);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("RectTop"));
            }
        }
        public double RectWidth
        {
            get { return rectWidth; }
            set
            {
                rectWidth = value;
                if (!CanBeyondArea)
                    rectWidth = rectWidth < 0 ? 0 : (rectLeft + rectWidth > width ? width - rectLeft : rectWidth);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("RectWidth"));
                Visible = rectWidth == 0 ? System.Windows.Visibility.Collapsed : System.Windows.Visibility.Visible;
            }
        }
        public double RectHeight
        {
            get { return rectHeight; }
            set
            {
                rectHeight = value;
                if (!CanBeyondArea)
                    rectHeight = rectHeight < 0 ? 0 : (rectTop + rectHeight > height ? height - rectTop : rectHeight);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("RectHeight"));
                Visible = rectHeight == 0 ? System.Windows.Visibility.Collapsed : System.Windows.Visibility.Visible;
            }
        }
        public System.Windows.Visibility Visible
        {
            get { return visible; }
            set
            {
                visible = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Visible"));
            }
        }
        public string PicOrigin_Split
        {
            get
            {
                return (picOrigin == null || picOrigin == "") ? "" : " - ";
            }
        }
        public double RealWidth
        {
            get { return realWidth; }
            set
            {
                realWidth = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("RealWidth"));
            }
        }
        public double RealHeight
        {
            get { return realHeight; }
            set
            {
                realHeight = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("RealHeight"));
            }
        }
        public double ElliSize
        {
            get { return elliSize; }
            set
            {
                elliSize = value;
                ElliMargin = new System.Windows.Thickness(-value / 2, -value / 2, 0, 0);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ElliSize"));
            }
        }
        public System.Windows.Thickness ElliMargin
        {
            get { return elliMargin; }
            set
            {
                elliMargin = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ElliMargin"));
            }
        }
        public double PointX1
        {
            get { return pointX1; }
            set
            {
                pointX1 = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PointX1"));
                RulerChanged();
            }
        }
        public double PointY1
        {
            get { return pointY1; }
            set
            {
                pointY1 = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PointY1"));
                RulerChanged();
            }
        }
        public double PointX2
        {
            get { return pointX2; }
            set
            {
                pointX2 = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PointX2"));
                RulerChanged();
            }
        }
        public double PointY2
        {
            get { return pointY2; }
            set
            {
                pointY2 = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PointY2"));
                RulerChanged();
            }
        }
        public double RulerHorizontalLen
        {
            get { return rulerHorizontalLen; }
            set
            {
                rulerHorizontalLen = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("RulerHorizontalLen"));
            }
        }
        public double RulerVerticalLen
        {
            get { return rulerVerticalLen; }
            set
            {
                rulerVerticalLen = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("RulerVerticalLen"));
            }
        }
        public double RulerLen
        {
            get { return rulerLen; }
            set
            {
                rulerLen = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("RulerLen"));
            }
        }
        public double RulerHorizontalAngle
        {
            get { return rulerHorizontalAngle; }
            set
            {
                rulerHorizontalAngle = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("RulerHorizontalAngle"));
            }
        }
        public double RulerVerticalAngle
        {
            get { return rulerVerticalAngle; }
            set
            {
                rulerVerticalAngle = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("RulerVerticalAngle"));
            }
        }
        public System.Windows.Media.Geometry RedPath { get; private set; }
        public System.Windows.Media.Geometry GreenPath { get; private set; }
        public System.Windows.Media.Geometry BluePath { get; private set; }

        public PicControl(System.Windows.Controls.Canvas chart)
        {
            Width = Height = 0;
            scale = 1;
            RectLeft = RectTop = RectWidth = RectHeight = 0;
            elliMargin = new System.Windows.Thickness(-8, -8, 0, 0);
            this.chart = chart;
        }
        private void RulerChanged()
        {
            RulerHorizontalLen = Math.Round(Math.Abs(pointX1 - pointX2), 2);
            RulerVerticalLen = Math.Round(Math.Abs(pointY1 - pointY2), 2);
            RulerLen = Math.Round(Math.Sqrt((pointX1 - pointX2) * (pointX1 - pointX2) + (pointY1 - pointY2) * (pointY1 - pointY2)), 2);
            RulerHorizontalAngle = rulerHorizontalLen == 0 ? 90 : Math.Round(Math.Atan(rulerVerticalLen / rulerHorizontalLen) * 180 / Math.PI, 2);
            RulerVerticalAngle = Math.Round(90 - RulerHorizontalAngle, 2);
        }
        /// <summary>
        /// 计算图像直方图
        /// </summary>
        /// <param name="ChartWidth">图的宽度</param>
        /// <param name="ChartHeight">图的高度</param>
        private void GetHistogram(int ChartWidth, int ChartHeight)
        {
            int[] red = new int[256];
            int[] green = new int[256];
            int[] blue = new int[256];
            double WidthOffset = (double)ChartWidth / 256;
            string reds = "M 0," + ChartHeight + " ";
            string greens = "M 0," + ChartHeight + " ";
            string blues = "M 0," + ChartHeight + " ";
            core.PicProcess picProcess = new core.PicProcess();
            int max = picProcess.GetPixelCount(pic, red, green, blue);
            for (int i = 0; i < 256;i++)
            {
                int y = ChartHeight - ChartHeight * red[i] / max;
                reds += (i * WidthOffset).ToString() + "," + y.ToString() + " ";
                y = ChartHeight - ChartHeight * green[i] / max;
                greens += (i * WidthOffset).ToString() + "," + y.ToString() + " ";
                y = ChartHeight - ChartHeight * blue[i] / max;
                blues += (i * WidthOffset).ToString() + "," + y.ToString() + " ";
            }
            reds += ChartWidth + "," + ChartHeight + " Z";
            greens += ChartWidth + "," + ChartHeight + " Z";
            blues += ChartWidth + "," + ChartHeight + " Z";
            var converter = TypeDescriptor.GetConverter(typeof(System.Windows.Media.Geometry));
            RedPath = (System.Windows.Media.Geometry)converter.ConvertFrom(reds);
            GreenPath = (System.Windows.Media.Geometry)converter.ConvertFrom(greens);
            BluePath = (System.Windows.Media.Geometry)converter.ConvertFrom(blues);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("RedPath"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("GreenPath"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("BluePath"));
        }
    }
}
