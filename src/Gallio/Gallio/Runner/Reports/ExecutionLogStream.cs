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
using System.Xml.Serialization;
using Gallio.Utilities;

namespace Gallio.Runner.Reports
{
    /// <summary>
    /// An xml-serializable execution log stram.
    /// </summary>
    [Serializable]
    [XmlType(Namespace = XmlSerializationUtils.GallioNamespace)]
    public sealed class ExecutionLogStream
    {
        private string name;
        private ExecutionLogStreamBodyTag body;

        /// <summary>
        /// Creates an uninitialized instance for Xml deserialization.
        /// </summary>
        private ExecutionLogStream()
        {
        }

        /// <summary>
        /// Creates an initialized stream.
        /// </summary>
        /// <param name="name">The stream name</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/> is null</exception>
        public ExecutionLogStream(string name)
        {
            if (name == null)
                throw new ArgumentNullException(@"name");

            this.name = name;
        }

        /// <summary>
        /// Gets or sets the name of the log stream, not null.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null</exception>
        [XmlAttribute("name")]
        public string Name
        {
            get { return name; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(@"value");
                name = value;
            }
        }

        /// <summary>
        /// Gets or sets the body of the log stream, not null.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null</exception>
        [XmlElement("body", IsNullable = false)]
        public ExecutionLogStreamBodyTag Body
        {
            get
            {
                if (body == null)
                    body = new ExecutionLogStreamBodyTag();
                return body;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(@"value");
                body = value;
            }
        }

        /// <summary>
        /// Formats the stream using a <see cref="ExecutionLogStreamTextFormatter" />.
        /// </summary>
        /// <returns>The formatted text</returns>
        public override string ToString()
        {
            return body.ToString();
        }
    }
}
