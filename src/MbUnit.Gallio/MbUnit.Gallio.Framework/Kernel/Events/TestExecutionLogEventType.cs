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
using System.Collections.Generic;
using System.Text;
using MbUnit.Framework.Services.ExecutionLogs;

namespace MbUnit.Framework.Kernel.Events
{
    /// <summary>
    /// Describes the types of test execution log events.
    /// </summary>
    public enum TestExecutionLogEventType
    {
        /// <summary>
        /// Write text to the a execution log.
        /// </summary>
        /// <seealso cref="IExecutionLogWriter.WriteText"/>
        WriteText,

        /// <summary>
        /// Write an attachment to a test execution log.
        /// </summary>
        /// <seealso cref="IExecutionLogWriter.WriteAttachment"/>
        WriteAttachment,

        /// <summary>
        /// Begin a section within a test execution log stream.
        /// </summary>
        /// <seealso cref="IExecutionLogWriter.BeginSection"/>
        BeginSection,

        /// <summary>
        /// End a section of a test execution log stream.
        /// </summary>
        /// <seealso cref="IExecutionLogWriter.EndSection"/>
        EndSection,

        /// <summary>
        /// Close a test execution log .
        /// </summary>
        /// <seealso cref="IExecutionLogWriter.Close"/>
        Close
    }
}
