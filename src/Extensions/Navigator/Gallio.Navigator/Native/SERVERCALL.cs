using System;
using System.Collections.Generic;
using System.Text;

namespace Gallio.Navigator.Native
{
    internal enum SERVERCALL
    {
        SERVERCALL_ISHANDLED = 0,
        SERVERCALL_REJECTED = 1,
        SERVERCALL_RETRYLATER = 2
    }
}
