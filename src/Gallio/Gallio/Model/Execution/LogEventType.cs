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

namespace Gallio.Model.Execution
{
    /// <summary>
    /// Describes the types of test execution log events.
    /// </summary>
    public enum LogEventType
    {
        /// <summary>
        /// Adds a text attachment to a log.
        /// </summary>
        AttachText,

        /// <summary>
        /// Adds a binary attachment to a log.
        /// </summary>
        AttachBytes,

        /// <summary>
        /// Writes text to a log stream.
        /// </summary>
        Write,

        /// <summary>
        /// Embeds an existing named attachment into a log stream.
        /// </summary>
        Embed,

        /// <summary>
        /// Begins a section within a log stream.
        /// </summary>
        BeginSection,

        /// <summary>
        /// Ends a section of a log stream.
        /// </summary>
        EndSection
    }
}