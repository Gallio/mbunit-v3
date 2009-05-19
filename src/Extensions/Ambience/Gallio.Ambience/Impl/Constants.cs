// Copyright 2005-2009 Gallio Project - http://www.gallio.org/
// Portions Copyright 2000-2004 Jonathan de Halleux
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Gallio.Common.Policies;

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
        public const string DefaultDatabaseFileName = "Default.db";

        public const string AnonymousUserName = "anonymous";
        public const string AnonymousPassword = "anonymous";

        public static string CommonAppDataFolderPath
        {
            get
            {
                return SpecialPathPolicy.For("Ambience").GetCommonApplicationDataDirectory().FullName;
            }
        }

        public static NetworkCredential CreateAnonymousCredential()
        {
            return new NetworkCredential(AnonymousUserName, AnonymousPassword);
        }
    }
}
