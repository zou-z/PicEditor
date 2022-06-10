using PicEditor.Model.PictureInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PicEditor.ViewModel
{
    internal class VmInsertPicture
    {
        private PicturePosition? picturePosition = null;

        public PicturePosition Position => picturePosition ??= new PicturePosition();

        public VmInsertPicture()
        {

        }


    }
}
