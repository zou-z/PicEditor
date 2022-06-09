using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PicEditor.Model.EditMode
{
    internal enum EditMode
    {
        View,               // 查看
        Select,             // 选择
        Move,               // 移动
        Paint,              // 画笔
        Straw,              // 吸管
        Ruler,              // 尺子
        Text,               // 文本
        Fill,               // 颜色填充
    }
}
