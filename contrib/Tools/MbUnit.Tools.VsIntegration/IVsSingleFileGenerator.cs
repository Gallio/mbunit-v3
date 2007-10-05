// Copyright 2007 MbUnit Project - http://www.mbunit.com/
// Portions Copyright 2000-2004 Jonathan De Halleux, Jamie Cansdale
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
using System.Runtime.InteropServices;

namespace MbUnit.Tools.VsIntegration
{
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("3634494C-492F-4F91-8009-4541234E4E99")]
    internal interface IVsSingleFileGenerator
    {
        [return: MarshalAs(UnmanagedType.BStr)]
        string GetDefaultExtension();

        void Generate([In, MarshalAs(UnmanagedType.LPWStr)] string wszInputFilePath,
            [In, MarshalAs(UnmanagedType.BStr)] string bstrInputFileContents,
            [In, MarshalAs(UnmanagedType.LPWStr)] string wszDefaultNamespace,
            out IntPtr pbstrOutputFileContents,
            [MarshalAs(UnmanagedType.U4)] out int pbstrOutputFileContentsSize,
            [In, MarshalAs(UnmanagedType.Interface)] IVsGeneratorProgress pGenerateProgress);
    }
}
