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

namespace Gallio.Model.Execution
{
    /// <summary>
    /// Writes messages to a test's execution log.
    /// </summary>
    public interface ITestLogWriter
    {
        /// <summary>
        /// Closes the log writer.
        /// </summary>
        void Close();

        /// <summary>
        /// Attaches an text attachment to the execution log.
        /// </summary>
        /// <param name="attachmentName">The attachment name</param>
        /// <param name="contentType">The content type</param>
        /// <param name="text">The text contents</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="attachmentName"/>,
        /// <paramref name="contentType"/> or <paramref name="text"/> is null</exception>
        /// <exception cref="InvalidOperationException">Thrown if there is already an attachment with the same name</exception>
        /// <exception cref="InvalidOperationException">Thrown if the log writer has been closed</exception>
        void AttachText(string attachmentName, string contentType, string text);

        /// <summary>
        /// Attaches an binary attachment to the execution log.
        /// </summary>
        /// <param name="attachmentName">The attachment name</param>
        /// <param name="contentType">The content type</param>
        /// <param name="bytes">The binary contents</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="attachmentName"/>,
        /// <paramref name="contentType"/> or <paramref name="bytes"/> is null</exception>
        /// <exception cref="InvalidOperationException">Thrown if there is already an attachment with the same name</exception>
        /// <exception cref="InvalidOperationException">Thrown if the log writer has been closed</exception>
        void AttachBytes(string attachmentName, string contentType, byte[] bytes);

        /// <summary>
        /// Writes text to a particular stream of the execution log.
        /// </summary>
        /// <param name="streamName">The log stream name</param>
        /// <param name="text">The text to write</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="streamName"/> or <paramref name="text"/>
        /// is null</exception>
        /// <exception cref="InvalidOperationException">Thrown if the log writer has been closed</exception>
        void Write(string streamName, string text);

        /// <summary>
        /// Embeds an attachment into a particular stream of the execution log.
        /// </summary>
        /// <param name="streamName">The log stream name</param>
        /// <param name="attachmentName">The attachment name</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="streamName"/>
        /// or <paramref name="attachmentName"/> is null</exception>
        /// <exception cref="InvalidOperationException">Thrown if there is no attachment with the specified name</exception>
        /// <exception cref="InvalidOperationException">Thrown if the log writer has been closed</exception>
        void Embed(string streamName, string attachmentName);

        /// <summary>
        /// Begins a section in a particular stream of the execution log.
        /// </summary>
        /// <param name="streamName">The log stream name</param>
        /// <param name="sectionName">The name of the section</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="streamName"/> or
        /// <paramref name="sectionName"/> is null</exception>
        /// <exception cref="InvalidOperationException">Thrown if the log writer has been closed</exception>
        void BeginSection(string streamName, string sectionName);

        /// <summary>
        /// Ends a section in a particular stream of the execution log.
        /// </summary>
        /// <param name="streamName">The log stream name</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="streamName"/> is null</exception>
        /// <exception cref="InvalidOperationException">Thrown if there is no current section in the stream</exception>
        /// <exception cref="InvalidOperationException">Thrown if the log writer has been closed</exception>
        void EndSection(string streamName);
    }
}
