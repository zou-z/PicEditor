using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PicEditor.Model.PictureData
{
    internal class PictureStatus : ObservableObject
    {
        private bool isPictureOpened = false;
        private bool isFunctionOn = false;
        private bool isInsertingPicture = false;

        public bool IsPictureOpened
        {
            get => isPictureOpened;
            set => SetProperty(ref isPictureOpened, value);
        }

        public bool IsFunctionOn
        {
            get => isFunctionOn;
            set => SetProperty(ref isFunctionOn, value);
        }

        public bool IsInsertingPicture
        {
            get => isInsertingPicture;
            set
            {
                SetProperty(ref isInsertingPicture, value);
                IsFunctionOn = false;
            }
        }
    }
}
