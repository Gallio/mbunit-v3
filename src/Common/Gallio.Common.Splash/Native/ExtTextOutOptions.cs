using System;
using System.Collections.Generic;
using System.Text;

namespace Gallio.Common.Splash.Native
{
    [Flags]
    internal enum ExtTextOutOptions
    {
        NONE = 0,
        ETO_OPAQUE = 0x0002,
        ETO_CLIPPED = 0x0004	
    }
}
