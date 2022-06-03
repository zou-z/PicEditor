using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace PicEditor.Model
{
    internal class Picture : Location
    {
        private ImageSource? source = null;

        public ImageSource? Source
        {
            get => source;
            set => SetProperty(ref source, value);
        }
    }
}
