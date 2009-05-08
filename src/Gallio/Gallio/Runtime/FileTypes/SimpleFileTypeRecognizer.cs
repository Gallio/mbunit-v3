using System;
using System.Collections.Generic;
using System.Text;

namespace Gallio.Runtime.FileTypes
{
    /// <summary>
    /// A simple file type recognizer that does not perform any filtering
    /// beyond that which is already specified by its associated traits.
    /// </summary>
    public class SimpleFileTypeRecognizer : IFileTypeRecognizer
    {
        /// <inheritdoc />
        public bool IsRecognizedFile(IFileInspector fileInspector)
        {
            return true;
        }
    }
}
