namespace PicEditor.Layer
{
    public interface ILayer
    {
        /// <summary>
        /// 设置控件的宽度
        /// </summary>
        /// <param name="width">显示的宽度</param>
        /// <param name="height">显示的高度</param>
        /// <param name="scale">缩放倍数</param>
        void SetSize(double width, double height, double scale);
    }
}
