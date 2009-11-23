using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Gallio.Common.Splash.Native
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct SCRIPT_ITEM
    {
        public int iCharPos;
        public SCRIPT_ANALYSIS a;
    }
}
