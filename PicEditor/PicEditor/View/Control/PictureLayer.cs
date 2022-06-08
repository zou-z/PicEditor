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
    internal class PictureLayer : InkCanvas, ILayer
    {
        public string Guid { get; set; } = string.Empty;

        public PictureLayer(WriteableBitmap bitmap, double scale = 1)
        {
            image = new Image { Source = bitmap };
            Children.Add(image);
            Background = Brushes.Transparent;
            EditingMode = InkCanvasEditingMode.None;
            SetSize(bitmap.PixelWidth * scale, bitmap.PixelHeight * scale, scale);
        }

        public void SetSize(double width, double height, double scale)
        {
            Width = image.Width = width;
            Height = image.Height = height;
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
