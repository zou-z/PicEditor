using Microsoft.Toolkit.Mvvm.Input;
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
        private RelayCommand<PictureRotate>? rotateCommand = null;
        private RelayCommand<PictureMirror>? mirrorCommand = null;

        public PicturePosition Position => picturePosition ??= new PicturePosition();

        public RelayCommand<PictureRotate> RotateCommand => rotateCommand ??= new RelayCommand<PictureRotate>(Rotate);

        public RelayCommand<PictureMirror> MirrorCommand => mirrorCommand ??= new RelayCommand<PictureMirror>(Mirror);

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

        private void Rotate(PictureRotate rotate)
        {
            if (Position.Rotate == PictureRotate.None)
            {
                Position.Rotate = rotate;
                double offset = (Position.RealWidth - Position.RealHeight) / 2;
                Position.RealLeft += offset;
                Position.RealTop -= offset;
                (Position.RealHeight, Position.RealWidth) = (Position.RealWidth, Position.RealHeight);
            }
        }

        private void Mirror(PictureMirror mirror)
        {
            if (Position.Mirror == PictureMirror.None)
            {
                Position.Mirror = mirror;
            }
        }
    }
}
