using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace PicEditor.Interface
{
    internal interface ILayerSource
    {
        void AddLayer(WriteableBitmap bitmap);
    }
}
