using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace PicEditor.Interface
{
    internal interface ILayerList
    {
        void AddLayer(string id, VisualBrush brush);

    }
}
