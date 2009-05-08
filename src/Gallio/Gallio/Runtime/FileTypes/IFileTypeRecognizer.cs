using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Gallio.Runtime.Extensibility;

namespace Gallio.Runtime.FileTypes
{
    /// <summary>
    /// Recognizes the type of a file.
    /// </summary>
    [Traits(typeof(FileTypeRecognizerTraits))]
    public interface IFileTypeRecognizer
    {
        /// <summary>
        /// Returns true if the recognizer recognizes the file described by the file inspector.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method is guaranteed to be called only when the file already matches the
        /// constraints described by the file type recognizer's traits.  For example, if the
        /// traits specify a file name regular expression then the recognizer will only be
        /// called if the file in question actually has a matching name.
        /// </para>
        /// </remarks>
        /// <param name="fileInspector">The file inspector, never null</param>
        /// <returns>True if the file type was recognized</returns>
        bool IsRecognizedFile(IFileInspector fileInspector);
    }
}