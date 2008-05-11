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
    /// An Xml-serializable test execution log.
    /// </summary>
    [Serializable]
    [XmlType(Namespace = XmlSerializationUtils.GallioNamespace)]
    [XmlRoot("executionLog", Namespace=XmlSerializationUtils.GallioNamespace)]
    public sealed class ExecutionLog
    {
        private readonly List<ExecutionLogStream> streams;
        private readonly List<ExecutionLogAttachment> attachments;

        /// <summary>
        /// Creates an empty execution log.
        /// </summary>
        public ExecutionLog()
        {
            streams = new List<ExecutionLogStream>();
            attachments = new List<ExecutionLogAttachment>();
        }

        /// <summary>
        /// Gets the list of streams, not null.
        /// Used for Xml-serialization.
        /// </summary>
        [XmlArray("streams", IsNullable = false, Namespace = XmlSerializationUtils.GallioNamespace)]
        [XmlArrayItem("stream", typeof(ExecutionLogStream), IsNullable = false, Namespace = XmlSerializationUtils.GallioNamespace)]
        public List<ExecutionLogStream> Streams
        {
            get { return streams; }
        }

        /// <summary>
        /// Gets the list of attachments, not null.
        /// </summary>
        [XmlArray("attachments", IsNullable = false, Namespace = XmlSerializationUtils.GallioNamespace)]
        [XmlArrayItem("attachment", typeof(ExecutionLogAttachment), IsNullable = false, Namespace = XmlSerializationUtils.GallioNamespace)]
        public List<ExecutionLogAttachment> Attachments
        {
            get { return attachments; }
        }

        /// <summary>
        /// Gets a stream by name.
        /// </summary>
        /// <param name="name">The stream name</param>
        /// <returns>The stream or null if not found</returns>
        public ExecutionLogStream GetStream(string name)
        {
            return streams.Find(delegate(ExecutionLogStream stream)
            {
                return stream.Name == name;
            });
        }
    }
}