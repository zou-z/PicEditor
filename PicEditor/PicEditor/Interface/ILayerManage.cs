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
        // 设置初始的图层
        void AddLayer(string guid, VisualBrush brush);

        // 设置图层缩略图
        void SetLayerThumbnail(string guid, VisualBrush brush);

        // 设置图层缩略图的尺寸
        void SetLayerSize(string guid, int width, int height);
    }
}
