using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PicEditor.Model
{
    internal class EditMode : ObservableObject
    {
        private Modes mode = Modes.Move;
        private bool isCanvasMoveEnabled = true;

        public enum Modes
        {
            Move = 0,
            Select = 1,
            AddPicture = 50,
        }

        public Modes Mode
        {
            get => mode;
            set
            {
                SetProperty(ref mode, value);
                IsCanvasMoveEnabled = mode == Modes.Move;
            }
        }

        public bool IsCanvasMoveEnabled
        {
            get => isCanvasMoveEnabled;
            set => SetProperty(ref isCanvasMoveEnabled, value);
        }
    }
}
