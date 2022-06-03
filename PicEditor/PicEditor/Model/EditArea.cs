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

        public double LeftOffset
        {
            get => leftOffset;
            set => SetProperty(ref leftOffset, value);
        }

        public double TopOffset
        {
            get => topOffset;
            set => SetProperty(ref topOffset, value);
        }
    }
}
