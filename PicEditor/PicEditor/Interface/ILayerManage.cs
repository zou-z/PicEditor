using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PicEditor.Interface
{
    internal interface ILayerManage
    {
        void AddLayer(string guid, VisualBrush brush);

        // void SetLayerThumbnail(VisualBrush brush);

        void SetLayerSize(string guid, int width, int height);
    }
}
