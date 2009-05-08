using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Gallio.Common.Platform;
using Gallio.Common.Reflection;

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
                return DotNetRuntimeSupport.IsAssembly(stream);
            }

            return false;
        }
    }
}
