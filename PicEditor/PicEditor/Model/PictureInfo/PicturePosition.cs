using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PicEditor.Model.PictureInfo
{
    internal class PicturePosition : ObservableObject
    {
        //private bool isCheckBoundary;
        private double realLeft = 0;
        private double realTop = 0;
        private double realWidth = 0;
        private double realHeight = 0;
        private Rect position = new();

        public double RealLeft
        {
            get => realLeft;
            set
            {
                SetProperty(ref realLeft, (int)value);
                UpdatePosition();
            } 
        }

        public double RealTop
        {
            get => realTop;
            set
            {
                SetProperty(ref realTop, (int)value);
                UpdatePosition();
            }
        }

        public double RealWidth
        {
            get => realWidth;
            set
            {
                SetProperty(ref realWidth, (int)value);
                UpdatePosition();
            }
        }

        public double RealHeight
        {
            get => realHeight;
            set
            {
                SetProperty(ref realHeight, (int)value);
                UpdatePosition();
            }
        }

        public Rect Position
        {
            get => position;
            set => SetProperty(ref position, value);
        }

        private void UpdatePosition()
        {
            Position = new Rect(RealLeft, RealTop, RealWidth, RealHeight);
        }
    }
}
