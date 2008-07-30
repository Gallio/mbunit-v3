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
using System.Collections.Generic;
using System.Xml.Serialization;
using Gallio.Utilities;

namespace Gallio.Runner.Reports
{
    /// <summary>
    /// An Xml-serializable test log.
    /// </summary>
    [Serializable]
    [XmlType(Namespace = XmlSerializationUtils.GallioNamespace)]
    [XmlRoot("testLog", Namespace=XmlSerializationUtils.GallioNamespace)]
    public sealed class TestLog
    {
        private readonly List<TestLogStream> streams;
        private readonly List<TestLogAttachment> attachments;

        /// <summary>
        /// Creates an empty execution log.
        /// </summary>
        public TestLog()
        {
            streams = new List<TestLogStream>();
            attachments = new List<TestLogAttachment>();
        }

        /// <summary>
        /// Gets the list of streams, not null.
        /// Used for Xml-serialization.
        /// </summary>
        [XmlArray("streams", IsNullable = false, Namespace = XmlSerializationUtils.GallioNamespace)]
        [XmlArrayItem("stream", typeof(TestLogStream), IsNullable = false, Namespace = XmlSerializationUtils.GallioNamespace)]
        public List<TestLogStream> Streams
        {
            get { return streams; }
        }

        /// <summary>
        /// Gets the list of attachments, not null.
        /// </summary>
        [XmlArray("attachments", IsNullable = false, Namespace = XmlSerializationUtils.GallioNamespace)]
        [XmlArrayItem("attachment", typeof(TestLogAttachment), IsNullable = false, Namespace = XmlSerializationUtils.GallioNamespace)]
        public List<TestLogAttachment> Attachments
        {
            get { return attachments; }
        }

        /// <summary>
        /// Gets a stream by name.
        /// </summary>
        /// <param name="name">The stream name</param>
        /// <returns>The stream or null if not found</returns>
        public TestLogStream GetStream(string name)
        {
            return streams.Find(delegate(TestLogStream stream)
            {
                return stream.Name == name;
            });
        }
    }
}