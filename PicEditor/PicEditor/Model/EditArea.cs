using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PicEditor.Model
{
    internal class EditArea : ObservableObject
    {
        private double scale = 1.0;
        private double leftOffset = 0;
        private double topOffset = 0;

        public double Scale
        {
            get => scale;
            set => SetProperty(ref scale, value);
        }

        // 滚动条横向移动距离
        public double LeftOffset
        {
            get => leftOffset;
            set => SetProperty(ref leftOffset, value);
        }

        // 滚动条纵向移动距离
        public double TopOffset
        {
            get => topOffset;
            set => SetProperty(ref topOffset, value);
        }
    }
}
