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

using Gallio.Model.Execution;

namespace Gallio.Framework
{
    internal sealed class InternalLogStreamWriter : LogStreamWriter
    {
        private readonly ITestLogWriter logWriter;

        public InternalLogStreamWriter(ITestLogWriter logWriter, string streamName) : base(streamName)
        {
            this.logWriter = logWriter;
        }

        protected override void WriteImpl(string text)
        {
            logWriter.Write(StreamName, text);
        }

        protected override void EmbedImpl(Attachment attachment)
        {
            attachment.Attach(logWriter);
            EmbedExistingImpl(attachment.Name);
        }

        protected override void EmbedExistingImpl(string attachmentName)
        {
            logWriter.Embed(StreamName, attachmentName);
        }

        protected override void BeginSectionImpl(string sectionName)
        {
            logWriter.BeginSection(StreamName, sectionName);
        }

        protected override void EndSectionImpl()
        {
            logWriter.EndSection(StreamName);
        }
    }
}
