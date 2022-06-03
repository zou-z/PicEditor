using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PicEditor.Model
{
    internal class Location : ObservableObject
    {
        private double width = 0;
        private double height = 0;
        private double left = 0;
        private double top = 0;

        public double Width
        {
            get => width;
            set => SetProperty(ref width, value);
        }

        public double Height
        {
            get => height;
            set => SetProperty(ref height, value);
        }

        public double Left
        {
            get => left;
            set => SetProperty(ref left, value);
        }

        public double Top
        {
            get => top;
            set => SetProperty(ref top, value);
        }
    }
}
