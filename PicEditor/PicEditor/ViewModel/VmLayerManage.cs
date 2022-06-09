using Microsoft.Toolkit.Mvvm.Input;
using PicEditor.Basic.Util;
using PicEditor.Interface;
using PicEditor.Model.Layer;
using PicEditor.View.DialogWindow;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PicEditor.ViewModel
{
    internal class VmLayerManage : ILayerManage
    {
        private readonly CollectionUtil collectionUtil = new();
        private ILayerDisplay? layerDisplay = null;
        private const double unitLeft = 30;
        private int groupIndex = 0;
        private int layerIndex = 0;
        private ObservableCollection<LayerBase>? layers = null;
        public LayerPicture? selectedLayer = null;
        private RelayCommand<LayerBase>? addLayerGroupCommand = null;
        private RelayCommand<LayerBase>? addLayerPictureCommand = null;
        private RelayCommand<LayerBase>? deleteCommand = null;
        private RelayCommand<LayerBase>? moveUpCommand = null;
        private RelayCommand<LayerBase>? moveDownCommand = null;
        private RelayCommand<LayerBase>? renameCommand = null;

        public ObservableCollection<LayerBase> Layers => layers ??= new ObservableCollection<LayerBase>();

        public RelayCommand<LayerBase> AddLayerGroupCommand => addLayerGroupCommand ??= new RelayCommand<LayerBase>(AddLayerGroup);

        public RelayCommand<LayerBase> AddLayerPictureCommand => addLayerPictureCommand ??= new RelayCommand<LayerBase>(AddLayer);

        public RelayCommand<LayerBase> DeleteCommand => deleteCommand ??= new RelayCommand<LayerBase>(Delete);

        public RelayCommand<LayerBase> MoveUpCommand => moveUpCommand ??= new RelayCommand<LayerBase>(MoveUp);

        public RelayCommand<LayerBase> MoveDownCommand => moveDownCommand ??= new RelayCommand<LayerBase>(MoveDown);

        public RelayCommand<LayerBase> RenameCommand => renameCommand ??= new RelayCommand<LayerBase>(Rename);

        public VmLayerManage()
        {
        }

        public void Initialize(ILayerDisplay layerDisplay)
        {
            this.layerDisplay = layerDisplay;
        }

        #region 拖拽改变位置
        // 把source移动到target的位置或target内
        public void Relocation(LayerBase source, LayerBase target)
        {
            ObservableCollection<LayerBase>? sourceCollection = collectionUtil.FindCollection(Layers, source);
            ObservableCollection<LayerBase>? targetCollection = collectionUtil.FindCollection(Layers, target);
            if (sourceCollection == null || targetCollection == null)
            {
                LogUtil.Log.Error(new Exception("未完成拖拽"), $"{(sourceCollection == null ? "sourceCollection is null," : "")}{(targetCollection == null ? "targetCollection is null" : "")}");
                return;
            }

            if (source is LayerPicture picture)
            {
                if (target is LayerPicture)
                {
                    int index = targetCollection.IndexOf(target);
                    sourceCollection.Remove(source);
                    targetCollection.Insert(index, source);
                    source.MarginLeft = target.MarginLeft;
                }
                else if (target is LayerGroup group)
                {
                    sourceCollection.Remove(source);
                    source.MarginLeft = target.MarginLeft + unitLeft;
                    group.Children.Add(source);
                }
                picture.IsSelected = true;
            }
            else if (source is LayerGroup sourceGroup)
            {
                if (collectionUtil.FindCollection(sourceGroup.Children, target) != null)
                {
                    MessageBox.Show("移动失败，不能将该组移动到其下面");
                    return;
                }
                if (target is LayerPicture)
                {
                    int index = targetCollection.IndexOf(target);
                    sourceCollection.Remove(source);
                    targetCollection.Insert(index, source);
                    source.MarginLeft = target.MarginLeft;
                    foreach (LayerBase layer in sourceGroup.Children)
                    {
                        layer.MarginLeft = target.MarginLeft + unitLeft;
                    }
                }
                else if (target is LayerGroup targetGroup)
                {
                    sourceCollection.Remove(source);
                    source.MarginLeft = target.MarginLeft + unitLeft;
                    double left = source.MarginLeft + unitLeft;
                    foreach (LayerBase layer in sourceGroup.Children)
                    {
                        layer.MarginLeft = left;
                    }
                    targetGroup.Children.Add(source);
                }
            }
            NotifyLayersChanged();
            UpdateAllChildVisible(Layers, true);
        }

        // 把source移动到最外面
        public void Relocation(LayerBase source)
        {
            ObservableCollection<LayerBase>? sourceCollection = collectionUtil.FindCollection(Layers, source);
            if (sourceCollection == null)
            {
                LogUtil.Log.Error(new Exception("未完成拖拽"), $"{(sourceCollection == null ? "sourceCollection is null" : "")}");
                return;
            }

            sourceCollection.Remove(source);
            source.MarginLeft = 0;
            if (source is LayerGroup group)
            {
                foreach (LayerBase layerBase in group.Children)
                {
                    layerBase.MarginLeft = unitLeft;
                }
            }
            Layers.Add(source);
            NotifyLayersChanged();
            UpdateAllChildVisible(Layers, true);
        }

        // 移动了图层或组之后，层级顺序改变
        private void NotifyLayersChanged()
        {
            var list = new List<string>();
            collectionUtil.TreeToList(Layers, ref list);
            layerDisplay?.LayersChanged(list);
        }
        #endregion

        #region 图层选中回调
        private void SelectedChanged(LayerPicture picture)
        {
            if (!picture.Equals(selectedLayer) && selectedLayer != null && selectedLayer is LayerPicture _picture && _picture != null)
            {
                _picture.SetIsSelected(false);
            }
            selectedLayer = picture;
        }

        private void LayerGroupSelected()
        {
            if (selectedLayer != null && selectedLayer is LayerPicture picture && picture != null)
            {
                picture.SetIsSelected(true);
            }
        }
        #endregion

        #region 添加图层
        public void AddLayer(string guid, VisualBrush? brush, bool isInit)
        {
            var picture = new LayerPicture
            {
                Guid = guid,
                LayerName = $"图层 {++layerIndex}",
                LayerType = LayerTypes.Picture,
                Thumbnail = brush,
                MarginLeft = 0,
            };
            picture.SelectedChanged += SelectedChanged;
            picture.IsVisibleChanged += IsVisibleChanged;
            if (isInit)
            {
                DeleteGroup(Layers);
            }
            Layers.Insert(0, picture);
        }

        private void AddLayer(LayerBase? layerBase)
        {
            var picture = new LayerPicture
            {
                Guid = GuidUtil.GetGuid(),
                LayerName = $"图层 {++layerIndex}",
                LayerType = LayerTypes.Picture,
                Thumbnail = null,
                MarginLeft = 0,
            };
            picture.SelectedChanged += SelectedChanged;
            picture.IsVisibleChanged += IsVisibleChanged;

            string? previousGuid = null;
            if (layerBase == null)
            {
                Layers.Insert(0, picture);
            }
            else if (layerBase is LayerGroup layerGroup)
            {
                picture.MarginLeft = layerGroup.MarginLeft + unitLeft;
                layerGroup.Children.Insert(0, picture);
                previousGuid = collectionUtil.FindPreviousGuid(Layers, picture.Guid);
            }
            else if (layerBase is LayerPicture layerPicture)
            {
                ObservableCollection<LayerBase>? collection = collectionUtil.FindCollection(Layers, layerPicture);
                if (collection == null)
                {
                    LogUtil.Log.Error(new Exception("添加图层失败"), "AddLayer未找到collection");
                    picture.SelectedChanged -= SelectedChanged;
                    picture.IsVisibleChanged -= IsVisibleChanged;
                    return;
                }
                picture.MarginLeft = layerPicture.MarginLeft;
                collection.Insert(collection.IndexOf(layerPicture), picture);
                previousGuid = collectionUtil.FindPreviousGuid(Layers, picture.Guid);
            }
            layerDisplay?.LayerAdded(picture.Guid, previousGuid);
        }
        #endregion

        #region 添加组
        private void AddLayerGroup(LayerBase? layerBase)
        {
            var group = new LayerGroup
            {
                LayerName = $"组 {++groupIndex}",
                LayerType = LayerTypes.Group,
                MarginLeft = 0,
            };
            group.LayerGroupSelected += LayerGroupSelected;
            group.IsVisibleChanged += IsVisibleChanged;

            if (layerBase == null)
            {
                Layers.Insert(0, group);
            }
            else if (layerBase is LayerGroup layerGroup)
            {
                group.MarginLeft = layerGroup.MarginLeft + unitLeft;
                layerGroup.Children.Insert(0, group);
            }
            else if (layerBase is LayerPicture layerPicture)
            {
                ObservableCollection<LayerBase>? collection = collectionUtil.FindCollection(Layers, layerPicture);
                if (collection == null)
                {
                    LogUtil.Log.Error(new Exception("添加组失败"), "AddLayer未找到collection");
                    group.LayerGroupSelected -= LayerGroupSelected;
                    group.IsVisibleChanged -= IsVisibleChanged;
                    return;
                }
                group.MarginLeft = layerPicture.MarginLeft;
                collection.Insert(collection.IndexOf(layerPicture), group);
            }
        }
        #endregion

        #region 图层和组Visible
        private void IsVisibleChanged(LayerBase layerBase)
        {
            if (!collectionUtil.IsGroupVisible(Layers, layerBase))
            {
                return;
            }
            if (layerBase is LayerPicture picture)
            {
                layerDisplay?.SetLayerVisible(picture.Guid, picture.IsVisible ? Visibility.Visible : Visibility.Collapsed);
            }
            else if (layerBase is LayerGroup group)
            {
                UpdateAllChildVisible(group.Children, group.IsVisible);
            }
        }

        public void UpdateAllChildVisible(ObservableCollection<LayerBase> collection, bool isVisible)
        {
            for (int i = 0; i < collection.Count; ++i)
            {
                if (collection[i] is LayerPicture picture && picture != null)
                {
                    layerDisplay?.SetLayerVisible(picture.Guid, isVisible && picture.IsVisible ? Visibility.Visible : Visibility.Collapsed);
                }
                else if (collection[i] is LayerGroup group && group != null)
                {
                    UpdateAllChildVisible(group.Children, isVisible && group.IsVisible);
                }
            }
        }
        #endregion

        #region 图层缩略图
        public void SetLayerSize(string guid, int width, int height)
        {
            LayerPicture? picture = collectionUtil.FindLayerPicture(Layers, guid);
            if (picture == null)
            {
                LogUtil.Log.Error(new Exception("设置图层缩略图高度失败"), "picture为空");
                return;
            }
            picture.ThumbnailHeight = picture.ThumbnailWidth * height / width;
        }

        public void SetLayerThumbnail(string guid, VisualBrush brush)
        {
            LayerPicture? picture = collectionUtil.FindLayerPicture(Layers, guid);
            if (picture == null)
            {
                LogUtil.Log.Error(new Exception("设置图层缩略图失败"), "picture为空");
                return;
            }
            picture.Thumbnail = brush;
        }
        #endregion

        #region 删除图层、组
        private void Delete(LayerBase? layerBase)
        {
            if (layerBase == null)
            {
                if (selectedLayer == null)
                {
                    MessageBox.Show("当前未选择任何图层");
                    return;
                }
                layerBase = selectedLayer;
            }
            ObservableCollection<LayerBase>? collection = collectionUtil.FindCollection(Layers, layerBase);
            if (collection == null)
            {
                LogUtil.Log.Error(new Exception($"删除{(layerBase is LayerPicture ? "图层" : "组")}失败"), "collection为空");
                return;
            }
            if (layerBase is LayerPicture picture)
            {
                if (picture.Equals(selectedLayer))
                {
                    selectedLayer = null;
                }
                DeleteLayer(collection, picture);
            }
            else if (layerBase is LayerGroup group)
            {
                if (selectedLayer != null && collectionUtil.GroupContainers(group.Children, selectedLayer))
                {
                    selectedLayer = null;
                }
                DeleteGroup(group.Children);
                group.LayerGroupSelected -= LayerGroupSelected;
                group.IsVisibleChanged -= IsVisibleChanged;
                collection.Remove(group);
            }
        }

        private void DeleteLayer(ObservableCollection<LayerBase> collection, LayerPicture picture)
        {
            picture.SelectedChanged -= SelectedChanged;
            picture.IsVisibleChanged -= IsVisibleChanged;
            collection.Remove(picture);
            layerDisplay?.LayerDeleted(picture.Guid);
        }

        private void DeleteGroup(ObservableCollection<LayerBase> collection)
        {
            for (int i = collection.Count - 1; i >= 0; --i)
            {
                if (collection[i] is LayerPicture picture && picture != null)
                {
                    DeleteLayer(collection, picture);
                }
                else if (collection[i] is LayerGroup group && group != null)
                {
                    DeleteGroup(group.Children);
                    group.LayerGroupSelected -= LayerGroupSelected;
                    group.IsVisibleChanged -= IsVisibleChanged;
                    collection.Remove(group);
                }
            }
        }
        #endregion

        #region 图层、组的同层移动
        private void MoveUp(LayerBase? layerBase)
        {
            Move(layerBase, true);
        }

        private void MoveDown(LayerBase? layerBase)
        {
            Move(layerBase, false);
        }

        private void Move(LayerBase? layerBase, bool isMoveUp)
        {
            if (layerBase != null)
            {
                ObservableCollection<LayerBase>? collection = collectionUtil.FindCollection(Layers, layerBase);
                if (collection != null)
                {
                    int index = collection.IndexOf(layerBase);
                    if (isMoveUp && index > 0)
                    {
                        collection.Remove(layerBase);
                        collection.Insert(index - 1, layerBase);
                    }
                    else if (!isMoveUp && index + 1 < collection.Count)
                    {
                        collection.Remove(layerBase);
                        collection.Insert(index + 1, layerBase);
                    }
                    NotifyLayersChanged();
                }
            }
        }
        #endregion

        #region 修改图层、组的名称
        private void Rename(LayerBase? layerBase)
        {
            if (layerBase != null)
            {
                var window = new LayerRenameWindow(layerBase.LayerName);
                if (window.ShowDialog() == true)
                {
                    layerBase.LayerName = window.GetResult();
                }
            }
        }
        #endregion

        private class CollectionUtil
        {
            // 找出LayerBase所在的Collection
            public ObservableCollection<LayerBase>? FindCollection(ObservableCollection<LayerBase> collection, LayerBase source)
            {
                for (int i = 0; i < collection.Count; ++i)
                {
                    if (collection[i].Equals(source))
                    {
                        return collection;
                    }
                    if (collection[i] is LayerGroup group && group != null)
                    {
                        ObservableCollection<LayerBase>? _collection = FindCollection(group.Children, source);
                        if (_collection != null)
                        {
                            return _collection;
                        }
                    }
                }
                return null;
            }

            // 通过guid找出LayerPicture
            public LayerPicture? FindLayerPicture(ObservableCollection<LayerBase> collection, string guid)
            {
                for (int i = 0; i < collection.Count; ++i)
                {
                    if (collection[i] is LayerPicture picture && picture != null && picture.Guid == guid)
                    {
                        return picture;
                    }
                    else if (collection[i] is LayerGroup group && group != null)
                    {
                        LayerPicture? layerPicture = FindLayerPicture(group.Children, guid);
                        if (layerPicture != null)
                        {
                            return layerPicture;
                        }
                    }
                }
                return null;
            }

            // 树状结构转列表结构
            public void TreeToList(ObservableCollection<LayerBase> collection, ref List<string> list)
            {
                for (int i = 0; i < collection.Count; ++i)
                {
                    if (collection[i] is LayerPicture picture && picture != null)
                    {
                        list.Add(picture.Guid);
                    }
                    else if (collection[i] is LayerGroup group && group != null)
                    {
                        TreeToList(group.Children, ref list);
                    }
                }
            }

            // 找出前一个图层的guid
            public string? FindPreviousGuid(ObservableCollection<LayerBase> collection, string guid)
            {
                var list = new List<string>();
                TreeToList(collection, ref list);
                for(int i= 0; i < list.Count; ++i)
                {
                    if (list[i] == guid)
                    {
                        if (i + 1 >= list.Count)
                        {
                            return null;
                        }
                        return list[i + 1];
                    }
                }
                return null;
            }

            // 获取该组的所有上层组是否可见
            public bool IsGroupVisible(ObservableCollection<LayerBase> collection, LayerBase layerBase)
            {
                LayerGroup? _group;
                if (layerBase is LayerPicture picture && picture != null)
                {
                    LayerGroup? tempGroup = FindParentGroup(collection, picture);
                    if (tempGroup == null)
                    {
                        return true;
                    }
                    if (!tempGroup.IsVisible)
                    {
                        return false;
                    }
                    _group = tempGroup;
                }
                else
                {
                    _group = layerBase as LayerGroup;
                }

                bool result = true;
                while (true)
                {
                    if (_group == null)
                    {
                        LogUtil.Log.Error(new Exception("获取所在的组失败"), "_group为空");
                        return true;
                    }
                    _group = FindParentGroup(collection, _group);
                    if (_group == null)
                    {
                        return result;
                    }
                    if (!_group.IsVisible)
                    {
                        return false;
                    }
                }
            }

            // 获取该组下是否含有该图层
            public bool GroupContainers(ObservableCollection<LayerBase> collection, LayerPicture layerPicture)
            {
                for (int i = 0; i < collection.Count; ++i)
                {
                    if (collection[i] is LayerPicture picture && picture != null && picture.Equals(layerPicture))
                    {
                        return true;
                    }
                    else if (collection[i] is LayerGroup group && group != null)
                    {
                        if (GroupContainers(group.Children, layerPicture))
                        {
                            return true;
                        }
                    }
                }
                return false;
            }

            // 找出图层或组所在的组
            private LayerGroup? FindParentGroup(ObservableCollection<LayerBase> collection, LayerBase layerBase)
            {
                for (int i = 0; i < collection.Count; ++i)
                {
                    if (collection[i] is LayerGroup layerGroup && layerGroup != null)
                    {
                        if (layerGroup.Children.Contains(layerBase))
                        {
                            return layerGroup;
                        }
                        LayerGroup? _group = FindParentGroup(layerGroup.Children, layerBase);
                        if (_group != null)
                        {
                            return _group;
                        }
                    }
                }
                return null;
            }
        }
    }
}
