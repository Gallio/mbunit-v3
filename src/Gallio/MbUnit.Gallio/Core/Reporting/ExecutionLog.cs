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
using System.Collections.Generic;
using System.Xml.Serialization;
using MbUnit.Model.Serialization;

namespace MbUnit.Core.Reporting
{
    /// <summary>
    /// An Xml-serializable test execution log.
    /// </summary>
    [XmlType(Namespace = SerializationUtils.XmlNamespace)]
    [XmlRoot("executionLog", Namespace=SerializationUtils.XmlNamespace)]
    [Serializable]
    public sealed class ExecutionLog
    {
        private List<ExecutionLogStream> streams;
        private List<ExecutionLogAttachment> attachments;

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
        [XmlArray("streams", IsNullable=false)]
        [XmlArrayItem("stream", typeof(ExecutionLogStream), IsNullable=false)]
        public List<ExecutionLogStream> Streams
        {
            get { return streams; }
        }

        /// <summary>
        /// Gets the list of attachments, not null.
        /// </summary>
        [XmlArray("attachments", IsNullable=false)]
        [XmlArrayItem("attachment", typeof(ExecutionLogAttachment), IsNullable=false)]
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