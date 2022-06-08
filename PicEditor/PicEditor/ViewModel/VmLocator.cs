namespace PicEditor.ViewModel
{
    internal static class VmLocator
    {
        private static readonly VmFile vmFile;
        private static readonly VmEditMode vmEditMode;
        private static readonly VmLayerDisplay vmLayerDisplay;
        private static readonly VmLayerManage vmLayerManage;

        public static VmFile File => vmFile;

        public static VmEditMode EditMode => vmEditMode;

        public static VmLayerDisplay LayerDisplay => vmLayerDisplay;

        public static VmLayerManage LayerManage => vmLayerManage;

        static VmLocator()
        {
            vmFile = new VmFile();
            vmEditMode = new VmEditMode();
            vmLayerDisplay = new VmLayerDisplay();
            vmLayerManage = new VmLayerManage();
            vmFile.Initialize(vmLayerDisplay);
            vmLayerDisplay.Initialize(vmLayerManage);
            vmLayerManage.Initialize(vmLayerDisplay);
        }
    }
}
