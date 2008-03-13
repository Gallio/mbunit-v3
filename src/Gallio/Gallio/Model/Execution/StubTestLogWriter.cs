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

namespace Gallio.Model.Execution
{
    /// <summary>
    /// A stub implementation of <see cref="ITestLogWriter" /> that logs output to <see cref="Console.Out" />.
    /// </summary>
    /// <seealso cref="StubTestContextTracker" />
    public class StubTestLogWriter : BaseTestLogWriter
    {
        private bool needNewline;

        /// <inheritdoc />
        protected override void CloseImpl()
        {
        }

        /// <inheritdoc />
        protected override void AttachTextImpl(string attachmentName, string contentType, string text)
        {
            Attach(attachmentName, contentType);
        }

        /// <inheritdoc />
        protected override void AttachBytesImpl(string attachmentName, string contentType, byte[] bytes)
        {
            Attach(attachmentName, contentType);
        }

        private void Attach(string attachmentName, string contentType)
        {
            WriteNewlineIfNeeded();
            Console.Out.WriteLine("[Attachment '{0}': {1}]", attachmentName, contentType);
        }

        /// <inheritdoc />
        protected override void WriteImpl(string streamName, string text)
        {
            Console.Out.Write(text);

            needNewline = ! text.EndsWith("\n");
        }

        /// <inheritdoc />
        protected override void EmbedImpl(string streamName, string attachmentName)
        {
            WriteNewlineIfNeeded();
            Console.Out.WriteLine("[Embedded Attachment '{0}']", attachmentName);
        }

        /// <inheritdoc />
        protected override void BeginSectionImpl(string streamName, string sectionName)
        {
            WriteNewlineIfNeeded();
            Console.Out.WriteLine("[Begin Section '{0}']", sectionName);
        }

        /// <inheritdoc />
        protected override void EndSectionImpl(string streamName)
        {
            WriteNewlineIfNeeded();
            Console.Out.WriteLine("[End Section]");
        }

        private void WriteNewlineIfNeeded()
        {
            if (needNewline)
            {
                needNewline = false;
                Console.Out.WriteLine();
            }
        }
    }
}
