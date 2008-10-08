using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Gallio.Ambience.Impl
{
    internal static class Constants
    {
        /// <summary>
        /// Gets the default port number for the Ambient server.
        /// </summary>
        public const int DefaultPortNumber = 7822;

        /// <summary>
        /// Gets the default port number for the Ambient server as a string.
        /// </summary>
        public const string DefaultPortNumberString = "7822";

        /// <summary>
        /// Gets the default database file name.
        /// </summary>
        public const string DatabaseFileName = "Gallio.Ambient.db4o";

        public const string AnonymousUserName = "anonymous";
        public const string AnonymousPassword = "anonymous";
        public static NetworkCredential AnonymousCredential = new NetworkCredential(AnonymousUserName, AnonymousPassword);
    }
}
