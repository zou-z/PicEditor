﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace PicEditor.View.Control
{
    internal abstract class RectSelectorBase : System.Windows.Controls.Panel
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

        public RectSelectorBase()
        {
            cornerThumbSizeHalf = (int)(cornerThumbSize / 2);
            edgeThumbThicknessHalf = (int)(edgeThumbThickness / 2);
            hoverBackground = (SolidColorBrush)Application.Current.Resources["ThemeDarkColor"];
            whiteBorder = new Border { BorderBrush = Brushes.White, BorderThickness = new Thickness(1), Background = Brushes.Transparent, Tag = Modes.Default };
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
            Children.Add(nThumb);
            Children.Add(sThumb);
            Children.Add(wThumb);
            Children.Add(eThumb);
            Children.Add(nwThumb);
            Children.Add(neThumb);
            Children.Add(swThumb);
            Children.Add(seThumb);
            SnapsToDevicePixels = true;
            Loaded += RectSelector_Loaded;
        }

        #region protected
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
                    child.Arrange(new Rect(-cornerThumbSizeHalf, -cornerThumbSizeHalf, cornerThumbSize, cornerThumbSize));
                }
                else if (child.Equals(neThumb))
                {
                    child.Arrange(new Rect(finalSize.Width - cornerThumbSizeHalf - 1, -cornerThumbSizeHalf, cornerThumbSize, cornerThumbSize));
                }
                else if (child.Equals(swThumb))
                {
                    child.Arrange(new Rect(-cornerThumbSizeHalf, finalSize.Height - cornerThumbSizeHalf - 1, cornerThumbSize, cornerThumbSize));
                }
                else if (child.Equals(seThumb))
                {
                    child.Arrange(new Rect(finalSize.Width - cornerThumbSizeHalf - 1, finalSize.Height - cornerThumbSizeHalf - 1, cornerThumbSize, cornerThumbSize));
                }
                else if (child.Equals(nThumb))
                {
                    child.Arrange(new Rect(0, -edgeThumbThicknessHalf, finalSize.Width, edgeThumbThickness));
                }
                else if (child.Equals(sThumb))
                {
                    child.Arrange(new Rect(0, finalSize.Height - edgeThumbThicknessHalf - 1, finalSize.Width, edgeThumbThickness));
                }
                else if (child.Equals(wThumb))
                {
                    child.Arrange(new Rect(-edgeThumbThicknessHalf, 0, edgeThumbThickness, finalSize.Height));
                }
                else if (child.Equals(eThumb))
                {
                    child.Arrange(new Rect(finalSize.Width - edgeThumbThicknessHalf - 1, 0, edgeThumbThickness, finalSize.Height));
                }
            }
            return finalSize;
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            if (Mouse.Captured is ContextMenu)
            {
                return;
            }
            base.OnMouseLeave(e);
            Canvas_MouseLeftButtonUp(this, null);
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);
            Canvas_MouseLeftButtonUp(this, null);
        }
        #endregion

        private const double edgeThumbThickness = 5;
        private readonly int edgeThumbThicknessHalf;
        private const double cornerThumbSize = 7;
        private readonly int cornerThumbSizeHalf;
        private readonly Brush hoverBackground;
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

        protected Canvas? parent = null;
        protected bool isPressed = false;
        protected Modes mode = Modes.Default;
        protected Point point = new(0, 0);
        protected Point startPoint = new(0, 0);

        protected enum Modes { Default = 0x0, N = 0x1, S = 0x2, W = 0x4, E = 0x8 }

        private Border CreateThumb(Modes mode)
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
            var border = new Border
            {
                Cursor = cursor,
                Tag = mode,
                Background = Brushes.Transparent,
            };
            border.MouseEnter += (sender, e) =>
            {
                border.Background = hoverBackground;
            };
            border.MouseLeave += (sender, e) =>
            {
                border.Background = Brushes.Transparent;
            };
            return border;
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

        protected abstract void Canvas_MouseMove(object sender, MouseEventArgs e);

        private static void ScaleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is RectSelectorBase self && self != null)
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
            if (d is RectSelectorBase self && self != null)
            {
                Canvas.SetLeft(self, (double)e.NewValue * self.Scale);
            }
        }

        private static void RealTopChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is RectSelectorBase self && self != null)
            {
                Canvas.SetTop(self, (double)e.NewValue * self.Scale);
            }
        }

        private static void RealWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is RectSelectorBase self && self != null)
            {
                self.Width = (double)e.NewValue * self.Scale;
            }
        }

        private static void RealHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is RectSelectorBase self && self != null)
            {
                self.Height = (double)e.NewValue * self.Scale;
            }
        }

        public static readonly DependencyProperty ScaleProperty = DependencyProperty.Register("Scale", typeof(double), typeof(RectSelectorBase), new PropertyMetadata(0d, new PropertyChangedCallback(ScaleChanged)));
        public static readonly DependencyProperty RealLeftProperty = DependencyProperty.Register("RealLeft", typeof(double), typeof(RectSelectorBase), new PropertyMetadata(0d, new PropertyChangedCallback(RealLeftChanged)));
        public static readonly DependencyProperty RealTopProperty = DependencyProperty.Register("RealTop", typeof(double), typeof(RectSelectorBase), new PropertyMetadata(0d, new PropertyChangedCallback(RealTopChanged)));
        public static readonly DependencyProperty RealWidthProperty = DependencyProperty.Register("RealWidth", typeof(double), typeof(RectSelectorBase), new PropertyMetadata(0d, new PropertyChangedCallback(RealWidthChanged)));
        public static readonly DependencyProperty RealHeightProperty = DependencyProperty.Register("RealHeight", typeof(double), typeof(RectSelectorBase), new PropertyMetadata(0d, new PropertyChangedCallback(RealHeightChanged)));
    }
}
