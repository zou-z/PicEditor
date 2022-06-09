using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PicEditor.Interface
{
    internal interface ILayerDisplay
    {
        // 添加图层
        void LayerAdded(string guid, string? previousGuid);

        // 删除图层
        void LayerDeleted(string guid);

        // 更改图层层级顺序
        void LayersChanged(List<string> layerList);

        // 图层或组是否可见改变
        void SetLayerVisible(string guid, Visibility visibility);
    }
}
