using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gallio.Ambience.Impl
{
    internal static class Validator
    {
        public static void ValidatePortNumber(int port)
        {
            if (port < 1 || port > 65535)
                throw new ArgumentOutOfRangeException("port", port,
                    "The port number should be in the range 1..65535");
        }
    }
}
