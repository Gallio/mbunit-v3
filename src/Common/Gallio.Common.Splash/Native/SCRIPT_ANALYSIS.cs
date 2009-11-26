using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Gallio.Common.Splash.Native
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct SCRIPT_ANALYSIS
    {
        public const ushort eScriptMask       = 0x03ff;
        public const ushort fRTLMask          = 0x0400;
        public const ushort fLayoutRTLMask    = 0x0800;
        public const ushort fLinkBeforeMask   = 0x1000;
        public const ushort fLinkAfterMask    = 0x2000;
        public const ushort fLogicalOrderMask = 0x4000;
        public const ushort fNoGlyphIndexMask = 0x8000;

        public ushort BitFields;
        public SCRIPT_STATE s;

        public int eScript
        {
            get { return BitFields & eScriptMask; }
            set { BitFields = (ushort)(BitFields & ~eScriptMask | value); }
        }

        public bool fRTL
        {
            get { return (BitFields & fRTLMask) != 0; }
        }
    }
}
