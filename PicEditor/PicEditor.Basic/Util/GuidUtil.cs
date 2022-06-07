using System;

namespace PicEditor.Basic.Util
{
    public class GuidUtil
    {
        public static string GetGuid()
        {
            return Guid.NewGuid().ToString("N");
        }
    }
}
