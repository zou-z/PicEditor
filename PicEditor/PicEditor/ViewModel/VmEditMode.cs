using Microsoft.Toolkit.Mvvm.ComponentModel;
using PicEditor.Model;
using PicEditor.Model.EditMode;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PicEditor.ViewModel
{
    internal class VmEditMode : ObservableObject
    {
        private readonly ObservableCollection<EditMode> modes;
        private EditMode selectedMode;
        private bool isCanvasMoveEnabled = true;

        public ObservableCollection<EditMode> Modes => modes;

        public EditMode SelectedMode
        {
            get => selectedMode;
            set
            {
                SetProperty(ref selectedMode, value);
                IsCanvasMoveEnabled = value == EditMode.View;
            }
        }

        public bool IsCanvasMoveEnabled
        {
            get => isCanvasMoveEnabled;
            set => SetProperty(ref isCanvasMoveEnabled, value);
        }

        public VmEditMode()
        {
            modes = new ObservableCollection<EditMode>
            {
                EditMode.View,
            };
            selectedMode = modes[0];
        }
    }
}
