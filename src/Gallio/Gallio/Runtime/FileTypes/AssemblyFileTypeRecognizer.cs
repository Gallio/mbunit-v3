using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Gallio.Reflection;

namespace Gallio.Runtime.FileTypes
{
    /// <summary>
    /// Recognizes .Net assemblies by looking for the CLR header within the file.
    /// </summary>
    public class AssemblyFileTypeRecognizer : IFileTypeRecognizer
    {
        /// <inheritdoc />
        public bool IsRecognizedFile(IFileInspector fileInspector)
        {
            Stream stream;
            if (fileInspector.TryGetStream(out stream))
            {
                return AssemblyUtils.IsAssembly(stream);
            }

            return false;
        }
    }
}
