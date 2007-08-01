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
using MbUnit.Framework.Services.ExecutionLogs;

namespace MbUnit.Framework.Services.ExecutionLogs
{
    /// <summary>
    /// Primitive interface for writing execution logs.  Client code will typically
    /// write to an execution log using the higher-level <see cref="IExecutionLog" /> interface.
    /// </summary>
    public interface IExecutionLogWriter
    {
        /// <summary>
        /// Writes a text string to the execution log.
        /// </summary>
        /// <param name="streamName">The name of the execution log stream</param>
        /// <param name="text">The text to write</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="streamName"/> or
        /// <paramref name="text"/> is null</exception>
        void WriteText(string streamName, string text);

        /// <summary>
        /// Writes an attachment to the execution log.
        /// </summary>
        /// <remarks>
        /// The implementation should allow the same attachment instance to be attached
        /// multiple times and optimized this case by representing embedded attachments
        /// as links.
        /// </remarks>
        /// <param name="streamName">The name of the execution log stream in which to embed
        /// the attachment, or null to attach it to the top level of the execution log</param>
        /// <param name="attachment">The attachment to write</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="attachment"/> is null</exception>
        /// <exception cref="InvalidOperationException">Thrown if a different attachment instance
        /// with the same name was already written</exception>
        void WriteAttachment(string streamName, Attachment attachment);

        /// <summary>
        /// Begins a section.
        /// </summary>
        /// <param name="streamName">The name of the execution log stream</param>
        /// <param name="sectionName">The name of the section to begin</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="streamName"/> or
        /// <paramref name="sectionName"/> is null</exception>
        void BeginSection(string streamName, string sectionName);

        /// <summary>
        /// Ends the current section.
        /// </summary>
        /// <param name="streamName">The name of the execution log stream</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="streamName"/> is null</exception>
        /// <exception cref="InvalidOperationException">Thrown if there is no current section</exception>
        void EndSection(string streamName);

        /// <summary>
        /// Closes the execution log.
        /// </summary>
        void Close();
    }
}
