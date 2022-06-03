namespace PicEditor.ViewModel
{
    internal static class VmLocator
    {
        private static readonly VmEdit vmEdit;
        private static readonly VmFile vmFile;
        private static readonly VmEditMode vmEditMode;

        public static VmEdit Edit => vmEdit;

        public static VmFile File => vmFile;

        public static VmEditMode EditMode => vmEditMode;

        static VmLocator()
        {
            vmEdit = new VmEdit();
            vmFile = new VmFile();
            vmEditMode = new VmEditMode();
            vmFile.Initialize(vmEdit);

        }
    }
}
