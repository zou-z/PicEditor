using PicEditor.Layer;
using PicEditor.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PicEditor.View.Control
{
    // ImageEx
    internal class PictureLayer : InkCanvas, ILayer
    {
        public string Guid { get; set; } = string.Empty;

        public double ImageWidth { get; set; }

        public double ImageHeight { get; set; }

        public Rect Position
        {
            get { return (Rect)GetValue(PositionProperty); }
            set { SetValue(PositionProperty, value); }
        }
        public static readonly DependencyProperty PositionProperty = DependencyProperty.Register("Position", typeof(Rect), typeof(PictureLayer), new PropertyMetadata(new Rect(),new PropertyChangedCallback(PositionChanged)));

        private static void PositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PictureLayer self && self != null)
            {
                Rect rect = (Rect)e.NewValue;
                Canvas.SetLeft(self.image, rect.Left * self.scale);
                Canvas.SetTop(self.image, rect.Top * self.scale);
                self.image.Width = rect.Width * self.scale;
                self.image.Height = rect.Height * self.scale;
            }
        }

        public PictureLayer(WriteableBitmap bitmap)
        {
            image = new Image { Source = bitmap };
            Children.Add(image);
            Background = Brushes.Transparent;
            EditingMode = InkCanvasEditingMode.None;
        }

        public void SetSize(double width, double height, double scale)
        {
            this.scale = scale;
            Width = width;
            Height = height;
            image.Width = ImageWidth * scale;
            image.Height = ImageHeight * scale;
            RenderOptions.SetBitmapScalingMode(image, scale >= 4 ? BitmapScalingMode.NearestNeighbor : BitmapScalingMode.Linear);
        }

        public VisualBrush GetVisualBrush()
        {
            VisualBrush brush = new()
            {
                Visual = image
            };
            return brush;
        }

        private double scale = 0;
        private readonly Image image;
    }
}
