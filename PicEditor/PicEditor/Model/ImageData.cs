using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PicEditor.Model
{
    internal class ImageData
    {
        public int Width { get; set; } = 0;

        public int Height { get; set; } = 0;

        public byte[]? Pixels { get; set; } = null;

        public double DpiX { get; set; } = 0;

        public double DpiY { get; set; } = 0;

        public PixelFormat PixelFormat { get; set; }

        public BitmapPalette? Palette { get; set; }
    }
}
