using System;
using System.Collections.ObjectModel;

namespace PicEditor.Model.Layer
{
    internal class LayerGroup : LayerBase
    {
        private ObservableCollection<LayerBase>? children = null;

        public event Action? LayerGroupSelected = null;

        public ObservableCollection<LayerBase> Children => children ??= new ObservableCollection<LayerBase>();

        public bool IsSelected
        {
            get => false;
            set => LayerGroupSelected?.Invoke();
        }
    }
}
