// Copyright 2005-2009 Gallio Project - http://www.gallio.org/
// Portions Copyright 2000-2004 Jonathan de Halleux
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

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
        /// <param name="fileInspector">The file inspector, never null.</param>
        /// <returns>True if the file type was recognized</returns>
        bool IsRecognizedFile(IFileInspector fileInspector);
    }
}
