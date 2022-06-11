using PicEditor.Layer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PicEditor.Interface
{
    internal interface IPictureLayer : ILayer
    {
        string GetID();
    }
}
