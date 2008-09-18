// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("79EAC9E5-BAF9-11CE-8C82-00AA004BA90B")]
    public interface IInternetProtocolSink
    {
        [PreserveSig]
        int Switch(
            ref PROTOCOLDATA pProtocolData);

        [PreserveSig]
        int ReportProgress(
            [MarshalAs(UnmanagedType.U4)] uint ulStatusCode,
            [MarshalAs(UnmanagedType.LPWStr)] string szStatusText);

        [PreserveSig]
        int ReportData(
            BSCF grfBSCF,
            [MarshalAs(UnmanagedType.U4)] uint ulProgress,
            [MarshalAs(UnmanagedType.U4)] uint ulProgressMax);

        [PreserveSig]
        int ReportResult(
            [MarshalAs(UnmanagedType.I4)] int hrResult,
            [MarshalAs(UnmanagedType.U4)] uint dwError,
            [MarshalAs(UnmanagedType.LPWStr)] string szResult);
    }
}
