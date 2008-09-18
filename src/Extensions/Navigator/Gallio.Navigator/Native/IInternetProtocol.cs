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
using System.Runtime.InteropServices;

namespace Gallio.Navigator.Native
{
    [ComImport]
    [Guid("79EAC9E4-BAF9-11CE-8C82-00AA004BA90B")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IInternetProtocol
    {
        #region Copied from IInternetProtocolRoot
        [PreserveSig]
        int Start(
            [In] [MarshalAs(UnmanagedType.LPWStr)] string szUrl,
            [In] IInternetProtocolSink protocolSink,
            [In] IInternetBindInfo bindInfo,
            [In] [MarshalAs(UnmanagedType.U4)] uint grfPI,
            [In] [MarshalAs(UnmanagedType.U4)] uint dwReserved);

        [PreserveSig]
        int Continue(
            [In] ref PROTOCOLDATA protocolData);

        [PreserveSig]
        int Abort(
            [In] [MarshalAs(UnmanagedType.I4)] int hrReason,
            [In] [MarshalAs(UnmanagedType.U4)] uint dwOptions);

        [PreserveSig]
        int Terminate(
            [In] [MarshalAs(UnmanagedType.U4)] uint dwOptions);

        [PreserveSig]
        int Suspend();

        [PreserveSig]
        int Resume();
        #endregion

        [PreserveSig]
        int Read(
            [In] IntPtr pv,
            [In] [MarshalAs(UnmanagedType.U4)] uint cb,
            [Out] [MarshalAs(UnmanagedType.U4)] out uint pcbRead);

        [PreserveSig]
        int Seek(
            [In] [MarshalAs(UnmanagedType.U8)] long dlibMove,
            [In] [MarshalAs(UnmanagedType.U4)] uint dwOrigin,
            [Out] [MarshalAs(UnmanagedType.U8)] out ulong plibNewPosition);

        [PreserveSig]
        int LockRequest(
            [In] [MarshalAs(UnmanagedType.U4)] uint dwOptions);

        [PreserveSig]
        int UnlockRequest();
    }
}
