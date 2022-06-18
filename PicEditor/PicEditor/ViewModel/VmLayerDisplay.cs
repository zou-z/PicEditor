using PicEditor.Basic.Util;
using PicEditor.Interface;
using PicEditor.Layer;
using PicEditor.Model.Layer;
using PicEditor.View.Control;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace PicEditor.ViewModel
{
    internal class VmLayerDisplay : IPictureSource, ILayerDisplay, ICanvas
    {
        private ILayerManage? layerManage = null;
        private IInsertPicture? insertPicture = null;
        private LayerInfo? layerInfo = null;
        private readonly ObservableCollection<UIElement> pictureLayers;
        private readonly ObservableCollection<UIElement> upperLayers;

        public LayerInfo LayerInfo => layerInfo ??= new LayerInfo();

        public ObservableCollection<UIElement> PictureLayers => pictureLayers;

        public ObservableCollection<UIElement> UpperLayers => upperLayers;

        public void Initialize(ILayerManage layerManage, IInsertPicture insertPicture)
        {
            this.layerManage = layerManage;
            this.insertPicture = insertPicture;
        }

        public VmLayerDisplay()
        {
            pictureLayers = new();
            upperLayers = new();
        }

        public void AddPictureSource(WriteableBitmap bitmap, bool isInit)
        {
            if (isInit || LayerInfo.CanvasSize.Width == 0 && LayerInfo.CanvasSize.Height == 0)
            {
                PictureLayers.Clear();
                LayerInfo.CanvasSize = new Size(bitmap.PixelWidth, bitmap.PixelHeight);
                LayerInfo.Scale = 1.0;
            }
            bool isInsertPicture = !isInit && PictureLayers.Count > 0;
            var image = new ImageEx(GuidUtil.GetGuid(), bitmap, !isInsertPicture);
            PictureLayers.Add(image);

            layerManage?.AddLayer(image.GetID(), image.GetVisualBrush(), isInit);
            layerManage?.SetLayerSize(image.GetID(), (int)LayerInfo.CanvasSize.Width, (int)LayerInfo.CanvasSize.Height);

            image.SetBinding(ImageEx.CanvasSizeProperty, new Binding("CanvasSize") { Source = LayerInfo, Mode = BindingMode.OneWay });
            image.SetBinding(ImageEx.ScaleProperty, new Binding("Scale") { Source = LayerInfo, Mode = BindingMode.OneWay });

            // 插入图片
            if (isInsertPicture)
            {
                InsertPictureBox insertBox;
                if (UpperLayers.Count == 0)
                {
                    insertBox = new InsertPictureBox();
                    insertBox.SetBinding(RectSelectorBase.ScaleProperty, new Binding("Scale") { Source = LayerInfo, Mode = BindingMode.OneWay });
                    insertBox.SetBinding(RectSelectorBase.RealLeftProperty, new Binding("RealLeft") { Source = insertPicture?.GetPositionSource(), Mode = BindingMode.TwoWay });
                    insertBox.SetBinding(RectSelectorBase.RealTopProperty, new Binding("RealTop") { Source = insertPicture?.GetPositionSource(), Mode = BindingMode.TwoWay });
                    insertBox.SetBinding(RectSelectorBase.RealWidthProperty, new Binding("RealWidth") { Source = insertPicture?.GetPositionSource(), Mode = BindingMode.TwoWay });
                    insertBox.SetBinding(RectSelectorBase.RealHeightProperty, new Binding("RealHeight") { Source = insertPicture?.GetPositionSource(), Mode = BindingMode.TwoWay });
                    insertBox.SetBinding(InsertPictureBox.WhRatioProperty, new Binding("WhRatio") { Source = insertPicture?.GetPositionSource(), Mode = BindingMode.OneWay });
                    insertBox.SetBinding(InsertPictureBox.IsKeepRatioProperty, new Binding("IsKeepRatio") { Source = insertPicture?.GetPositionSource(), Mode = BindingMode.OneWay });
                    UpperLayers.Add(insertBox);
                }
                else
                {
                    insertBox = (InsertPictureBox)UpperLayers[0];
                }

                image.SetBinding(ImageEx.RealLeftProperty, new Binding("RealLeft") { Source = insertPicture?.GetPositionSource(), Mode = BindingMode.OneWay });
                image.SetBinding(ImageEx.RealTopProperty, new Binding("RealTop") { Source = insertPicture?.GetPositionSource(), Mode = BindingMode.OneWay });
                image.SetBinding(ImageEx.RealWidthProperty, new Binding("RealWidth") { Source = insertPicture?.GetPositionSource(), Mode = BindingMode.OneWay });
                image.SetBinding(ImageEx.RealHeightProperty, new Binding("RealHeight") { Source = insertPicture?.GetPositionSource(), Mode = BindingMode.OneWay });
                image.SetBinding(ImageEx.RotateProperty, new Binding("Rotate") { Source = insertPicture?.GetPositionSource(), Mode = BindingMode.TwoWay });
                image.SetBinding(ImageEx.MirrorProperty, new Binding("Mirror") { Source = insertPicture?.GetPositionSource(), Mode = BindingMode.TwoWay });

                insertPicture?.InitData(bitmap.PixelWidth, bitmap.PixelHeight);
                insertBox.Visibility = Visibility.Visible;
            }
        }

        public void SetLayerVisible(string guid, Visibility visibility)
        {
            foreach (IPictureLayer layer in PictureLayers)
            {
                if (layer.GetID() == guid)
                {
                    layer.SetVisible(visibility);
                    return;
                }
            }
        }

        public void LayerAdded(string guid, string? previousGuid)
        {
            var bitmap = FileUtil.GetTransparentBitmap((int)LayerInfo.CanvasSize.Width, (int)LayerInfo.CanvasSize.Height);
            var image = new ImageEx(guid, bitmap);
            image.SetBinding(ImageEx.CanvasSizeProperty, new Binding("CanvasSize") { Source = LayerInfo, Mode = BindingMode.OneWay });
            image.SetBinding(ImageEx.ScaleProperty, new Binding("Scale") { Source = LayerInfo, Mode = BindingMode.OneWay });
            if (previousGuid == null)
            {
                PictureLayers.Add(image);
            }
            else
            {
                for (int i = 0; i < PictureLayers.Count; ++i)
                {
                    if (PictureLayers[i] is IPictureLayer layer && layer != null && layer.GetID() == previousGuid)
                    {
                        PictureLayers.Insert(i + 1, image);
                        break;
                    }
                }
            }
            layerManage?.SetLayerThumbnail(guid, image.GetVisualBrush());
            layerManage?.SetLayerSize(guid, bitmap.PixelWidth, bitmap.PixelHeight);
        }

        public void LayerDeleted(string guid)
        {
            for (int i = 0; i < PictureLayers.Count; ++i)
            {
                if (PictureLayers[i] is IPictureLayer layer && layer != null && layer.GetID() == guid)
                {
                    PictureLayers.RemoveAt(i);
                    return;
                }
            }
            LogUtil.Log.Error(new Exception("删除图层失败"), "未找到该图层");
        }

        public void LayersChanged(List<string> layerList)
        {
            layerList.Reverse();
            for (int i = 0; i < layerList.Count; ++i)
            {
                if (PictureLayers[i] is IPictureLayer layer && layer != null && layer.GetID() != layerList[i])
                {
                    for (int j = i + 1; j < layerList.Count; ++j)
                    {
                        if (PictureLayers[j] is IPictureLayer tempLayer && tempLayer != null && tempLayer.GetID() == layerList[i])
                        {
                            PictureLayers.Remove((UIElement)tempLayer);
                            PictureLayers.Insert(i, (UIElement)tempLayer);
                            PictureLayers.Remove((UIElement)layer);
                            PictureLayers.Insert(j, (UIElement)layer);
                            break;
                        }
                    }
                }
            }
        }

        public Size GetCanvasSize()
        {
            return LayerInfo.CanvasSize;
        }
    }
}
