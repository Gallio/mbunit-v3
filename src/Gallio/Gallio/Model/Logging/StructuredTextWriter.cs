// Copyright 2005-2009 Gallio Project - http://www.gallio.org/
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
using Gallio.Model.Logging;

namespace Gallio.Model.Logging
{
    /// <summary>
    /// <para>
    /// Writes log information in a structured manner so as to produce a
    /// <see cref="StructuredText" /> or <see cref="string" />.
    /// </para>
    /// </summary>
    public class StructuredTextWriter : TestLogStreamWriter, ITestLogStreamWritable
    {
        /// <summary>
        /// Creates a structured text writer.
        /// </summary>
        public StructuredTextWriter() 
            : base(new StructuredDocumentWriter(), TestLogStreamNames.Default)
        {
        }

        /// <summary>
        /// Gets the structured text produced so far.
        /// </summary>
        /// <returns>The structured text</returns>
        public StructuredText ToStructuredText()
        {
            Flush();

            return new StructuredText(
                Container.TestLog.GetStream(StreamName).Body.Clone(),
                Container.Attachments.ToArray());
        }

        /// <summary>
        /// Returns the structured text formatted as a string.
        /// </summary>
        /// <returns>The structured text</returns>
        public override string ToString()
        {
            return ToStructuredText().ToString();
        }

        /// <inheritdoc />
        public void WriteTo(TestLogStreamWriter writer)
        {
            ToStructuredText().WriteTo(writer);
        }

        #region Hide irrelevant base functionality.

        new private StructuredDocumentWriter Container
        {
            get { return (StructuredDocumentWriter) base.Container; }
        }

        new private string StreamName
        {
            get { return base.StreamName; }
        }

        #endregion

        private sealed class StructuredDocumentWriter : StructuredTestLogWriter
        {
            public readonly List<Attachment> Attachments = new List<Attachment>();

            protected override void AttachImpl(Attachment attachment)
            {
                Attachments.Add(attachment);
            }
        }
    }
}
