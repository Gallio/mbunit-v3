// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
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

using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Gallio.MbUnitCppAdapter.Model.Bridge;
using Gallio.Runtime.FileTypes;
using System;
using System.IO;
using Gallio.MbUnitCppAdapter.Model.PE;

namespace Gallio.MbUnitCppAdapter.Model
{
    /// <summary>
    /// Identifies valid unmanaged C++ test libraries for MbUnitCpp.
    /// </summary>
    public class MbUnitCppTestFileTypeRecognizer : IFileTypeRecognizer
    {
        /// <inheritdoc />
        public bool IsRecognizedFile(IFileInspector fileInspector)
        {
            Stream stream;

            if (!fileInspector.TryGetStream(out stream))
                return false;

            using (var reader = new PEImageReader(stream))
            {
                PEImageInfo info = reader.Read();
                return (info != null) && info.Exports.Contains("MbUnitCpp_RunTest");
            }
        }
    }
}
