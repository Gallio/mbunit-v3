using System;
using System.Collections.Generic;
using System.Text;

namespace Gallio.Navigator.Native
{
    internal enum BINDVERB : uint
    {
        BINDVERB_GET = 0,
        BINDVERB_POST = 1,
        BINDVERB_PUT = 2,
        BINDVERB_CUSTOM = 3,
    }
}
