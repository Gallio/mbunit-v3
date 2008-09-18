using System;
using System.Collections.Generic;
using System.Text;

namespace Gallio.Navigator.Native
{
    public enum BSCF
    {
        BSCF_FIRSTDATANOTIFICATION = 0x01,
        BSCF_INTERMEDIATEDATANOTIFICATION = 0x02,
        BSCF_LASTDATANOTIFICATION = 0x04,
        BSCF_DATAFULLYAVAILABLE = 0x08,
        BSCF_AVAILABLEDATASIZEUNKNOWN = 0x10
    }
}
