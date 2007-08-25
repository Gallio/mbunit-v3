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
using MbUnit.Framework.Kernel.Contexts;

namespace MbUnit.Framework.Kernel.ExecutionLogs
{
    /// <summary>
    /// The execution log service manages the execution log for a given test execution context.
    /// </summary>
    /// <remarks>
    /// The operations on this interface are thread-safe.
    /// </remarks>
    public interface IExecutionLogService
    {
        /// <summary>
        /// Gets the execution log for the specified context.
        /// </summary>
        /// <param name="context">The test execution context</param>
        /// <returns>The execution log, never null</returns>
        IExecutionLog GetExecutionLog(IContext context);
    }
}
