using PicEditor.Layer;
using PicEditor.Model;
using PicEditor.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace PicEditor.View.Control
{
    internal class PictureLayer : Canvas, ILayer
    {
        public PictureLayer(ImageData imageData)
        {
            Width = imageData.Width;
            Height = imageData.Height;
            image = new Image
            {
                Source = FileUtil.ImageDataToImageSource(imageData),
                Width = Width,
                Height = Height,
            };
            Children.Add(image);
            RenderOptions.SetBitmapScalingMode(image, BitmapScalingMode.NearestNeighbor);
        }

        public void SetSize(double width, double height, double scale)
        {
            Width = image.Width = width;
            Height = image.Height = height;
            RenderOptions.SetBitmapScalingMode(image, scale >= 4 ? BitmapScalingMode.NearestNeighbor : BitmapScalingMode.Linear);
        }

        private readonly Image image;
    }
}
