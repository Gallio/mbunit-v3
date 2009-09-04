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
using System.Runtime.InteropServices;
using System.Text;

namespace Gallio.Navigator.Native
{
    [ComImport]
    [Guid("79eac9ec-baf9-11ce-8c82-00aa004ba90b")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IInternetProtocolInfo
    {
        [PreserveSig]
        int ParseUrl(
            [In] [MarshalAs(UnmanagedType.LPWStr)] string pwzUrl,
            [In] PARSEACTION ParseAction,
            [In] [MarshalAs(UnmanagedType.U4)] uint dwParseFlags,
            [In] IntPtr pwzResult,
            [In] [MarshalAs(UnmanagedType.U4)] uint cchResult,
            [Out] [MarshalAs(UnmanagedType.U4)] out uint pcchResult,
            [In] [MarshalAs(UnmanagedType.U4)] uint dwReserved);

        [PreserveSig]
        int CombineUrl(
            [In] [MarshalAs(UnmanagedType.LPWStr)] string pwzBaseUrl,
            [In] [MarshalAs(UnmanagedType.LPWStr)] string pwzRelativeUrl,
            [In] [MarshalAs(UnmanagedType.U4)] uint dwCombineFlags,
            [In] IntPtr pwzResult,
            [In] [MarshalAs(UnmanagedType.U4)] uint cchResult,
            [Out] [MarshalAs(UnmanagedType.U4)] out uint pcchResult,
            [In] [MarshalAs(UnmanagedType.U4)] uint dwReserved);

        [PreserveSig]
        int CompareUrl(
            [In] [MarshalAs(UnmanagedType.LPWStr)] string pwzUrl1,
            [In] [MarshalAs(UnmanagedType.LPWStr)] string pwzUrl2,
            [In] [MarshalAs(UnmanagedType.U4)] uint dwCompareFlags);

        [PreserveSig]
        int QueryInfo(
            [In] [MarshalAs(UnmanagedType.LPWStr)] string pwzUrl,
            [In] QUERYOPTION OueryOption,
            [In] [MarshalAs(UnmanagedType.U4)] uint dwQueryFlags,
            [In] IntPtr pBuffer,
            [In] [MarshalAs(UnmanagedType.U4)] uint cbBuffer,
            [In, Out] [MarshalAs(UnmanagedType.U4)] ref uint pcbBuf,
            [In] [MarshalAs(UnmanagedType.U4)] uint dwReserved);
    }
}
