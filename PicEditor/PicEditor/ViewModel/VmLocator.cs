namespace PicEditor.ViewModel
{
    internal static class VmLocator
    {
        private static readonly VmFile vmFile;
        private static readonly VmEditMode vmEditMode;
        private static readonly VmLayerDisplay vmLayerDisplay;
        private static readonly VmLayerManage vmLayerManage;
        private static readonly VmInsertPicture vmInsertPicture;
        private static readonly VmLayer vmLayer;
        private static readonly VmLayerList vmLayerList;

        public static VmFile File => vmFile;

        public static VmEditMode EditMode => vmEditMode;

        public static VmLayerDisplay LayerDisplay => vmLayerDisplay;

        public static VmLayerManage LayerManage => vmLayerManage;

        public static VmInsertPicture InsertPicture => vmInsertPicture;

        public static VmLayer Layer => vmLayer;

        public static VmLayerList LayerList => vmLayerList;

        static VmLocator()
        {
            vmFile = new VmFile();
            vmEditMode = new VmEditMode();
            vmLayerDisplay = new VmLayerDisplay();
            vmLayerManage = new VmLayerManage();
            vmInsertPicture = new VmInsertPicture();
            vmLayer = new VmLayer();
            vmLayerList = new VmLayerList();
            ViewModelInit();
        }

        private static void ViewModelInit()
        {
            File.Initialize(Layer);
            Layer.Initialize(LayerList);
            LayerList.Initialize(Layer);

            vmLayerDisplay.Initialize(vmLayerManage, vmInsertPicture);
            vmLayerManage.Initialize(vmLayerDisplay);
            vmInsertPicture.Initialize(vmLayerDisplay);
        }
    }
}
