using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Gallio.Icarus.Utilities
{
    internal static class Dialogs
    {
        public static OpenFileDialog OpenDialog
        {
            get
            {
                const string fileFilter = "Assemblies or Executables (*.dll, *.exe)|*.dll;*.exe|All Files (*.*)|*.*";
                var openFileDialog = new OpenFileDialog
                {
                    Filter = fileFilter,
                    Multiselect = true
                };
                return openFileDialog;
            }
        }
    }
}
