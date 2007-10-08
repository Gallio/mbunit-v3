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

using System.IO;
using System.Text;
using MbUnit.Logging;

namespace MbUnit.Core.Harness
{
    /// <summary>
    /// A contextual log stream text writer writes text messages to a stream in
    /// the execution log associated with the current test execution context at the time
    /// the message was written.
    /// </summary>
    public sealed class ContextualLogStreamTextWriter : TextWriter
    {
        private readonly string streamName;

        /// <summary>
        /// Creates a text writer that writes to the specified execution log stream.
        /// </summary>
        /// <param name="streamName">The execution log stream name</param>
        public ContextualLogStreamTextWriter(string streamName)
        {
            this.streamName = streamName;
            NewLine = "\n";
        }

        /// <inheritdoc />
        public override Encoding Encoding
        {
            get { return Encoding.Unicode; }
        }

        /// <inheritdoc />
        public override void Write(char value)
        {
            CurrentLogStreamWriter.Write(value);
        }

        /// <inheritdoc />
        public override void Write(string value)
        {
            CurrentLogStreamWriter.Write(value);
        }

        /// <inheritdoc />
        public override void Write(char[] buffer, int index, int count)
        {
            CurrentLogStreamWriter.Write(buffer, index, count);
        }

        private LogStreamWriter CurrentLogStreamWriter
        {
            get { return Log.Writer[streamName]; }
        }
    }
}