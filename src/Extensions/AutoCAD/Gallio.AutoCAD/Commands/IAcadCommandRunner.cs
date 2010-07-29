// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
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
using Gallio.Common.Concurrency;

namespace Gallio.AutoCAD.Commands
{
    /// <summary>
    /// Runs commands in a remote AutoCAD process.
    /// </summary>
    public interface IAcadCommandRunner
    {
        /// <summary>
        /// Begins an asynchronous command in the specified AutoCAD process.
        /// </summary>
        /// <param name="command">The command to run.</param>
        /// <param name="process">The AutoCAD process.</param>
        /// <param name="completionCallback">A method to be called when the asynchronous command is completed.</param>
        /// <param name="asyncState">A object that can be used to distinguish this asynchronous command from others.</param>
        IAsyncResult BeginRun(AcadCommand command, IProcess process, AsyncCallback completionCallback, object asyncState);

        /// <summary>
        /// Ends an asynchronous command, blocking until the command has completed.
        /// </summary>
        /// <param name="result">The pending asynchronous command.</param>
        void EndRun(IAsyncResult result);
    }
}
