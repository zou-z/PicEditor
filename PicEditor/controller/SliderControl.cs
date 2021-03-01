using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace PicEditor.controller
{
    class SliderControl : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private readonly System.Windows.Controls.ScrollViewer SViewer;
        private readonly int PicPadding;
        private bool IsWheel = false;
        private const double a= 0.093577; //函数y=ln(x-a)的系数a
        private readonly PicControl picControl;

        private double sliderValue; //存放Slider值
        private string picSliderText;
        public double SliderValue
        {
            get { return sliderValue; }
            set
            {
                //通过鼠标滚轮缩放图片，value为图片缩放倍数
                if (IsWheel)
                {
                    sliderValue = Math.Log(value - a);
                    picControl.Scale = value;
                    PicSliderText = (value * 100).ToString() + "%";
                    //因为定点缩放需要获取鼠标位置，因此不在此处设置SViewer的offset
                    IsWheel = false;
                }
                //通过拖动Slider缩放图片，value为Slider值
                else
                {
                    sliderValue = value;
                    double scale = picControl.Scale, offset_h = SViewer.HorizontalOffset, offset_v = SViewer.VerticalOffset;
                    picControl.Scale = Math.Pow(Math.E, value) + a;
                    PicSliderText = ((int)(picControl.Scale * 100)).ToString() + "%";
                    //以目前窗口内图像中心为缩放中心
                    if ((scale * picControl.Width + 2 * PicPadding <= SViewer.ActualWidth) && (picControl.Scale * picControl.Width + 2 * PicPadding > SViewer.ActualWidth))
                    {
                        //图像中心点像素横坐标：(offset1-padding+sv.width/2)*scale2/scale1 (纵坐标相同)，应用定点缩放x1*s-x2+padding即可计算出offset2
                        SViewer.ScrollToHorizontalOffset((picControl.Scale * picControl.Width - SViewer.ActualWidth) / 2 + PicPadding);
                    }
                    else
                    {
                        SViewer.ScrollToHorizontalOffset((offset_h - PicPadding + SViewer.ActualWidth / 2) * picControl.Scale / scale - SViewer.ActualWidth / 2 + PicPadding);
                    }
                    if ((scale * picControl.Height + 2 * PicPadding <= SViewer.ActualHeight) && (picControl.Scale * picControl.Height + 2 * PicPadding > SViewer.ActualHeight))
                    {
                        SViewer.ScrollToVerticalOffset((picControl.Scale * picControl.Height - SViewer.ActualHeight) / 2 + PicPadding);
                    }
                    else
                    {
                        SViewer.ScrollToVerticalOffset((offset_v - PicPadding + SViewer.ActualHeight / 2) * picControl.Scale / scale - SViewer.ActualHeight / 2 + PicPadding);
                    }
                }
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SliderValue"));
            }
        }
        public string PicSliderText
        {
            get { return picSliderText; }
            set
            {
                picSliderText = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PicSliderText"));
            }
        }

        public SliderControl(PicControl picControl,System.Windows.Controls.ScrollViewer SViewer, int PicPadding)
        {
            this.picControl = picControl;
            this.SViewer = SViewer;
            this.PicPadding = PicPadding;
            SliderValue = -0.098249;
        }
        public double GetScaleValue(bool increase)
        {
            double[] v = new double[] { 0.1, 0.25, 0.33, 0.5, 0.66, 0.75, 1, 1.25, 1.5, 2, 3, 4, 6, 8, 16, 32, 64, 128 };
            //标记为是用鼠标滚轮缩放图片
            IsWheel = true;
            for (int i = 0; i < v.Length; i++)
            {
                //if (picControl.Scale == v[i])
                if (Math.Abs(picControl.Scale - v[i]) < 0.01)
                {
                    if (increase)
                        return (i + 1 == v.Length) ? v[i] : v[i + 1];
                    else
                        return i == 0 ? v[0] : v[i - 1];
                }
                //找到第一个比scale大的数
                else if (v[i] > picControl.Scale)
                {
                    if (increase)
                        return v[i];
                    else
                        return v[i - 1];
                }
            }
            return picControl.Scale;
        }
    }
}
