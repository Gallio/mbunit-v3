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

        public const int TRANSPARENT = 1;
        public const int OPAQUE = 2;
    }
}
