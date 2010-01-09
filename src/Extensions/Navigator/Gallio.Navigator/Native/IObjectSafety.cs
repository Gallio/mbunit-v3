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

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Gallio.Navigator.Native
{
    [ComImport, Guid("CB5BDC81-93C1-11CF-8F20-00805F2CD064")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IObjectSafety
    {
        [PreserveSig]
        int GetInterfaceSafetyOptions(
            ref Guid riid,
            [MarshalAs(UnmanagedType.U4)] ref int pdwSupportedOptions,
            [MarshalAs(UnmanagedType.U4)] ref int pdwEnabledOptions);

        [PreserveSig]
        int SetInterfaceSafetyOptions(
            ref Guid riid,
            [MarshalAs(UnmanagedType.U4)] int dwOptionSetMask,
            [MarshalAs(UnmanagedType.U4)] int dwEnabledOptions);
    }
}
