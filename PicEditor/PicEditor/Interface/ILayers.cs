using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace PicEditor.Interface
{
    internal interface ILayers
    {
        Size GetCanvasSize();

        Brush? AddLayer(string id, string? previousID);


        


    }
}
