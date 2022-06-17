namespace PicEditor.ViewModel
{
    internal static class VmLocator
    {
        private static readonly VmFile vmFile;
        private static readonly VmEditMode vmEditMode;
        private static readonly VmLayerDisplay vmLayerDisplay;
        private static readonly VmLayerManage vmLayerManage;
        private static readonly VmInsertPicture vmInsertPicture;

        public static VmFile File => vmFile;

        public static VmEditMode EditMode => vmEditMode;

        public static VmLayerDisplay LayerDisplay => vmLayerDisplay;

        public static VmLayerManage LayerManage => vmLayerManage;

        public static VmInsertPicture InsertPicture => vmInsertPicture;

        static VmLocator()
        {
            vmFile = new VmFile();
            vmEditMode = new VmEditMode();
            vmLayerDisplay = new VmLayerDisplay();
            vmLayerManage = new VmLayerManage();
            vmInsertPicture = new VmInsertPicture();
            ViewModelInit();
        }

        private static void ViewModelInit()
        {
            vmFile.Initialize(vmLayerDisplay);
            vmLayerDisplay.Initialize(vmLayerManage, vmInsertPicture);
            vmLayerManage.Initialize(vmLayerDisplay);
            vmInsertPicture.Initialize(vmLayerDisplay);
        }
    }
}
