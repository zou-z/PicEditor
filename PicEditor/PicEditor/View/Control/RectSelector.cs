using PicEditor.Layer;
using System;
using System.Collections.Generic;
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
    public class RectSelector : System.Windows.Controls.Panel, ILayer
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

        public RectSelector()
        {
            halfSize = (int)(size / 2);
            whiteBorder = new Border { BorderBrush = Brushes.White, BorderThickness = new Thickness(1), Background = new SolidColorBrush(Color.FromArgb(1, 0, 0, 0)), Tag = Modes.Default };
            blackBorder = new Rectangle { Stroke = Brushes.Black, StrokeThickness = 1, StrokeDashArray = new DoubleCollection() { 5, 5 } };
            nwThumb = CreateThumb(Cursors.SizeNWSE, Modes.N | Modes.W);
            neThumb = CreateThumb(Cursors.SizeNESW, Modes.N | Modes.E);
            swThumb = CreateThumb(Cursors.SizeNESW, Modes.S | Modes.W);
            seThumb = CreateThumb(Cursors.SizeNWSE, Modes.S | Modes.E);
            nThumb = CreateThumb(Cursors.SizeNS, Modes.N);
            sThumb = CreateThumb(Cursors.SizeNS, Modes.S);
            wThumb = CreateThumb(Cursors.SizeWE, Modes.W);
            eThumb = CreateThumb(Cursors.SizeWE, Modes.E);
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

        private enum Modes { Default = 0x00, N = 0x01, S = 0x02, W = 0x04, E = 0x08 }

        private static Border CreateThumb(Cursor cursor, Modes tag)
        {
            return new Border
            {
                Width = size,
                Height = size,
                Background = Brushes.White,
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(1),
                Cursor = cursor,
                Tag = tag
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
                    if ((mode & Modes.N) == Modes.N)
                    {
                        RealTop = (p.Y >= point.Y - 1 ? point.Y - 1 : p.Y) / Scale;
                        RealHeight = (p.Y >= point.Y - 1 ? 1 : point.Y - p.Y) / Scale;
                    }
                    else if ((mode & Modes.S) == Modes.S)
                    {
                        RealHeight = (p.Y - 1 <= point.Y ? 1 : p.Y - point.Y) / Scale;
                    }
                    if ((mode & Modes.W) == Modes.W)
                    {
                        RealLeft = (p.X >= point.X - 1 ? point.X - 1 : p.X) / Scale;
                        RealWidth = (p.X >= point.X - 1 ? 1 : point.X - p.X) / Scale;
                    }
                    else if ((mode & Modes.E) == Modes.E)
                    {
                        RealWidth = (p.X - 1 <= point.X ? 1 : p.X - point.X) / Scale;
                    }
                }
            }
        }

        public void SetSize(double width, double height, double scale)
        {
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

        public static readonly DependencyProperty ScaleProperty = DependencyProperty.Register("Scale", typeof(double), typeof(RectSelector), new PropertyMetadata(0d, new PropertyChangedCallback(ScaleChanged)));
        public static readonly DependencyProperty RealLeftProperty = DependencyProperty.Register("RealLeft", typeof(double), typeof(RectSelector), new PropertyMetadata(0d, new PropertyChangedCallback(RealLeftChanged)));
        public static readonly DependencyProperty RealTopProperty = DependencyProperty.Register("RealTop", typeof(double), typeof(RectSelector), new PropertyMetadata(0d, new PropertyChangedCallback(RealTopChanged)));
        public static readonly DependencyProperty RealWidthProperty = DependencyProperty.Register("RealWidth", typeof(double), typeof(RectSelector), new PropertyMetadata(0d, new PropertyChangedCallback(RealWidthChanged)));
        public static readonly DependencyProperty RealHeightProperty = DependencyProperty.Register("RealHeight", typeof(double), typeof(RectSelector), new PropertyMetadata(0d, new PropertyChangedCallback(RealHeightChanged)));
    }
}
