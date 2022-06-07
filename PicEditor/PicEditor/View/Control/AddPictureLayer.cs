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
    internal class AddPictureLayer : Canvas, ILayer
    {
        public Image ImageControl => image;

        public AddPictureLayer(ImageData imageData)
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
            throw new NotImplementedException();
        }

        private readonly Image image;
    }
}
