// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
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
using System.Text;
using System.Xml.Serialization;

namespace Gallio.Runtime.Hosting
{
    /// <summary>
    /// Specifies where a <see cref="IHost" /> should store the temporary configuration file
    /// it generates when it is being initialized.
    /// </summary>
    public enum ConfigurationFileLocation
    {
        /// <summary>
        /// Do not create a configuration file.
        /// </summary>
        [XmlEnum("none")]
        None = 0,

        /// <summary>
        /// Stores the configuration file in the user's temporary directory.
        /// </summary>
        [XmlEnum("temp")]
        Temp = 1,

        /// <summary>
        /// Stores the configuration file in the application base directory of the hosted components.
        /// </summary>
        [XmlEnum("appBase")]
        AppBase = 2,
    }
}
