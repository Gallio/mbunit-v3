// Copyright 2007 MbUnit Project - http://www.mbunit.com/
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
using System.Xml.Serialization;
using MbUnit.Framework.Kernel.Utilities;

namespace MbUnit.Core.Reporting
{
    /// <summary>
    /// An xml-serializable execution log stram.
    /// </summary>
    [XmlType(Namespace = SerializationUtils.XmlNamespace)]
    [Serializable]
    public sealed class ExecutionLogStream
    {
        private string name;
        private ExecutionLogStreamBodyTag body;

        /// <summary>
        /// Gets or sets the name of the log stream, not null.
        /// </summary>
        [XmlAttribute("name")]
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        /// <summary>
        /// Gets or sets the body of the log stream, not null.
        /// </summary>
        [XmlElement("body", IsNullable=false)]
        public ExecutionLogStreamBodyTag Body
        {
            get { return body; }
            set { body = value; }
        }

        /// <summary>
        /// Creates an empty but fully initialized instance.
        /// </summary>
        public static ExecutionLogStream Create(string streamName)
        {
            ExecutionLogStream stream = new ExecutionLogStream();
            stream.name = streamName;
            stream.body = ExecutionLogStreamBodyTag.Create();
            return stream;
        }
    }
}
