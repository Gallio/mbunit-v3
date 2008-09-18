using System;
using System.Runtime.InteropServices;

namespace Gallio.Navigator.Native
{
    [ComImport]
    [Guid("79EAC9E3-BAF9-11CE-8C82-00AA004BA90B")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IInternetProtocolRoot
    {
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
    }
}
