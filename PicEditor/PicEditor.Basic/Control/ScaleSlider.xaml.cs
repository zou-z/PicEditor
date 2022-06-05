using System;
using System.Windows;
using System.Windows.Controls;

namespace PicEditor.Basic.Control
{
    /// <summary>
    /// 缩放倍数值滑动条
    /// [映射关系]
    /// (1) 参数
    ///     A: maxScale
    ///     B: minScale
    ///     C: minValue
    ///     D: midValue
    ///     E: maxValue
    ///     x: Value
    ///     y: Scale
    /// (2) 关系
    ///     当 x <= midValue 时，y = a1 * x + b1 
    ///     a1 = (1 - B) / (D - C)
    ///     b1 = (B * D - C) / (D - C)
    ///     当 x > midValue 时，y = a2 * (x - D)^n + 1
    ///     a2 = (A - 1) / ((E - D)^n)
    ///     n >= 1
    /// </summary>
    public partial class ScaleSlider : Slider
    {
        #region public
        /// <summary>
        /// 缩放倍数
        /// </summary>
        public double Scale
        {
            get => (double)GetValue(ScaleProperty);
            set => SetValue(ScaleProperty, value);
        }

        public ScaleSlider()
        {
            InitializeComponent();
            Maximum = 100;
            Minimum = 0;
            midValue = (Maximum + Minimum) / 2;
            a1 = (1 - minScale) / (midValue - Minimum);
            b1 = (minScale * midValue - Minimum) / (midValue - Minimum);
            a2 = (maxScale - 1) / Math.Pow(Maximum - midValue, n);
            Value = ScaleToValue(1);
        }
        #endregion

        private const double minScale = 0.1;
        private const double maxScale = 128;
        private readonly double midValue;
        private readonly double a1;
        private readonly double b1;
        private readonly double a2;
        private readonly double n = 2.5;
        private bool isSettingValue = false;

        private double ValueToScale(double value)
        {
            if (value <= midValue)
            {
                return value * a1 + b1;
            }
            else
            {
                return a2 * Math.Pow(value - midValue, n) + 1;
            }
        }

        private double ScaleToValue(double scale)
        {
            if (scale <= 1)
            {
                return (scale - b1) / a1;
            }
            else
            {
                return Math.Pow((scale - 1) / a2, 1 / n) + midValue;
            }
        }

        protected override void OnValueChanged(double oldValue, double newValue)
        {
            base.OnValueChanged(oldValue, newValue);
            if (!isSettingValue)
            {
                Scale = ValueToScale(newValue);
            }
        }

        private static void ScaleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ScaleSlider self && self != null)
            {
                self.isSettingValue = true;
                self.Value = self.ScaleToValue((double)e.NewValue);
                self.isSettingValue = false;
            }
        }

        public static readonly DependencyProperty ScaleProperty = DependencyProperty.Register("Scale", typeof(double), typeof(ScaleSlider), new PropertyMetadata(1d,new PropertyChangedCallback(ScaleChanged)));
    }
}
