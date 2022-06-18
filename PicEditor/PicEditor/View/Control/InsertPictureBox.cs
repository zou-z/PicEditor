using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace PicEditor.View.Control
{
    internal class InsertPictureBox : RectSelectorBase
    {
        public double WhRatio => (double)GetValue(WhRatioProperty);

        public bool IsKeepRatio => (bool)GetValue(IsKeepRatioProperty);

        private bool keptRatio = true; // 是否已经保持了宽高比例

        protected override void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (isPressed)
            {
                Point p = e.GetPosition(parent);
                if (mode == Modes.Default)
                {
                    RealLeft = (point.X + p.X - startPoint.X) / Scale;
                    RealTop = (point.Y + p.Y - startPoint.Y) / Scale;
                }
                else
                {
                    double left = RealLeft, top = RealTop, width = RealWidth, height = RealHeight;
                    if ((mode & Modes.N) == Modes.N)
                    {
                        top = (p.Y >= point.Y - 1 ? point.Y - 1 : p.Y) / Scale;
                        height = (p.Y >= point.Y - 1 ? 1 : point.Y - p.Y) / Scale;
                    }
                    else if ((mode & Modes.S) == Modes.S)
                    {
                        height = (p.Y - 1 <= point.Y ? 1 : p.Y - point.Y) / Scale;
                    }
                    if ((mode & Modes.W) == Modes.W)
                    {
                        left = (p.X >= point.X - 1 ? point.X - 1 : p.X) / Scale;
                        width = (p.X >= point.X - 1 ? 1 : point.X - p.X) / Scale;
                    }
                    else if ((mode & Modes.E) == Modes.E)
                    {
                        width = (p.X - 1 <= point.X ? 1 : p.X - point.X) / Scale;
                    }

                    if (IsKeepRatio)
                    {
                        if (mode == Modes.N || mode == Modes.S || mode == Modes.W || mode == Modes.E)
                        {
                            RealLeft = left;
                            RealTop = top;
                            if (mode == Modes.N || mode == Modes.S)
                            {
                                RealHeight = height;
                                RealWidth = height * WhRatio;
                            }
                            else if (mode == Modes.W || mode == Modes.E)
                            {
                                RealWidth = width;
                                RealHeight = width / WhRatio;
                            }
                        }
                        else
                        {
                            if (width / height > WhRatio)
                            {
                                RealTop = top;
                                RealHeight = height;
                                if (mode == (Modes.N | Modes.W) || mode == (Modes.S | Modes.W))
                                {
                                    RealLeft = point.X / Scale - height * WhRatio;
                                    RealWidth = height * WhRatio;
                                }
                                else if (mode == (Modes.N | Modes.E) || mode == (Modes.S | Modes.E))
                                {
                                    RealLeft = left;
                                    RealWidth = height * WhRatio;
                                }
                            }
                            else
                            {
                                RealLeft = left;
                                RealWidth = width;
                                if (mode == (Modes.N | Modes.W) || mode == (Modes.N | Modes.E))
                                {
                                    RealTop = point.Y / Scale - width / WhRatio;
                                    RealHeight = width / WhRatio;
                                }
                                else if (mode == (Modes.S | Modes.W) || mode == (Modes.S | Modes.E))
                                {
                                    RealTop = top;
                                    RealHeight = width / WhRatio;
                                }
                            }
                        }
                    }
                    else
                    {
                        RealLeft = left;
                        RealTop = top;
                        RealWidth = width;
                        RealHeight = height;
                    }
                    keptRatio = IsKeepRatio;
                }
            }
        }

        private static void IsKeepRatioChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is InsertPictureBox self && self != null && e.NewValue is bool b && b && !self.keptRatio)
            {
                if (self.RealWidth / self.RealHeight > self.WhRatio)
                {
                    self.RealWidth = self.RealHeight * self.WhRatio;
                }
                if (self.RealWidth / self.RealHeight < self.WhRatio)
                {
                    self.RealHeight = self.RealWidth / self.WhRatio;
                }
                self.keptRatio = true;
            }
        }

        public static readonly DependencyProperty WhRatioProperty = DependencyProperty.Register("WhRatio", typeof(double), typeof(InsertPictureBox), new PropertyMetadata(0d));
        public static readonly DependencyProperty IsKeepRatioProperty = DependencyProperty.Register("IsKeepRatio", typeof(bool), typeof(InsertPictureBox), new PropertyMetadata(true, new PropertyChangedCallback(IsKeepRatioChanged)));
    }
}
