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
using Gallio.Model.Logging.Tags;
using Gallio.Utilities;

namespace Gallio.Model.Logging
{
    /// <summary>
    /// <para>
    /// A structured log stream object represents a recursively structured stream of rich text that
    /// supports embedded attachments, nested sections and marked regions.  Each part of the
    /// text is captured by a tag, some of which are composable and may therefore contain other tags.
    /// </para>
    /// <para>
    /// It is effectively an xml-serializable representation of a test log stream written by
    /// a <see cref="TestLogStreamWriter"/>.
    /// </para>
    /// </summary>
    [Serializable]
    [XmlType(Namespace = XmlSerializationUtils.GallioNamespace)]
    public sealed class StructuredTestLogStream
    {
        private string name;
        private BodyTag body;

        /// <summary>
        /// Creates an uninitialized instance for Xml deserialization.
        /// </summary>
        private StructuredTestLogStream()
        {
        }

        /// <summary>
        /// Creates an initialized stream.
        /// </summary>
        /// <param name="name">The stream name</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/> is null</exception>
        public StructuredTestLogStream(string name)
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
        public BodyTag Body
        {
            get
            {
                if (body == null)
                    body = new BodyTag();
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
        /// Writes the structured text to a test log stream writer.
        /// </summary>
        /// <param name="writer">The writer</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="writer"/> is null</exception>
        public void WriteTo(TestLogStreamWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");

            if (body != null)
                body.WriteTo(writer);
        }

        /// <summary>
        /// Formats the stream using a <see cref="TagFormatter" />.
        /// </summary>
        /// <returns>The formatted text</returns>
        public override string ToString()
        {
            return body != null ? body.ToString() : string.Empty;
        }
    }
}