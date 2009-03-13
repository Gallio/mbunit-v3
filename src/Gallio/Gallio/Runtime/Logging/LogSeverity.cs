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
using System.Xml.Serialization;

namespace Gallio.Runtime.Logging
{
    /// <summary>
    /// Describes the severity of a log message.
    /// </summary>
    public enum LogSeverity
    {
        /// <summary>
        /// The severity used for debug messages.
        /// </summary>
        [XmlEnum("debug")]
        Debug = 0,

        /// <summary>
        /// The severity used for informational messages.
        /// </summary>
        [XmlEnum("info")]
        Info = 1,

        /// <summary>
        /// The severity used for important messages.
        /// </summary>
        [XmlEnum("important")]
        Important = 2,

        /// <summary>
        /// The severity used for warning messages.
        /// </summary>
        [XmlEnum("warning")]
        Warning = 3,

        /// <summary>
        /// The severity used for error messages.
        /// </summary>
        [XmlEnum("error")]
        Error = 4
    }
}
