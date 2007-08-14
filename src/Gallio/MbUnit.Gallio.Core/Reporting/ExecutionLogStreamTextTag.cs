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
using System.Text;
using System.Xml.Serialization;
using MbUnit.Framework.Kernel.Utilities;

namespace MbUnit.Core.Reporting
{
    /// <summary>
    /// An Xml-serializable block of preformatted text to include in an execution log stream.
    /// </summary>
    [XmlType(Namespace = SerializationUtils.XmlNamespace)]
    [Serializable]
    public sealed class ExecutionLogStreamTextTag : ExecutionLogStreamTag
    {
        private string text;

        /// <summary>
        /// Gets or sets the text within the tag, not null.
        /// </summary>
        [XmlText]
        public string Text
        {
            get { return text; }
            set { text = value; }
        }

        /// <summary>
        /// Creates an empty but fully initialized instance.
        /// </summary>
        /// <param name="text">The text</param>
        public static ExecutionLogStreamTextTag Create(string text)
        {
            ExecutionLogStreamTextTag tag = new ExecutionLogStreamTextTag();
            tag.text = text;
            return tag;
        }
    }
}
