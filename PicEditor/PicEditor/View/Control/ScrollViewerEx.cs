using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PicEditor.View.Control
{
    internal class ScrollViewerEx : ScrollViewer
    {
        #region public
        /// <summary>
        /// 是否启用鼠标拖拽移动子控件
        /// </summary>
        public bool IsContentMoveEnabled
        {
            get => (bool)GetValue(IsContentMoveEnabledProperty);
            set => SetValue(IsContentMoveEnabledProperty, value);
        }

        /// <summary>
        /// 缩放倍数
        /// </summary>
        public double Scale
        {
            get => (double)GetValue(ScaleProperty);
            set => SetValue(ScaleProperty, value);
        }

        public ScrollViewerEx()
        {
            HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            Loaded += ScrollViewerEx_Loaded;
        }

        public static readonly DependencyProperty IsContentMoveEnabledProperty = DependencyProperty.Register("IsContentMoveEnabled", typeof(bool), typeof(ScrollViewerEx), new PropertyMetadata(true));
        public static readonly DependencyProperty ScaleProperty = DependencyProperty.Register("Scale", typeof(double), typeof(ScrollViewerEx), new PropertyMetadata(0d,new PropertyChangedCallback(ScaleChanged)));
        #endregion

        #region private
        private readonly ScaleContext scaleContext = new();
        private readonly MoveContext moveContext = new();
        private bool isMousePressed = false;

        private void ScrollViewerEx_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= ScrollViewerEx_Loaded;
            PreviewMouseLeftButtonDown += ScrollViewerEx_MouseLeftButtonDown;
            MouseLeftButtonUp += ScrollViewerEx_MouseLeftButtonUp;
            MouseLeave += (_sender, _e) => { ScrollViewerEx_MouseLeftButtonUp(_sender, null); };
            MouseMove += ScrollViewerEx_MouseMove;
            PreviewMouseWheel += ScrollViewerEx_MouseWheel;
        }

        private void ScrollViewerEx_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (IsContentMoveEnabled)
            {
                Point point = e.GetPosition(this);
                if (point.X + 20 >= ActualWidth || point.Y + 20 >= ActualHeight)
                {
                    return;
                }
                moveContext.BeforeMoving(point, HorizontalOffset, VerticalOffset);
                Mouse.Capture(this);
                Cursor = Cursors.ScrollAll;
                isMousePressed = true;
            }
        }

        private void ScrollViewerEx_MouseLeftButtonUp(object sender, MouseButtonEventArgs? e)
        {
            if (IsContentMoveEnabled)
            {
                isMousePressed = false;
                Mouse.Capture(null);
                Cursor = Cursors.Arrow;
            }
        }

        private void ScrollViewerEx_MouseMove(object sender, MouseEventArgs e)
        {
            if (isMousePressed && IsContentMoveEnabled)
            {
                double[] result = moveContext.Moving(e.GetPosition(this));
                ScrollToHorizontalOffset(result[0]);
                ScrollToVerticalOffset(result[1]);
            }
        }

        private void ScrollViewerEx_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                if (Content is Canvas canvas && canvas != null && e.Delta != 0)
                {
                    /*
                    // 获取缩放相关参数
                    double preScale = scaleContext.Get();        // 保存缩放前的缩放倍数
                    if (!scaleContext.Zoom(e.Delta > 0))         // 获取缩放后的缩放倍数
                    {
                        e.Handled = true;
                        return;
                    }
                    Point contentPoint = e.GetPosition(canvas);  // 获取鼠标相对内容控件的坐标
                    Point viewPoint = e.GetPosition(this);       // 获取鼠标相对视区的坐标

                    // 定点缩放
                    double left = contentPoint.X / preScale;
                    double top = contentPoint.Y / preScale;
                    left = (left * scaleContext.Get()) - viewPoint.X + canvas.Margin.Left;
                    top = (top * scaleContext.Get()) - viewPoint.Y + canvas.Margin.Top;
                    ScrollToHorizontalOffset(left);
                    ScrollToVerticalOffset(top);

                    // 计算和设置每个控件的位置和尺寸
                    double relativeScale = scaleContext.Get() / preScale;
                    canvas.Width *= relativeScale;
                    canvas.Height *= relativeScale;
                    foreach (FrameworkElement fe in canvas.Children)
                    {
                        left = Canvas.GetLeft(fe);
                        top = Canvas.GetTop(fe);
                        Canvas.SetLeft(fe, left * relativeScale);
                        Canvas.SetTop(fe, top * relativeScale);
                        fe.Width *= relativeScale;
                        fe.Height *= relativeScale;
                    }*/



                    if (!scaleContext.Zoom(e.Delta > 0))         // 获取缩放后的缩放倍数
                    {
                        e.Handled = true;
                        return;
                    }
                    Scale = scaleContext.Get();
                }
            }
        }

        private void UpdateView(double scaleBefore,double scaleAfter)
        {
            if (Content is Canvas canvas && canvas != null)
            {
                Point contentPoint = Mouse.GetPosition(canvas);  // 获取鼠标相对内容控件的坐标
                Point viewPoint = Mouse.GetPosition(this);       // 获取鼠标相对视区的坐标

                // 定点缩放
                double left = contentPoint.X / scaleBefore;
                double top = contentPoint.Y / scaleBefore;
                left = (left * scaleAfter) - viewPoint.X + canvas.Margin.Left;
                top = (top * scaleAfter) - viewPoint.Y + canvas.Margin.Top;
                ScrollToHorizontalOffset(left);
                ScrollToVerticalOffset(top);

                // 计算和设置每个控件的位置和尺寸
                double relativeScale = scaleAfter / scaleBefore;
                canvas.Width *= relativeScale;
                canvas.Height *= relativeScale;
                foreach (FrameworkElement fe in canvas.Children)
                {
                    left = Canvas.GetLeft(fe);
                    top = Canvas.GetTop(fe);
                    Canvas.SetLeft(fe, left * relativeScale);
                    Canvas.SetTop(fe, top * relativeScale);
                    fe.Width *= relativeScale;
                    fe.Height *= relativeScale;
                }
            }
        }

        private static void ScaleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ScrollViewerEx self && self != null)
            {
                double scale1 = (double)e.OldValue;
                double scale2 = (double)e.NewValue;
                self.UpdateView(scale1, scale2);
            }
        }
        #endregion

        #region ScaleContext
        private class ScaleContext
        {
            private int index;
            private const int defaultIndex = 6;
            private readonly double[] scales;

            public ScaleContext()
            {
                index = defaultIndex;
                scales = new double[] { 0.1, 0.25, 0.33, 0.5, 0.66, 0.75, 1.0, 1.25, 1.5, 2.0, 3.0, 4.0, 6.0, 8.0, 16.0, 32.0, 64.0, 128.0 };
            }

            public double Get()
            {
                return scales[index];
            }

            public void Reset()
            {
                index = defaultIndex;
            }

            public bool Zoom(bool isZoomIn)
            {
                if ((index <= 0 && !isZoomIn) || (index + 1 >= scales.Length && isZoomIn))
                {
                    return false;
                }
                index = isZoomIn ? index + 1 : index - 1;
                return true;
            }

            public void Set(double scale)
            {
                for (int i = 0; i < scales.Length; ++i)
                {
                    if (scale == scales[i])
                    {
                        index = i;
                        return;
                    }
                }
                index = defaultIndex;
            }
        }
        #endregion

        #region MoveContext
        private class MoveContext
        {
            private Point point = new();
            private double left = 0;
            private double top = 0;
            private readonly double[] offset = new double[2];

            public void BeforeMoving(Point point, double left, double top)
            {
                this.point = point;
                this.left = left;
                this.top = top;
            }

            public double[] Moving(Point point)
            {
                offset[0] = this.point.X - point.X + left;
                offset[1] = this.point.Y - point.Y + top;
                return offset;
            }
        }
        #endregion
    }
}
