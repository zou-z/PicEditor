using PicEditor.Layer.Basic;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PicEditor.Layer
{
    public class LayerPanel : ScrollViewer
    {
        #region public
        // 图片图层
        public ObservableCollection<ILayer> Layers
        {
            get => (ObservableCollection<ILayer>)GetValue(LayersProperty);
            set => SetValue(LayersProperty, value);
        }

        // 图片图层上面的图层
        public ObservableCollection<ILayer> UpperLayers
        {
            get => (ObservableCollection<ILayer>)GetValue(UpperLayersProperty);
            set => SetValue(UpperLayersProperty, value);
        }

        // 缩放倍数
        public double Scale
        {
            get => (double)GetValue(ScaleProperty);
            set => SetValue(ScaleProperty, value);
        }

        // 画布尺寸
        public Size CanvasSize
        {
            get => (Size)GetValue(CanvasSizeProperty);
            set => SetValue(CanvasSizeProperty, value);
        }

        // 画布边缘空白距离
        public Thickness CanvasMargin
        {
            get => (Thickness)GetValue(CanvasMarginProperty);
            set => SetValue(CanvasMarginProperty, value);
        }
        // 鼠标位置
        public Point MousePosition
        {
            get => (Point)GetValue(MousePositionProperty);
            set => SetValue(MousePositionProperty, value);
        }

        // 是否启用鼠标拖拽移动画布
        public bool IsCanvasMoveEnabled
        {
            get => (bool)GetValue(IsCanvasMoveEnabledProperty);
            set => SetValue(IsCanvasMoveEnabledProperty, value);
        }

        public LayerPanel()
        {
            HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            Content = canvas = new Canvas();
            squareBackground = new SquareBackground();
            scaleContext = new ScaleContext();
            moveContext = new MoveContext();
            SnapsToDevicePixels = true;
            Loaded += LayerPanel_Loaded;
        }
        #endregion

        #region private
        private readonly Canvas canvas;
        private readonly SquareBackground squareBackground;
        private readonly ScaleContext scaleContext;
        private readonly MoveContext moveContext;
        private bool isMousePressed = false;
        private bool isScalingByMouseWheel = false;
        private bool isEmptyContent = false;

        private void LayerPanel_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= LayerPanel_Loaded;
            PreviewMouseLeftButtonDown += LayerPanel_MouseLeftButtonDown;
            MouseLeftButtonUp += LayerPanel_MouseLeftButtonUp;
            MouseMove += LayerPanel_MouseMove;
            MouseLeave += (_sender, _e) => { LayerPanel_MouseLeftButtonUp(_sender, null); };
            PreviewMouseWheel += LayerPanel_MouseWheel;
        }

        #region 依赖属性值改变时的回调
        private static void LayersSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is LayerPanel self && self != null)
            {
                if (e.OldValue is ObservableCollection<ILayer> oldLayers && oldLayers != null)
                {
                    oldLayers.CollectionChanged -= self.ContentChanged;
                }
                if (e.NewValue is ObservableCollection<ILayer> newLayers && newLayers != null)
                {
                    newLayers.CollectionChanged += self.ContentChanged;
                }
            }
        }

        private static void CanvasMarginChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is LayerPanel self && self != null)
            {
                self.canvas.Margin = (Thickness)e.NewValue;
            }
        }

        private static void CanvasSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is LayerPanel self && self != null)
            {
                Size size = (Size)e.NewValue;
                self.SetCanvasSize(size.Width, size.Height);
            }
        }

        private static void ScaleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is LayerPanel self && self != null)
            {
                self.UpdateView((double)e.OldValue, (double)e.NewValue);
            }
        }
        #endregion

        private void ContentChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            canvas.Children.Clear();
            canvas.Children.Add(squareBackground);
            if (Layers != null)
            {
                foreach (ILayer? layer in Layers)
                {
                    if (layer != null && layer is UIElement element && element != null)
                    {
                        canvas.Children.Add(element);
                    }
                }
            }
            if (UpperLayers != null)
            {
                foreach (ILayer? layer in UpperLayers)
                {
                    if (layer != null && layer is UIElement element && element != null)
                    {
                        canvas.Children.Add(element);
                    }
                }
            }
            if (isEmptyContent)
            {
                isEmptyContent = false;
                double width = CanvasSize.Width + canvas.Margin.Left + canvas.Margin.Right;
                double height = CanvasSize.Height + canvas.Margin.Top + canvas.Margin.Bottom;
                Dispatcher.BeginInvoke(() =>
                {
                    ScrollToHorizontalOffset((width - ActualWidth) / 2);
                    ScrollToVerticalOffset((height - ActualHeight) / 2);
                });
            }
            else
            {
                isEmptyContent = Layers?.Count == 0 && UpperLayers?.Count == 0;
            }
        }

        #region 移动画布
        private void LayerPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (IsCanvasMoveEnabled)
            {
                Point point = e.GetPosition(this);
                #region 排除滚动条上的点击事件
                //if ((HorizontalScrollBarVisibility == ScrollBarVisibility.Visible && point.X + 20 >= ActualWidth) ||
                //    (VerticalScrollBarVisibility == ScrollBarVisibility.Visible && point.Y + 20 >= ActualHeight))
                //{
                //    return;
                //}
                if (point.X + 20 >= ActualWidth || point.Y + 20 >= ActualHeight)
                {
                    return;
                }
                #endregion
                moveContext.BeforeMoving(point, HorizontalOffset, VerticalOffset);
                Mouse.Capture(this);
                Cursor = Cursors.ScrollAll;
                isMousePressed = true;
            }
        }

        private void LayerPanel_MouseLeftButtonUp(object sender, MouseButtonEventArgs? e)
        {
            if (IsCanvasMoveEnabled || isMousePressed)
            {
                isMousePressed = false;
                Mouse.Capture(null);
                Cursor = Cursors.Arrow;
            }
        }

        private void LayerPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (IsCanvasMoveEnabled && isMousePressed)
            {
                double[] result = moveContext.Moving(e.GetPosition(this));
                ScrollToHorizontalOffset(result[0]);
                ScrollToVerticalOffset(result[1]);
            }
        }
        #endregion

        #region 缩放画布
        private void LayerPanel_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                if (canvas != null && e.Delta != 0)
                {
                    double scale = scaleContext.GetNearbyScale(e.Delta > 0);
                    if (scale < 0)
                    {
                        e.Handled = true;
                        return;
                    }
                    UpdateView(scale);
                }
            }
        }

        // 通过滚轮缩放
        private void UpdateView(double scale)
        {
            Point contentPoint = Mouse.GetPosition(canvas);  // 获取鼠标相对内容控件的坐标
            Point viewPoint = Mouse.GetPosition(this);       // 获取鼠标相对视区的坐标

            // 鼠标点定点缩放
            double left = contentPoint.X / scaleContext.Get();
            double top = contentPoint.Y / scaleContext.Get();
            left = (left * scale) - viewPoint.X + canvas.Margin.Left;
            top = (top * scale) - viewPoint.Y + canvas.Margin.Top;
            ScrollToHorizontalOffset(left);
            ScrollToVerticalOffset(top);

            // 计算和设置每个控件的位置和尺寸
            SetCanvasSize(CanvasSize.Width * scale, CanvasSize.Height * scale);
            foreach (object? child in canvas.Children)
            {
                if (child is ILayer layer && layer != null)
                {
                    layer.SetSize(canvas.Width, canvas.Height, scale);
                }
            }
            scaleContext.Set(scale);
            isScalingByMouseWheel = true;
            Scale = scaleContext.Get();
            isScalingByMouseWheel = false;
        }

        // 通过数值缩放
        private void UpdateView(double previousScale, double scale)
        {
            if (isScalingByMouseWheel || previousScale <= 0 || scale <= 0)
            {
                return;
            }
            Point contentPoint = new(HorizontalOffset + ActualWidth / 2 - canvas.Margin.Left, VerticalOffset + ActualHeight / 2 - canvas.Margin.Top);
            Point viewPoint = new(ActualWidth / 2, ActualHeight / 2);

            // 视区中心定点缩放
            double left = contentPoint.X / previousScale;
            double top = contentPoint.Y / previousScale;
            left = (left * scale) - viewPoint.X + canvas.Margin.Left;
            top = (top * scale) - viewPoint.Y + canvas.Margin.Top;
            ScrollToHorizontalOffset(left);
            ScrollToVerticalOffset(top);

            // 计算和设置每个控件的位置和尺寸
            SetCanvasSize(CanvasSize.Width * scale, CanvasSize.Height * scale);
            foreach (object? child in canvas.Children)
            {
                if (child is ILayer layer && layer != null)
                {
                    layer.SetSize(canvas.Width, canvas.Height, scale);
                }
            }
            scaleContext.Set(scale);
        }

        private void SetCanvasSize(double width, double height)
        {
            canvas.Width = squareBackground.Width = width;
            canvas.Height = squareBackground.Height = height;
        }
        #endregion




        #endregion

        public static readonly DependencyProperty LayersProperty = DependencyProperty.Register("Layers", typeof(ObservableCollection<ILayer>), typeof(LayerPanel), new PropertyMetadata(null, new PropertyChangedCallback(LayersSourceChanged)));
        public static readonly DependencyProperty UpperLayersProperty = DependencyProperty.Register("UpperLayers", typeof(ObservableCollection<ILayer>), typeof(LayerPanel), new PropertyMetadata(null, new PropertyChangedCallback(LayersSourceChanged)));
        public static readonly DependencyProperty ScaleProperty = DependencyProperty.Register("Scale", typeof(double), typeof(LayerPanel), new PropertyMetadata(0d, new PropertyChangedCallback(ScaleChanged)));
        public static readonly DependencyProperty CanvasSizeProperty = DependencyProperty.Register("CanvasSize", typeof(Size), typeof(LayerPanel), new PropertyMetadata(new Size(0, 0), new PropertyChangedCallback(CanvasSizeChanged)));
        public static readonly DependencyProperty CanvasMarginProperty = DependencyProperty.Register("CanvasMargin", typeof(Thickness), typeof(LayerPanel), new PropertyMetadata(new Thickness(0), new PropertyChangedCallback(CanvasMarginChanged)));
        public static readonly DependencyProperty MousePositionProperty = DependencyProperty.Register("MousePosition", typeof(Point), typeof(LayerPanel), new PropertyMetadata(new Point(0, 0)));
        public static readonly DependencyProperty IsCanvasMoveEnabledProperty = DependencyProperty.Register("IsCanvasMoveEnabled", typeof(bool), typeof(LayerPanel), new PropertyMetadata(true));

        #region ScaleContext
        private class ScaleContext
        {
            private double scale = 1;
            private readonly double[] scales =
            {
                0.1, 0.25, 0.33, 0.5, 0.66, 0.75, 1.0, 1.25, 1.5, 2.0, 3.0, 4.0, 6.0, 8.0, 16.0, 32.0, 64.0, 128.0
            };

            public double Get()
            {
                return scale;
            }

            public void Set(double scale)
            {
                this.scale = scale;
            }

            public double GetNearbyScale(bool isZoomIn)
            {
                if (isZoomIn && scale < scales[^1])
                {
                    for (int i = scales.Length - 2; i >= 0; --i)
                    {
                        if (scales[i] <= scale)
                        {
                            return scales[i + 1];
                        }
                    }
                }
                if (!isZoomIn && scale > scales[0])
                {
                    for (int i = 1; i < scales.Length; ++i)
                    {
                        if (scales[i] >= scale)
                        {
                            return scales[i - 1];
                        }
                    }
                }
                return -1;
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
