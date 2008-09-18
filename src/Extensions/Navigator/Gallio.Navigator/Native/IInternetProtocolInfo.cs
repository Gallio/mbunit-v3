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
            [Out] [MarshalAs(UnmanagedType.LPWStr)] out string pwzResult,
            [In] [MarshalAs(UnmanagedType.U4)] uint cchResult,
            [Out] [MarshalAs(UnmanagedType.U4)] out uint pcchResult,
            [In] [MarshalAs(UnmanagedType.U4)] uint dwReserved);

        [PreserveSig]
        int CombineUrl(
            [In] [MarshalAs(UnmanagedType.LPWStr)] string pwzBaseUrl,
            [In] [MarshalAs(UnmanagedType.LPWStr)] string pwzRelativeUrl,
            [In] [MarshalAs(UnmanagedType.U4)] uint dwCombineFlags,
            [Out] [MarshalAs(UnmanagedType.LPWStr)] out string pwzResult,
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
            [In, Out] IntPtr pBuffer,
            [In] [MarshalAs(UnmanagedType.U4)] uint cbBuffer,
            [In, Out] [MarshalAs(UnmanagedType.U4)] ref uint pcbBuf,
            [In] [MarshalAs(UnmanagedType.U4)] uint dwReserved);
    }
}
