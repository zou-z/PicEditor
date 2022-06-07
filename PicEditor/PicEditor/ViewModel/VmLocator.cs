namespace PicEditor.ViewModel
{
    internal static class VmLocator
    {
        private static readonly VmEdit vmEdit;
        private static readonly VmFile vmFile;
        private static readonly VmEditMode vmEditMode;
        private static readonly VmLayer vmLayer;

        public static VmEdit Edit => vmEdit;

        public static VmFile File => vmFile;

        public static VmEditMode EditMode => vmEditMode;

        public static VmLayer Layer => vmLayer;

        static VmLocator()
        {
            vmEdit = new VmEdit();
            vmFile = new VmFile();
            vmEditMode = new VmEditMode();
            vmLayer=new VmLayer();
            vmFile.Initialize(vmEdit);
            vmEdit.Initialize(vmLayer);
        }
    }
}
