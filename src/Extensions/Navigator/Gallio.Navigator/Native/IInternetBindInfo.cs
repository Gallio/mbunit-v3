using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace Gallio.Navigator.Native
{
    [ComImport]
    [Guid("79EAC9E1-BAF9-11CE-8C82-00AA004BA90B")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IInternetBindInfo
    {
        [PreserveSig]
        int GetBindInfo(
            [Out] [MarshalAs(UnmanagedType.U4)] out uint grfBINDF,
            [In, Out] ref BINDINFO pbindinfo);

        [PreserveSig]
        int GetBindString(
            [In] [MarshalAs(UnmanagedType.U4)] uint ulStringType,
            [In, Out] [MarshalAs(UnmanagedType.LPWStr)] ref string ppwzStr,
            [In] [MarshalAs(UnmanagedType.U4)] uint cEl,
            [In] [MarshalAs(UnmanagedType.U4)] ref uint pcElFetched);
    }
}
