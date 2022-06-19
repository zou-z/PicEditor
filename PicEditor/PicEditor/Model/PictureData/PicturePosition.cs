using Microsoft.Toolkit.Mvvm.ComponentModel;
using PicEditor.Basic.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PicEditor.Model.PictureData
{
    internal class PicturePosition : ObservableObject
    {
        //private bool isCheckBoundary;
        private double realLeft = 0;
        private double realTop = 0;
        private double realWidth = 0;
        private double realHeight = 0;
        private int initialWidth = 0;
        private int initialHeight = 0;
        private double whRatio = 0;
        private bool isKeepRatio = true;
        private PictureRotate rotate = PictureRotate.None;
        private PictureMirror mirror = PictureMirror.None;

        public double RealLeft
        {
            get => realLeft;
            set => SetProperty(ref realLeft, (int)value);
        }

        public double RealTop
        {
            get => realTop;
            set => SetProperty(ref realTop, (int)value);
        }

        public double RealWidth
        {
            get => realWidth;
            set => SetProperty(ref realWidth, (int)(value < 1 ? 1 : value));
        }

        public double RealHeight
        {
            get => realHeight;
            set => SetProperty(ref realHeight, (int)(value < 1 ? 1 : value));
        }

        public int InitialWidth => initialWidth;

        public int InitialHeight => initialHeight;

        public double WhRatio
        {
            get => whRatio;
            private set => SetProperty(ref whRatio, value);
        }

        public bool IsKeepRatio
        {
            get => isKeepRatio;
            set => SetProperty(ref isKeepRatio, value);
        }

        public PictureRotate Rotate
        {
            get => rotate;
            set => SetProperty(ref rotate, value);
        }

        public PictureMirror Mirror
        {
            get => mirror;
            set => SetProperty(ref mirror, value);
        }

        public void InitData(int width, int height)
        {
            SetProperty(ref realLeft, 0, nameof(RealLeft));
            SetProperty(ref realTop, 0, nameof(RealTop));
            SetProperty(ref realWidth, width, nameof(RealWidth));
            SetProperty(ref realHeight, height, nameof(RealHeight));
            SetProperty(ref initialWidth, width, nameof(InitialWidth));
            SetProperty(ref initialHeight, height, nameof(InitialHeight));
            SetProperty(ref isKeepRatio, true, nameof(IsKeepRatio));
            SetProperty(ref rotate, PictureRotate.None);
            SetProperty(ref mirror, PictureMirror.None);
            if (RealHeight == 0)
            {
                LogUtil.Log.Error(new Exception("图片高度为0"), "RealHeight为0");
                SetProperty(ref whRatio, 0, nameof(WhRatio));
            }
            else
            {
                SetProperty(ref whRatio, RealWidth / RealHeight, nameof(WhRatio));
            }
        }
    }
}
