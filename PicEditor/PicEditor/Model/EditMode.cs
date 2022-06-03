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
        private bool isContentMoveEnabled = true;

        public enum Modes
        {
            Move = 0,
            Select = 1,
        }

        public Modes Mode
        {
            get => mode;
            set
            {
                SetProperty(ref mode, value);
                IsContentMoveEnabled = mode == Modes.Move;
            }
        }

        public bool IsContentMoveEnabled
        {
            get => isContentMoveEnabled;
            set => SetProperty(ref isContentMoveEnabled, value);
        }
    }
}
