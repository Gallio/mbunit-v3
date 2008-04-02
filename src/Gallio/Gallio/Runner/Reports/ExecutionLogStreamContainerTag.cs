// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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
using Gallio.Utilities;

namespace Gallio.Runner.Reports
{
    /// <summary>
    /// Abstract class of Xml-serializable execution log container tags.
    /// </summary>
    [Serializable]
    [XmlType(Namespace = XmlSerializationUtils.GallioNamespace)]
    public abstract class ExecutionLogStreamContainerTag : ExecutionLogStreamTag
    {
        private readonly List<ExecutionLogStreamTag> contents;

        /// <summary>
        /// Creates an empty container tag.
        /// </summary>
        protected ExecutionLogStreamContainerTag()
        {
            contents = new List<ExecutionLogStreamTag>();
        }

        /// <summary>
        /// Gets the list of nested contents of this tag.
        /// </summary>
        [XmlArray("contents", IsNullable=false)]
        [XmlArrayItem("section", typeof(ExecutionLogStreamSectionTag), IsNullable = false)]
        [XmlArrayItem("text", typeof(ExecutionLogStreamTextTag), IsNullable = false)]
        [XmlArrayItem("embed", typeof(ExecutionLogStreamEmbedTag), IsNullable = false)]
        public List<ExecutionLogStreamTag> Contents
        {
            get { return contents; }
        }

        /// <summary>
        /// Invokes the appropriate visitor method each element contained within this tag.
        /// </summary>
        /// <param name="visitor">The visitor</param>
        public void AcceptContents(IExecutionLogStreamTagVisitor visitor)
        {
            foreach (ExecutionLogStreamTag content in contents)
                content.Accept(visitor);
        }
    }
}
