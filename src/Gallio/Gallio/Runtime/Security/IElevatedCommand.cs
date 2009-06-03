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
using System.Text;
using Gallio.Runtime.ProgressMonitoring;

namespace Gallio.Runtime.Security
{
    /// <summary>
    /// Represents a command to be executed in an elevated context.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The command parameters and results must be serializable because they may be
    /// transmitted across processes.
    /// </para>
    /// </remarks>
    public interface IElevatedCommand
    {
        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="arguments">The command arguments.</param>
        /// <param name="progressMonitor">The progress monitor, non-null.</param>
        /// <returns>The command result, must be null or serializable</returns>
        object Execute(object arguments, IProgressMonitor progressMonitor);
    }
}
