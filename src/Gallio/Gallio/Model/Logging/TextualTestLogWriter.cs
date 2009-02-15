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
using System.IO;
using Gallio.Model.Execution;

namespace Gallio.Model.Logging
{
    /// <summary>
    /// An implementation of <see cref="TestLogWriter" /> that represents the test log as
    /// text written to a <see cref="TextWriter" />.
    /// </summary>
    /// <seealso cref="StubTestContextTracker" />
    public class TextualTestLogWriter : TestLogWriter
    {
        private readonly TextWriter writer;
        private readonly bool verbose;
        private readonly Stack<bool> blockStack;
        private bool needNewline;

        /// <summary>
        /// Creates a stub test log writer.
        /// </summary>
        /// <param name="writer">The text writer to write to</param>
        /// <param name="verbose">If true, prints detailed information about the location of
        /// attachments, sections, and markers, otherwise discards these formatting details
        /// and prints section headers as text on their own line</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="writer"/> is null</exception>
        public TextualTestLogWriter(TextWriter writer, bool verbose)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");

            this.writer = writer;
            this.verbose = verbose;
            blockStack = new Stack<bool>();
        }

        /// <summary>
        /// Gets the underlying text writer.
        /// </summary>
        protected TextWriter Writer
        {
            get { return writer; }
        }

        /// <summary>
        /// Returns true if the verbose print mode is enabled.
        /// </summary>
        protected bool Verbose
        {
            get { return verbose; }
        }

        /// <inheritdoc />
        protected override void AttachImpl(Attachment attachment)
        {
            if (verbose)
            {
                WriteNewlineIfNeeded();
                writer.WriteLine("[Attachment '{0}': {1}]", attachment.Name, attachment.ContentType);
            }
        }

        /// <inheritdoc />
        protected override void StreamWriteImpl(string streamName, string text)
        {
            writer.Write(text);

            needNewline = ! text.EndsWith("\n");
        }

        /// <inheritdoc />
        protected override void StreamEmbedImpl(string streamName, string attachmentName)
        {
            if (verbose)
            {
                WriteNewlineIfNeeded();
                writer.WriteLine("[Embedded Attachment '{0}']", attachmentName);
            }
        }

        /// <inheritdoc />
        protected override void StreamBeginSectionImpl(string streamName, string sectionName)
        {
            if (verbose)
            {
                WriteNewlineIfNeeded();
                writer.WriteLine("[Section '{0}']", sectionName);
            }
            else
            {
                WriteNewlineIfNeeded();
                writer.WriteLine(sectionName);
            }

            blockStack.Push(true);
        }

        /// <inheritdoc />
        protected override void StreamBeginMarkerImpl(string streamName, Marker marker)
        {
            if (verbose)
                writer.Write("[Marker '{0}']", marker.Class);

            blockStack.Push(false);
        }

        /// <inheritdoc />
        protected override void StreamEndImpl(string streamName)
        {
            bool block = blockStack.Pop();
            if (block)
                WriteNewlineIfNeeded();

            if (verbose)
            {
                writer.Write("[End]");
                if (block)
                    writer.WriteLine();
            }
        }

        private void WriteNewlineIfNeeded()
        {
            if (needNewline)
            {
                needNewline = false;
                writer.WriteLine();
            }
        }
    }
}