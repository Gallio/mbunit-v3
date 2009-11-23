using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Gallio.Common.Splash.Native
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct SCRIPT_STATE
    {
        public const ushort uBidiLevelMask         = 0x001f;
        public const ushort fOverrideDirectionMask = 0x0020;
        public const ushort fInhibitSymSwapMask    = 0x0040;
        public const ushort fCharShapeMask         = 0x0080;
        public const ushort fDigitSubstituteMask   = 0x0100;
        public const ushort fInhibitLigateMask     = 0x0200;
        public const ushort fDisplayZWGMask        = 0x0400;
        public const ushort fArabicNumContextMask  = 0x0800;
        public const ushort fGcpClustersMask       = 0x1000;
        public const ushort fReservedMask          = 0x2000;
        public const ushort fEngineReservedMask    = 0xc000;

        public ushort BitFields;

        public int uBidiLevel
        {
            get { return BitFields & uBidiLevelMask; }
        }
    }
}
