using PicEditor.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PicEditor.ViewModel
{
    internal class VmEditMode
    {
        private EditMode? editMode = null;

        public EditMode EditMode => editMode ??= new EditMode();
    }
}
