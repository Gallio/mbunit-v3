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
using System.Text;
using System.Runtime.InteropServices;

namespace Gallio.Navigator.Native
{
    [ComImport]
    [Guid("79EAC9E1-BAF9-11CE-8C82-00AA004BA90B")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IInternetBindInfo
    {
        [PreserveSig]
        int GetBindInfo(
            [Out] [MarshalAs(UnmanagedType.U4)] out BINDF grfBINDF,
            [In, Out] ref BINDINFO pbindinfo);

        [PreserveSig]
        int GetBindString(
            [In] [MarshalAs(UnmanagedType.U4)] uint ulStringType,
            [In, Out] [MarshalAs(UnmanagedType.LPWStr)] ref string ppwzStr,
            [In] [MarshalAs(UnmanagedType.U4)] uint cEl,
            [In] [MarshalAs(UnmanagedType.U4)] ref uint pcElFetched);
    }
}
