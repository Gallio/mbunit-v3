using System;
using System.Collections.Generic;
using System.Text;

namespace Gallio.Common.Splash.Native
{
    internal static class NativeConstants
    {
        public const int S_OK = 0;
        public const int S_FALSE = 1;

        public const int E_OUTOFMEMORY = unchecked((int)0x8007000E);
        public const int USP_E_SCRIPT_NOT_IN_FONT = unchecked((int)0x80040200);

        public const int SCRIPT_UNDEFINED = 0;

        public const int TRANSPARENT = 1;
        public const int OPAQUE = 2;

        public const int DC_BRUSH = 18;
        public const int DC_PEN = 19;

        public const int RGN_XOR = 3;
    }
}
