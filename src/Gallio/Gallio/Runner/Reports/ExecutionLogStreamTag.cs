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
    /// A tag is an Xml-serializable object that is included
    /// in the body of an execution log stream and describes its contents.
    /// </summary>
    [Serializable]
    [XmlType(Namespace = SerializationUtils.XmlNamespace)]
    public abstract class ExecutionLogStreamTag
    {
        /// <summary>
        /// Invokes the appropriate visitor method for this tag type.
        /// </summary>
        /// <param name="visitor">The visitor</param>
        public abstract void Accept(IExecutionLogStreamTagVisitor visitor);

        /// <summary>
        /// Formats the tag using a <see cref="ExecutionLogStreamTextFormatter" />.
        /// </summary>
        /// <returns>The formatted text</returns>
        public override string ToString()
        {
            ExecutionLogStreamTextFormatter formatter = new ExecutionLogStreamTextFormatter();
            Accept(formatter);
            return formatter.Text;
        }
    }
}
