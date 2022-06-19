using Microsoft.Toolkit.Mvvm.Input;
using PicEditor.Interface;
using PicEditor.Model.PictureData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PicEditor.ViewModel
{
    internal class VmInsertPicture : IInsertPicture
    {
        private ICanvas? canvas = null;
        private PicturePosition? picturePosition = null;
        private RelayCommand<PictureRotate>? rotateCommand = null;
        private RelayCommand<PictureMirror>? mirrorCommand = null;
        private RelayCommand<PictureAlign>? alignCommand = null;

        public PicturePosition Position => picturePosition ??= new PicturePosition();

        public RelayCommand<PictureRotate> RotateCommand => rotateCommand ??= new RelayCommand<PictureRotate>(Rotate);

        public RelayCommand<PictureMirror> MirrorCommand => mirrorCommand ??= new RelayCommand<PictureMirror>(Mirror);

        public RelayCommand<PictureAlign> AlignCommand => alignCommand ??= new RelayCommand<PictureAlign>(Align);

        public VmInsertPicture()
        {
        }

        public void Initialize(ICanvas canvas)
        {
            this.canvas = canvas;
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

        private void Align(PictureAlign align)
        {
            if (align == PictureAlign.AlignLeft)
            {
                Position.RealLeft = 0;
            }
            else if (align == PictureAlign.AlignTop)
            {
                Position.RealTop = 0;
            }
            else if (align == PictureAlign.AlignRight && canvas != null)
            {
                Position.RealLeft = canvas.GetCanvasSize().Width - Position.RealWidth;
            }
            else if (align == PictureAlign.AlignBottom && canvas != null)
            {
                Position.RealTop = canvas.GetCanvasSize().Height - Position.RealHeight;
            }
            else if (align == PictureAlign.AlignHorizontalCenter && canvas != null)
            {
                Position.RealLeft = (canvas.GetCanvasSize().Width - Position.RealWidth) / 2;
            }
            else if (align == PictureAlign.AlignVerticalCenter && canvas != null)
            {
                Position.RealTop = (canvas.GetCanvasSize().Height - Position.RealHeight) / 2;
            }
        }
    }
}
