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
using Gallio.Model.Serialization;

namespace Gallio.Runner.Reports
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
        /// Creates an uninitialized instance for Xml deserialization.
        /// </summary>
        private ExecutionLogStreamTextTag()
        {
        }

        /// <summary>
        /// Creates an initialized text tag.
        /// </summary>
        /// <param name="text">The text within the tag</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="text"/> is null</exception>
        public ExecutionLogStreamTextTag(string text)
        {
            if (text == null)
                throw new ArgumentNullException(@"text");
            this.text = text;
        }

        /// <summary>
        /// Gets or sets the text within the tag, not null.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null</exception>
        [XmlText]
        public string Text
        {
            get { return text; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(@"value");
                text = value;
            }
        }

        /// <inheritdoc />
        public override void Accept(IExecutionLogStreamTagVisitor visitor)
        {
            visitor.VisitTextTag(this);
        }
    }
}
