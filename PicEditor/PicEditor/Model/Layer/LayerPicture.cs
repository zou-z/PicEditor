using System;
using System.Windows.Media;

namespace PicEditor.Model.Layer
{
    internal class LayerPicture : LayerBase
    {
        private Brush? thumbnail = null;
        private bool isSelected = false;

        public event Action<LayerPicture>? SelectedChanged = null;

        public string Guid { get; set; } = string.Empty;

        public Brush? Thumbnail
        {
            get => thumbnail;
            set => SetProperty(ref thumbnail, value);
        }

        public bool IsSelected
        {
            get => isSelected;
            set
            {
                SetIsSelected(value);
                if (value)
                {
                    SelectedChanged?.Invoke(this);
                }
            }
        }

        public void SetIsSelected(bool value)
        {
            SetProperty(ref isSelected, value, nameof(IsSelected));
        }
    }
}
