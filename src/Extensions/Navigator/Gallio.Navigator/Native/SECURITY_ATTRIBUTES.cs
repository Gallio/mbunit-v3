using System;
using System.Collections.Generic;
using System.Text;

namespace Gallio.Navigator.Native
{
    public struct SECURITY_ATTRIBUTES
    {
        public uint nLength;
        public IntPtr lpSecurityDescriptor;
        public int bInheritHandle;
    }
}
