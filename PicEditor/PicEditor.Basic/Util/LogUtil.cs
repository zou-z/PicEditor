using NLog;

namespace PicEditor.Basic.Util
{
    public class LogUtil
    {
        public static Logger Log => logger ??= LogManager.GetCurrentClassLogger();

        private static Logger? logger = null;
    }
}
