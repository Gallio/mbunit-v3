using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Gallio.Common.Splash.Native
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct SCRIPT_VISATTR
    {
        public const ushort uJustificationMask = 0x000f;
        public const ushort fClusterStartMask  = 0x0010;
        public const ushort fDiacriticMask     = 0x0020;
        public const ushort fZeroWidthMask     = 0x0040;
        public const ushort fReservedMask      = 0x0080;
        public const ushort fShapeReservedMask = 0xff00;

        public ushort BitFields;
    }
}
