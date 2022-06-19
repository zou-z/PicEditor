using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace PicEditor.Model.PictureData
{
    internal class PictureSourceInfo : ObservableObject
    {
        private PictureSourceType sourceType = PictureSourceType.LocalFile;
        private string sourcePath = string.Empty;
        private int sourceSize = 0;
        public double sourceDpiX = 0;
        public double sourceDpiY = 0;
        public PixelFormat sourcePixelFormat = PixelFormats.Default;

        public string SourcePath
        {
            get => sourcePath;
            set => SetProperty(ref sourcePath, value);
        }

        public PictureSourceType SourceType
        {
            get => sourceType;
            set => SetProperty(ref sourceType, value);
        }

        public int SourceSize
        {
            get => sourceSize;
            set => SetProperty(ref sourceSize, value);
        }

        public double SourceDpiX
        {
            get => sourceDpiX;
            set => SetProperty(ref sourceDpiX, value);
        }

        public double SourceDpiY
        {
            get => sourceDpiY;
            set => SetProperty(ref sourceDpiY, value);
        }

        public PixelFormat SourcePixelFormat
        {
            get => sourcePixelFormat;
            set => SetProperty(ref sourcePixelFormat, value);
        }
    }
}
