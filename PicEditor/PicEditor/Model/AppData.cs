using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PicEditor.Model
{
    internal static class AppData
    {
        private static PictureSourceInfo? pictureSourceInfo = null;

        public static PictureSourceInfo PictureSourceInfo => pictureSourceInfo ??= new PictureSourceInfo();






    }

    internal enum PictureSourceType
    {
        LocalFile,
        NetworkLink,
        Clipboard,
    }

    internal struct PictureSourceInfo
    {
        public string Name;

        public string Path;

        public ulong Length;

        public PictureSourceType Type;

        public int Width;

        public int Height;

        public double DpiX;

        public double DpiY;

        public PixelFormat PixelFormat;

        public BitmapPalette? Palette;
    }
}
