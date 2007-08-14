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
using MbUnit.Framework.Kernel.Utilities;

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
        /// Gets or sets the array of streams, not null.
        /// Used for Xml-serialization.
        /// </summary>
        [XmlArray("streams", IsNullable=false)]
        [XmlArrayItem("stream", IsNullable=false)]
        public ExecutionLogStream[] Streams
        {
            get { return streams.ToArray(); }
            set { streams = new List<ExecutionLogStream>(value); }
        }

        /// <summary>
        /// Gets or sets the array of attachments, not null.
        /// </summary>
        [XmlArray("attachments", IsNullable=false)]
        [XmlArrayItem("attachment", IsNullable=false)]
        public ExecutionLogAttachment[] Attachments
        {
            get { return attachments.ToArray(); }
            set { attachments = new List<ExecutionLogAttachment>(value); }
        }

        /// <summary>
        /// Adds an execution log stream to the execution log.
        /// </summary>
        /// <param name="stream">The log stream to add</param>
        public void AddStream(ExecutionLogStream stream)
        {
            streams.Add(stream);
        }

        /// <summary>
        /// Adds an attachment to the execution log.
        /// </summary>
        /// <param name="attachment">The attachment to add</param>
        public void AddAttachment(ExecutionLogAttachment attachment)
        {
            attachments.Add(attachment);
        }

        /// <summary>
        /// Creates an empty but fully initialized instance.
        /// </summary>
        public static ExecutionLog Create()
        {
            ExecutionLog executionLog = new ExecutionLog();
            executionLog.streams = new List<ExecutionLogStream>();
            executionLog.attachments = new List<ExecutionLogAttachment>();
            return executionLog;
        }
    }
}