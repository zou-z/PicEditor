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
        // 打开图片
        // 添加图片

        void AddLayerPicture(VisualBrush brush, string guid);


    }
}
