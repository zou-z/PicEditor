using PicEditor.Interface;
using PicEditor.Model.PictureInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PicEditor.ViewModel
{
    internal class VmInsertPicture : IInsertPicture
    {
        private PicturePosition? picturePosition = null;

        public PicturePosition Position => picturePosition ??= new PicturePosition();

        public VmInsertPicture()
        {
        }

        public object GetPositionSource()
        {
            return Position;
        }

        public void InitData(int width, int height)
        {
            Position.InitData(width, height);
        }
    }
}
