using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Gallio.Navigator.Native
{
    public struct BINDINFO
    {
        public uint cbSize;
        [MarshalAs(UnmanagedType.LPWStr)] public string szExtraInfo;
        public uint stgmedData;
        public uint grfBindInfoF;
        public uint dwBindVerb;
        [MarshalAs(UnmanagedType.LPWStr)] public string szCustomVerb;
        public uint cbstgmedData;
        public uint dwOptions;
        public uint dwOptionsFlags;
        public uint dwCodePage;
        public SECURITY_ATTRIBUTES securityAttributes;
        public Guid iid;
        [MarshalAs(UnmanagedType.IUnknown)] public object punk;
        public uint dwReserved;
    }
}
