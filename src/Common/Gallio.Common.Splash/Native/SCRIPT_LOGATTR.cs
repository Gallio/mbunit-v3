using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Gallio.Common.Splash.Native
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct SCRIPT_LOGATTR
    {
        public const byte fSoftBreakMask  = 0x01;
        public const byte fWhiteSpaceMask = 0x02;
        public const byte fCharStopMask   = 0x04;
        public const byte fWordStopMask   = 0x08;
        public const byte fInvalidMask    = 0x10;
        public const byte fReservedMask   = 0xe0;

        public byte BitFields;

        public bool fWhiteSpace
        {
            get { return (BitFields & fWhiteSpaceMask) != 0; }
        }

        public bool fSoftBreak
        {
            get { return (BitFields & fSoftBreakMask) != 0; }
        }

        public bool fCharStop
        {
            get { return (BitFields & fCharStopMask) != 0; }
        }

        public void SetfWhiteSpace()
        {
            BitFields |= fWhiteSpaceMask;
        }

        public void SetfSoftBreakfCharStopAndfWordStop()
        {
            BitFields |= fSoftBreakMask | fCharStopMask | fWordStopMask;
        }
    }
}
