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
using System.Net;
using Gallio.Ambience.Impl;

namespace Gallio.Ambience
{
    /// <summary>
    /// Provides configuration data for <see cref="AmbienceClient" />.
    /// </summary>
    public class AmbienceClientConfiguration
    {
        private string hostName = "localhost";
        private int port = Constants.DefaultPortNumber;
        private NetworkCredential credential = Constants.CreateAnonymousCredential();

        /// <summary>
        /// Gets or sets the Ambient server hostname.
        /// </summary>
        /// <value>The hostname, defaults to "localhost".</value>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="value"/> is empty.</exception>
        public string HostName
        {
            get { return hostName; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                if (value.Length == 0)
                    throw new ArgumentException("Host name should not be empty.", "value");
                hostName = value;
            }
        }

        /// <summary>
        /// Gets or sets the Ambient server port number.
        /// </summary>
        /// <value>The port number, defaults to 7822.</value>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the port number is not in the range 1..65535.</exception>
        public int Port
        {
            get { return port; }
            set
            {
                Validator.ValidatePortNumber(value);
                port = value;
            }
        }

        /// <summary>
        /// Gets or sets the Ambient server username and password.
        /// </summary>
        /// <value>The username and password, defaults to an anonymous credential.</value>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null.</exception>
        public NetworkCredential Credential
        {
            get { return credential; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                credential = value;
            }
        }
    }
}
