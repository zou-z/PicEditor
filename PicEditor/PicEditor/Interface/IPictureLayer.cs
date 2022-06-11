using PicEditor.Layer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PicEditor.Interface
{
    internal interface IPictureLayer
    {
        string GetID();

        void SetVisible(Visibility visiblity);
    }
}
