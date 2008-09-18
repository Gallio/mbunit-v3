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
