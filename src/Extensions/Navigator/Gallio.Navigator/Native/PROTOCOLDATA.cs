using System;

namespace Gallio.Navigator.Native
{
    public struct PROTOCOLDATA
    {
        public uint grfFlags;
        public uint dwState;
        public IntPtr pData;
        public uint cbData;
    }
}
