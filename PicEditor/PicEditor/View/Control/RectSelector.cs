using PicEditor.Layer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace PicEditor.View.Control
{
    internal class RectSelector : Canvas, ILayer
    {
        private readonly Selector selector;

        public RectSelector()
        {
            selector = new Selector()
            {
                Width = 30,
                Height = 30,
            };
            Children.Add(selector);
        }

        public void SetSize(double width, double height, double scale)
        {

        }


        private class Selector : System.Windows.Controls.Panel
        {
            private const double size = 7;
            private readonly int halfSize;
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

            public Selector()
            {
                halfSize = (int)(size / 2);
                whiteBorder = new Border { BorderBrush = Brushes.White, BorderThickness = new Thickness(1) };
                blackBorder = new Rectangle { Stroke = Brushes.Black, StrokeThickness = 1, StrokeDashArray = new DoubleCollection() { 5, 5 } };
                nwThumb = new Border { Width = size, Height = size, Background = Brushes.White, BorderBrush = Brushes.Black, BorderThickness = new Thickness(1) };
                neThumb = new Border { Width = size, Height = size, Background = Brushes.White, BorderBrush = Brushes.Black, BorderThickness = new Thickness(1) };
                swThumb = new Border { Width = size, Height = size, Background = Brushes.White, BorderBrush = Brushes.Black, BorderThickness = new Thickness(1) };
                seThumb = new Border { Width = size, Height = size, Background = Brushes.White, BorderBrush = Brushes.Black, BorderThickness = new Thickness(1) };
                nThumb = new Border { Width = size, Height = size, Background = Brushes.White, BorderBrush = Brushes.Black, BorderThickness = new Thickness(1) };
                sThumb = new Border { Width = size, Height = size, Background = Brushes.White, BorderBrush = Brushes.Black, BorderThickness = new Thickness(1) };
                wThumb = new Border { Width = size, Height = size, Background = Brushes.White, BorderBrush = Brushes.Black, BorderThickness = new Thickness(1) };
                eThumb = new Border { Width = size, Height = size, Background = Brushes.White, BorderBrush = Brushes.Black, BorderThickness = new Thickness(1) };
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
                Background = new SolidColorBrush(Color.FromArgb(1, 0, 0, 0));
                SnapsToDevicePixels = true;
            }

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
        }
    }
}
