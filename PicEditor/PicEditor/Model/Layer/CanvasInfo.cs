using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PicEditor.Model.Layer
{
    internal class CanvasInfo : ObservableObject
    {
        private Size canvasSize = new(0, 0);
        private double scale = 1.0;

        // 画布大小
        public Size CanvasSize
        {
            get => canvasSize;
            set => SetProperty(ref canvasSize, value);
        }

        public double Scale
        {
            get => scale;
            set => SetProperty(ref scale, value);
        }
    }
}
