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
    internal class VmLayerDisplay : IPictureSource, ILayerDisplay
    {
        private ILayerManage? layerManage = null;
        private LayerInfo? layerInfo = null;
        private readonly ObservableCollection<ILayer> pictureLayers;
        private readonly ObservableCollection<ILayer> upperLayers;

        public LayerInfo LayerInfo => layerInfo ??= new LayerInfo();

        public ObservableCollection<ILayer> PictureLayers => pictureLayers;

        public ObservableCollection<ILayer> UpperLayers => upperLayers;

        public void Initialize(ILayerManage layerManage)
        {
            this.layerManage = layerManage;
        }

        public VmLayerDisplay()
        {
            pictureLayers = new();
            upperLayers = new();
        }

        public void AddPictureSource(WriteableBitmap bitmap, bool isInit)
        {
            //if (isInit || LayerInfo.CanvasSize.Width == 0 && LayerInfo.CanvasSize.Height == 0)
            //{
            //    PictureLayers.Clear();
            //    LayerInfo.CanvasSize = new Size(bitmap.PixelWidth, bitmap.PixelHeight);
            //    LayerInfo.Scale = 1.0;
            //}
            //var item = new PictureLayer(bitmap)
            //{
            //    Guid = GuidUtil.GetGuid(),
            //    ImageWidth = bitmap.PixelWidth,
            //    ImageHeight = bitmap.PixelHeight,
            //};
            //item.SetSize(LayerInfo.CanvasSize.Width * LayerInfo.Scale, LayerInfo.CanvasSize.Height * LayerInfo.Scale, LayerInfo.Scale);
            //PictureLayers.Add(item);

            //layerManage?.AddLayer(item.Guid, item.GetVisualBrush(), isInit);
            //layerManage?.SetLayerSize(item.Guid, bitmap.PixelWidth, bitmap.PixelHeight);

            //// 插入图片
            //if (!isInit && PictureLayers.Count > 0)
            //{
            //    RectSelector selector;
            //    if (UpperLayers.Count == 0)
            //    {
            //        selector = new RectSelector() { DataContext = VmLocator.InsertPicture };
            //        selector.SetBinding(RectSelector.RealLeftProperty, new Binding("Position.RealLeft") { Mode = BindingMode.TwoWay });
            //        selector.SetBinding(RectSelector.RealTopProperty, new Binding("Position.RealTop") { Mode = BindingMode.TwoWay });
            //        selector.SetBinding(RectSelector.RealWidthProperty, new Binding("Position.RealWidth") { Mode = BindingMode.TwoWay });
            //        selector.SetBinding(RectSelector.RealHeightProperty, new Binding("Position.RealHeight") { Mode = BindingMode.TwoWay });
            //        UpperLayers.Add(selector);
            //    }
            //    else
            //    {
            //        selector = (RectSelector)UpperLayers[0];
            //    }

            //    item.SetBinding(PictureLayer.PositionProperty, new Binding("Position") { Source = VmLocator.InsertPicture.Position, Mode = BindingMode.OneWay });

            //    selector.Visibility = Visibility.Visible;
            //    VmLocator.InsertPicture.Position.RealWidth = bitmap.PixelWidth; // scale
            //    VmLocator.InsertPicture.Position.RealHeight = bitmap.PixelHeight; // scale
            //}



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
            layerManage?.SetLayerSize(image.GetID(), bitmap.PixelWidth, bitmap.PixelHeight);

            image.SetBinding(ImageEx.CanvasSizeProperty, new Binding("CanvasSize") { Source = LayerInfo, Mode = BindingMode.OneWay });
            image.SetBinding(ImageEx.ScaleProperty, new Binding("Scale") { Source = LayerInfo, Mode = BindingMode.OneWay });

            // 插入图片
            if (isInsertPicture)
            {
                RectSelector selector;
                if (UpperLayers.Count == 0)
                {
                    selector = new RectSelector();
                    selector.SetBinding(RectSelector.ScaleProperty, new Binding("Scale") { Source = LayerInfo, Mode = BindingMode.OneWay });
                    selector.SetBinding(RectSelector.RealLeftProperty, new Binding("RealLeft") { Source = VmLocator.InsertPicture.Position, Mode = BindingMode.TwoWay });
                    selector.SetBinding(RectSelector.RealTopProperty, new Binding("RealTop") { Source = VmLocator.InsertPicture.Position, Mode = BindingMode.TwoWay });
                    selector.SetBinding(RectSelector.RealWidthProperty, new Binding("RealWidth") { Source = VmLocator.InsertPicture.Position, Mode = BindingMode.TwoWay });
                    selector.SetBinding(RectSelector.RealHeightProperty, new Binding("RealHeight") { Source = VmLocator.InsertPicture.Position, Mode = BindingMode.TwoWay });
                    UpperLayers.Add(selector);
                }
                else
                {
                    selector = (RectSelector)UpperLayers[0];
                }

                image.SetBinding(ImageEx.RealLeftProperty, new Binding("RealLeft") { Source = VmLocator.InsertPicture.Position, Mode = BindingMode.OneWay });
                image.SetBinding(ImageEx.RealTopProperty, new Binding("RealTop") { Source = VmLocator.InsertPicture.Position, Mode = BindingMode.OneWay });
                image.SetBinding(ImageEx.RealWidthProperty, new Binding("RealWidth") { Source = VmLocator.InsertPicture.Position, Mode = BindingMode.OneWay });
                image.SetBinding(ImageEx.RealHeightProperty, new Binding("RealHeight") { Source = VmLocator.InsertPicture.Position, Mode = BindingMode.OneWay });

                selector.Visibility = Visibility.Visible;
                VmLocator.InsertPicture.Position.RealWidth = bitmap.PixelWidth;
                VmLocator.InsertPicture.Position.RealHeight = bitmap.PixelHeight;
            }
        }

        public void SetLayerVisible(string guid, Visibility visibility)
        {
            foreach (PictureLayer layer in PictureLayers)
            {
                if (layer.Guid == guid)
                {
                    layer.Visibility = visibility;
                    return;
                }
            }
        }

        public void LayerAdded(string guid, string? previousGuid)
        {
            var bitmap = FileUtil.GetTransparentBitmap((int)LayerInfo.CanvasSize.Width, (int)LayerInfo.CanvasSize.Height);
            var item = new PictureLayer(bitmap)
            {
                Guid = guid,
                ImageWidth = bitmap.PixelWidth,
                ImageHeight = bitmap.PixelHeight,
            };
            item.SetSize(LayerInfo.CanvasSize.Width * LayerInfo.Scale, LayerInfo.CanvasSize.Height * LayerInfo.Scale, LayerInfo.Scale);
            if (previousGuid == null)
            {
                PictureLayers.Add(item);
            }
            else
            {
                for (int i = 0; i < PictureLayers.Count; ++i)
                {
                    if (PictureLayers[i] is PictureLayer layer && layer != null && layer.Guid == previousGuid)
                    {
                        PictureLayers.Insert(i + 1, item);
                        break;
                    }
                }
            }
            layerManage?.SetLayerThumbnail(guid, item.GetVisualBrush());
            layerManage?.SetLayerSize(guid, bitmap.PixelWidth, bitmap.PixelHeight);
        }

        public void LayerDeleted(string guid)
        {
            for (int i = 0; i < PictureLayers.Count; ++i)
            {
                if (PictureLayers[i] is PictureLayer layer && layer != null && layer.Guid == guid)
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
                if (PictureLayers[i] is PictureLayer layer && layer != null && layer.Guid != layerList[i])
                {
                    for (int j = i + 1; j < layerList.Count; ++j)
                    {
                        if (PictureLayers[j] is PictureLayer tempLayer && tempLayer != null && tempLayer.Guid == layerList[i])
                        {
                            PictureLayers.Remove(tempLayer);
                            PictureLayers.Insert(i, tempLayer);
                            PictureLayers.Remove(layer);
                            PictureLayers.Insert(j, layer);
                            break;
                        }
                    }
                }
            }
        }
    }
}
