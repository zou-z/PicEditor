using PicEditor.Layer;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace PicEditor.View.Control
{
    public class RectSelector : System.Windows.Controls.Panel
    {
        public double Scale
        {
            get => (double)GetValue(ScaleProperty);
            set => SetValue(ScaleProperty, value);
        }

        public double RealLeft
        {
            get { return (double)GetValue(RealLeftProperty); }
            set { SetValue(RealLeftProperty, value); }
        }

        public double RealTop
        {
            get { return (double)GetValue(RealTopProperty); }
            set { SetValue(RealTopProperty, value); }
        }

        public double RealWidth
        {
            get { return (double)GetValue(RealWidthProperty); }
            set { SetValue(RealWidthProperty, value); }
        }

        public double RealHeight
        {
            get { return (double)GetValue(RealHeightProperty); }
            set { SetValue(RealHeightProperty, value); }
        }

        public double WhRatio => (double)GetValue(WhRatioProperty);

        public bool IsKeepRatio => (bool)GetValue(IsKeepRatioProperty);

        public RectSelector()
        {
            halfSize = (int)(size / 2);
            whiteBorder = new Border { BorderBrush = Brushes.White, BorderThickness = new Thickness(1), Background = new SolidColorBrush(Color.FromArgb(1, 0, 0, 0)), Tag = Modes.Default };
            blackBorder = new Rectangle { Stroke = Brushes.Black, StrokeThickness = 1, StrokeDashArray = new DoubleCollection() { 5, 5 } };
            nwThumb = CreateThumb(Modes.N | Modes.W);
            neThumb = CreateThumb(Modes.N | Modes.E);
            swThumb = CreateThumb(Modes.S | Modes.W);
            seThumb = CreateThumb(Modes.S | Modes.E);
            nThumb = CreateThumb(Modes.N);
            sThumb = CreateThumb(Modes.S);
            wThumb = CreateThumb(Modes.W);
            eThumb = CreateThumb(Modes.E);
            Children.Add(whiteBorder);
            Children.Add(blackBorder);
            Children.Add(nwThumb);
            Children.Add(neThumb);
            Children.Add(swThumb);
            Children.Add(seThumb);
            Children.Add(nThumb);
            Children.Add(sThumb);
            Children.Add(wThumb);
            Children.Add(eThumb);
            SnapsToDevicePixels = true;
            Loaded += RectSelector_Loaded;
        }

        #region protected
        protected override Size MeasureOverride(Size availableSize)
        {
            return base.MeasureOverride(availableSize);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            foreach (UIElement child in Children)
            {
                if (child.Equals(whiteBorder) || child.Equals(blackBorder))
                {
                    child.Arrange(new Rect(0, 0, finalSize.Width, finalSize.Height));
                }
                else if (child.Equals(nwThumb))
                {
                    child.Arrange(new Rect(-halfSize, -halfSize, size, size));
                }
                else if (child.Equals(neThumb))
                {
                    child.Arrange(new Rect(finalSize.Width - halfSize - 1, -halfSize, size, size));
                }
                else if (child.Equals(swThumb))
                {
                    child.Arrange(new Rect(-halfSize, finalSize.Height - halfSize - 1, size, size));
                }
                else if (child.Equals(seThumb))
                {
                    child.Arrange(new Rect(finalSize.Width - halfSize - 1, finalSize.Height - halfSize - 1, size, size));
                }
                else if (child.Equals(nThumb))
                {
                    child.Arrange(new Rect((finalSize.Width - size) / 2, -halfSize, size, size));
                }
                else if (child.Equals(sThumb))
                {
                    child.Arrange(new Rect((finalSize.Width - size) / 2, finalSize.Height - halfSize - 1, size, size));
                }
                else if (child.Equals(wThumb))
                {
                    child.Arrange(new Rect(-halfSize, (finalSize.Height - size) / 2, size, size));
                }
                else if (child.Equals(eThumb))
                {
                    child.Arrange(new Rect(finalSize.Width - halfSize - 1, (finalSize.Height - size) / 2, size, size));
                }
            }
            return finalSize;
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            Canvas_MouseLeftButtonUp(this, null);
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);
            Canvas_MouseLeftButtonUp(this, null);
        }
        #endregion

        private const double size = 7;
        private readonly int halfSize;
        private Canvas? parent = null;
        private readonly Border whiteBorder;
        private readonly Rectangle blackBorder;
        private readonly Border nwThumb;
        private readonly Border neThumb;
        private readonly Border swThumb;
        private readonly Border seThumb;
        private readonly Border nThumb;
        private readonly Border sThumb;
        private readonly Border wThumb;
        private readonly Border eThumb;
        private bool isPressed = false;
        private Modes mode = Modes.Default;
        private Point point = new(0, 0);
        private Point startPoint = new(0, 0);
        private bool keptRatio = true; // 是否已经保持了宽高比例

        private enum Modes { Default = 0x0, N = 0x1, S = 0x2, W = 0x4, E = 0x8 }

        private static Border CreateThumb(Modes mode)
        {
            Cursor cursor = Cursors.Arrow;
            if (mode == (Modes.N | Modes.W) || mode == (Modes.S | Modes.E))
            {
                cursor = Cursors.SizeNWSE;
            }
            else if (mode == (Modes.N | Modes.E) || mode == (Modes.S | Modes.W))
            {
                cursor = Cursors.SizeNESW;
            }
            else if (mode == Modes.N || mode == Modes.S)
            {
                cursor = Cursors.SizeNS;
            }
            else if (mode == Modes.W || mode == Modes.E)
            {
                cursor = Cursors.SizeWE;
            }
            return new Border
            {
                Width = size,
                Height = size,
                Background = Brushes.White,
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(1),
                Cursor = cursor,
                Tag = mode
            };
        }

        private void RectSelector_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= RectSelector_Loaded;
            Canvas.SetLeft(this, 0);
            Canvas.SetTop(this, 0);
            if (Parent is Canvas canvas && canvas != null)
            {
                parent = canvas;
                canvas.MouseMove += Canvas_MouseMove;
                canvas.MouseLeftButtonUp += Canvas_MouseLeftButtonUp;
                whiteBorder.MouseLeftButtonDown += Thumb_MouseLeftButtonDown;
                nwThumb.MouseLeftButtonDown += Thumb_MouseLeftButtonDown;
                neThumb.MouseLeftButtonDown += Thumb_MouseLeftButtonDown;
                swThumb.MouseLeftButtonDown += Thumb_MouseLeftButtonDown;
                seThumb.MouseLeftButtonDown += Thumb_MouseLeftButtonDown;
                nThumb.MouseLeftButtonDown += Thumb_MouseLeftButtonDown;
                sThumb.MouseLeftButtonDown += Thumb_MouseLeftButtonDown;
                wThumb.MouseLeftButtonDown += Thumb_MouseLeftButtonDown;
                eThumb.MouseLeftButtonDown += Thumb_MouseLeftButtonDown;
            }
        }

        private void Thumb_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender.Equals(whiteBorder))
            {
                startPoint = e.GetPosition(parent);
                point = new Point(Canvas.GetLeft(this), Canvas.GetTop(this));
                Cursor = Cursors.SizeAll;
            }
            else
            {
                if (sender.Equals(nwThumb) || sender.Equals(nThumb))
                {
                    point = new Point(Canvas.GetLeft(this) + Width, Canvas.GetTop(this) + Height);
                }
                else if (sender.Equals(neThumb) || sender.Equals(eThumb))
                {
                    point = new Point(Canvas.GetLeft(this), Canvas.GetTop(this) + Height);
                }
                else if (sender.Equals(seThumb) || sender.Equals(sThumb))
                {
                    point = new Point(Canvas.GetLeft(this), Canvas.GetTop(this));
                }
                else if (sender.Equals(swThumb) || sender.Equals(wThumb))
                {
                    point = new Point(Canvas.GetLeft(this) + Width, Canvas.GetTop(this));
                }
            }
            mode = (Modes)((FrameworkElement)sender).Tag;
            Mouse.Capture(sender as FrameworkElement);
            isPressed = true;
        }

        private void Canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs? e)
        {
            isPressed = false;
            Mouse.Capture(null);
            Cursor = Cursors.Arrow;
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
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

        private static void ScaleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is RectSelector self && self != null)
            {
                double scale = (double)e.NewValue;
                self.Width = self.RealWidth * scale;
                self.Height = self.RealHeight * scale;
                Canvas.SetLeft(self, self.RealLeft * scale);
                Canvas.SetTop(self, self.RealTop * scale);
            }
        }

        private static void RealLeftChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is RectSelector self && self != null)
            {
                Canvas.SetLeft(self, (double)e.NewValue * self.Scale);
            }
        }

        private static void RealTopChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is RectSelector self && self != null)
            {
                Canvas.SetTop(self, (double)e.NewValue * self.Scale);
            }
        }

        private static void RealWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is RectSelector self && self != null)
            {
                self.Width = (double)e.NewValue * self.Scale;
            }
        }

        private static void RealHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is RectSelector self && self != null)
            {
                self.Height = (double)e.NewValue * self.Scale;
            }
        }

        private static void IsKeepRatioChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is RectSelector self && self != null && e.NewValue is bool b && b && !self.keptRatio)
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

        public static readonly DependencyProperty ScaleProperty = DependencyProperty.Register("Scale", typeof(double), typeof(RectSelector), new PropertyMetadata(0d, new PropertyChangedCallback(ScaleChanged)));
        public static readonly DependencyProperty RealLeftProperty = DependencyProperty.Register("RealLeft", typeof(double), typeof(RectSelector), new PropertyMetadata(0d, new PropertyChangedCallback(RealLeftChanged)));
        public static readonly DependencyProperty RealTopProperty = DependencyProperty.Register("RealTop", typeof(double), typeof(RectSelector), new PropertyMetadata(0d, new PropertyChangedCallback(RealTopChanged)));
        public static readonly DependencyProperty RealWidthProperty = DependencyProperty.Register("RealWidth", typeof(double), typeof(RectSelector), new PropertyMetadata(0d, new PropertyChangedCallback(RealWidthChanged)));
        public static readonly DependencyProperty RealHeightProperty = DependencyProperty.Register("RealHeight", typeof(double), typeof(RectSelector), new PropertyMetadata(0d, new PropertyChangedCallback(RealHeightChanged)));
        public static readonly DependencyProperty WhRatioProperty = DependencyProperty.Register("WhRatio", typeof(double), typeof(RectSelector), new PropertyMetadata(0d));
        public static readonly DependencyProperty IsKeepRatioProperty = DependencyProperty.Register("IsKeepRatio", typeof(bool), typeof(RectSelector), new PropertyMetadata(true, new PropertyChangedCallback(IsKeepRatioChanged)));
    }
}
