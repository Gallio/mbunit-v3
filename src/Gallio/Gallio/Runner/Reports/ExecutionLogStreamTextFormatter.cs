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
using System.Text;

namespace Gallio.Runner.Reports
{
    /// <summary>
    /// Formats <see cref="ExecutionLogStreamTag" /> instances to plain text by recursively
    /// concatenating the text of all contained <see cref="ExecutionLogStreamTextTag" />
    /// elements.  Sections and embedded attachments introduce line-breaks within the
    /// text but are otherwise ignored.
    /// </summary>
    public sealed class ExecutionLogStreamTextFormatter : IExecutionLogStreamTagVisitor
    {
        private readonly StringBuilder textBuilder = new StringBuilder();

        /// <summary>
        /// Gets the text that has been built.
        /// </summary>
        public string Text
        {
            get { return textBuilder.ToString(); }
        }

        /// <inheritdoc />
        public void VisitBodyTag(ExecutionLogStreamBodyTag tag)
        {
            InsertLineBreakIfNeeded();
            tag.AcceptContents(this);
            InsertLineBreakIfNeeded();
        }

        /// <inheritdoc />
        public void VisitSectionTag(ExecutionLogStreamSectionTag tag)
        {
            InsertLineBreakIfNeeded();
            tag.AcceptContents(this);
            InsertLineBreakIfNeeded();
        }

        /// <inheritdoc />
        public void VisitEmbedTag(ExecutionLogStreamEmbedTag tag)
        {
            InsertLineBreakIfNeeded();
        }

        /// <inheritdoc />
        public void VisitTextTag(ExecutionLogStreamTextTag tag)
        {
            textBuilder.Append(tag.Text);
        }

        private void InsertLineBreakIfNeeded()
        {
            if (textBuilder.Length != 0 && textBuilder[textBuilder.Length - 1] != '\n')
                textBuilder.Append('\n');
        }
    }
}
