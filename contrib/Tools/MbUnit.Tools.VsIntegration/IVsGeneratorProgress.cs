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

using System.Runtime.InteropServices;

namespace MbUnit.Tools.VsIntegration
{
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("BED89B98-6EC9-43CB-B0A8-41D6E2D6669D")]
    internal interface IVsGeneratorProgress
    {
        [return: MarshalAs(UnmanagedType.U4)]
        void GeneratorError(
            [In, MarshalAs(UnmanagedType.Bool)] bool fWarning,
            [In, MarshalAs(UnmanagedType.U4)] int dwLevel,
            [In, MarshalAs(UnmanagedType.BStr)] string bstrError,
            [In, MarshalAs(UnmanagedType.U4)] int dwLine,
            [In, MarshalAs(UnmanagedType.U4)] int dwColumn);

        [return: MarshalAs(UnmanagedType.U4)]
        void Progress(
            [In, MarshalAs(UnmanagedType.U4)] int nComplete,
            [In, MarshalAs(UnmanagedType.U4)] int nTotal);
    }
}
