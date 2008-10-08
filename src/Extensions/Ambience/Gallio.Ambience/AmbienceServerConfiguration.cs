// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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
using System.IO;
using System.Net;
using Gallio.Ambience.Impl;

namespace Gallio.Ambience
{
    /// <summary>
    /// <para>
    /// Provides configuration data for <see cref="AmbienceServer" />.
    /// </para>
    /// </summary>
    public class AmbienceServerConfiguration
    {
        private int port = Constants.DefaultPortNumber;
        private NetworkCredential credential = Constants.AnonymousCredential;
        private string databaseFolder;

        /// <summary>
        /// Gets or sets the database folder.
        /// </summary>
        /// <value>The database folder, the default is a file in the Gallio.Ambient subdirectory
        /// of the Local Application Data folder.
        /// </value>
        public string DatabaseFolder
        {
            get
            {
                if (databaseFolder == null)
                    databaseFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Gallio.Ambient");
                return databaseFolder;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                databaseFolder = value;
            }
        }

        /// <summary>
        /// Gets or sets the Ambient server port number.
        /// </summary>
        /// <value>The port number, defaults to 7822.</value>
        public int Port
        {
            get { return port; }
            set { port = value; }
        }

        /// <summary>
        /// Gets or sets the Ambient server username and password.
        /// </summary>
        /// <value>The username and password, defaults to an anonymous credential.</value>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null</exception>
        public NetworkCredential Credential
        {
            get { return credential; }
            set {
                if (value == null)
                    throw new ArgumentNullException("value");
                credential = value;
            }
        }
    }
}
