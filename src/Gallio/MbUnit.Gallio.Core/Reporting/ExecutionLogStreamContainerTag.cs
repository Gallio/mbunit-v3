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
using MbUnit.Core.Reporting;

namespace MbUnit.Core.Reporting
{
    /// <summary>
    /// Abstract class of Xml-serializable execution log container tags.
    /// </summary>
    public abstract class ExecutionLogStreamContainerTag : ExecutionLogStreamTag
    {
        private List<ExecutionLogStreamTag> contents;

        /// <summary>
        /// Gets or sets the nested contents of this tag.
        /// </summary>
        [XmlArray("contents", IsNullable=false)]
        [XmlArrayItem("section", typeof(ExecutionLogStreamSectionTag), IsNullable = false)]
        [XmlArrayItem("text", typeof(ExecutionLogStreamTextTag), IsNullable = false)]
        [XmlArrayItem("embed", typeof(ExecutionLogStreamEmbedTag), IsNullable = false)]
        public ExecutionLogStreamTag[] Contents
        {
            get { return contents.ToArray(); }
            set { contents = new List<ExecutionLogStreamTag>(value); }
        }

        /// <summary>
        /// Adds a content tag to the container.
        /// </summary>
        /// <param name="tag">The tag to add</param>
        public void AddContent(ExecutionLogStreamTag tag)
        {
            contents.Add(tag);
        }

        /// <summary>
        /// Initializes the contents list to an empty list.
        /// </summary>
        protected void Initialize()
        {
            contents = new List<ExecutionLogStreamTag>();
        }
    }
}
