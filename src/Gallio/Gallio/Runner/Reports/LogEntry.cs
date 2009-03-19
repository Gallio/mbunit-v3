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
using Gallio.Runtime.Logging;
using Gallio.Utilities;

namespace Gallio.Runner.Reports
{
    /// <summary>
    /// Describes a log entry for reporting purposes.
    /// </summary>
    [Serializable]
    [XmlType(Namespace = XmlSerializationUtils.GallioNamespace)]
    public class LogEntry
    {
        /// <summary>
        /// Gets or sets the log message severity.
        /// </summary>
        [XmlAttribute("severity")]
        public LogSeverity Severity { get; set; }

        /// <summary>
        /// Gets or sets the log message.
        /// </summary>
        [XmlAttribute("message")]
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the log message details, such as an exception.
        /// </summary>
        [XmlAttribute("details")]
        public string Details { get; set; }
    }
}
