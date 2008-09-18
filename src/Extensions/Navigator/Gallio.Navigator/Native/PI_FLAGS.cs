using System;
using System.Collections.Generic;
using System.Text;

namespace Gallio.Navigator.Native
{
    internal enum PI_FLAGS
    {
        PI_PARSE_URL = 0x00000001,
        PI_FILTER_MODE = 0x00000002,
        PI_FORCE_ASYNC = 0x00000004,
        PI_USE_WORKERTHREAD = 0x00000008,
        PI_MIMEVERIFICATION = 0x00000010,
        PI_CLSIDLOOKUP = 0x00000020,
        PI_DATAPROGRESS = 0x00000040,
        PI_SYNCHRONOUS = 0x00000080,
        PI_APARTMENTTHREADED = 0x00000100,
        PI_CLASSINSTALL = 0x00000200,
        PI_PASSONBINDCTX = 0x00002000,
        PI_NOMIMEHANDLER = 0x00008000,
        PI_LOADAPPDIRECT = 0x00004000,
        PD_FORCE_SWITCH = 0x00010000
    }
}
