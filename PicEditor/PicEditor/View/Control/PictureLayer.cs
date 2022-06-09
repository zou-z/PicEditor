using PicEditor.Layer;
using PicEditor.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public PictureLayer(WriteableBitmap bitmap)
        {
            image = new Image { Source = bitmap };
            Children.Add(image);
            Background = Brushes.Transparent;
            EditingMode = InkCanvasEditingMode.None;
        }

        public void SetSize(double width, double height, double scale)
        {
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

        private readonly Image image;
    }
}
