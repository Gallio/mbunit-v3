// Copyright 2008 MbUnit Project - http://www.mbunit.com/
// Portions Copyright 2000-2004 Jonathan De Halleux, Jamie Cansdale
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
using System.Threading;
using System.Xml.Serialization;
using Gallio.Model.Serialization;

namespace Gallio.Hosting
{
    /// <summary>
    /// Specifies a collection of parameters for setting up a <see cref="IHost" />.
    /// </summary>
    [Serializable]
    [XmlRoot("hostSetup", Namespace = SerializationUtils.XmlNamespace)]
    [XmlType(Namespace = SerializationUtils.XmlNamespace)]
    public sealed class HostSetup
    {
        private string applicationBase;
        private bool enableShadowCopy;
        private HostConfiguration configuration;

        /// <summary>
        /// Creates a default host setup.
        /// </summary>
        public HostSetup()
        {
        }

        /// <summary>
        /// Gets or sets the relative or absolute path of the application base directory.
        /// If relative, the path is based on the application base of the test runner,
        /// so a value of "" causes the test runner's application base to be used.
        /// </summary>
        [XmlAttribute("applicationBase")]
        public string ApplicationBase
        {
            get { return applicationBase; }
            set { applicationBase = value; }
        }

        /// <summary>
        /// Gets or sets whether assembly shadow copying is enabled.
        /// </summary>
        [XmlAttribute("enableShadowCopy")]
        public bool EnableShadowCopy
        {
            get { return enableShadowCopy; }
            set { enableShadowCopy = value; }
        }

        /// <summary>
        /// Gets or sets the host configuration information.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null</exception>
        [XmlElement("configuration", IsNullable=false)]
        public HostConfiguration Configuration
        {
            get
            {
                if (configuration == null)
                    Interlocked.CompareExchange(ref configuration, new HostConfiguration(), null);
                return configuration;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                configuration = value;
            }
        }
    }
}
