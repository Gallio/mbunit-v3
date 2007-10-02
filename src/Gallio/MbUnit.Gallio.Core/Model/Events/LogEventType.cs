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

using MbUnit.Framework.Kernel.ExecutionLogs;

namespace MbUnit.Core.Model.Events
{
    /// <summary>
    /// Describes the types of test execution log events.
    /// </summary>
    public enum LogEventType
    {
        /// <summary>
        /// Adds an attachment to a log.
        /// </summary>
        /// <seealso cref="LogWriter.Attach"/>
        Attach,

        /// <summary>
        /// Writes text to a log stream.
        /// </summary>
        /// <seealso cref="LogStreamWriter.Write(string)"/>
        Write,

        /// <summary>
        /// Embeds an existing named attachment into a log stream.
        /// </summary>
        /// <seealso cref="LogStreamWriter.EmbedExisting"/>
        EmbedExisting,

        /// <summary>
        /// Begins a section within a log stream.
        /// </summary>
        /// <seealso cref="LogStreamWriter.BeginSection"/>
        BeginSection,

        /// <summary>
        /// Ends a section of a log stream.
        /// </summary>
        /// <seealso cref="LogStreamWriter.EndSection"/>
        EndSection
    }
}