using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PicEditor.Interface
{
    internal interface IInsertPicture
    {
        object GetPositionSource();

        void InitData(string id, int width, int height);
    }
}
