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
using Gallio.AutoCAD.Commands;
using Gallio.Common.Concurrency;
using Gallio.Runtime.Debugging;
using Gallio.Runtime.Logging;

namespace Gallio.AutoCAD.ProcessManagement
{
    /// <summary>
    /// Implementation of <see cref="IAcadProcess"/> that represents
    /// an existing AutoCAD process appropriated by Gallio to execute code in.
    /// </summary>
    /// <remarks>
    /// The AutoCAD process will remain running after Gallio is finished
    /// using it. However, it is unlikely that subsequent test runs would
    /// be successful within the same process.
    /// </remarks>
    public class ExistingAcadProcess : AcadProcessBase
    {
        private readonly IProcess process;

        /// <summary>
        /// Creates a new <see cref="ExistingAcadProcess"/> instane.
        /// </summary>
        /// <param name="logger">A logger.</param>
        /// <param name="commandRunner">An AutoCAD command runner.</param>
        /// <param name="process">The existing AutoCAD process.</param>
        public ExistingAcadProcess(ILogger logger, IAcadCommandRunner commandRunner, IProcess process)
            : base(logger, commandRunner)
        {
            if (process == null)
                throw new ArgumentNullException("process");

            this.process = process;
        }

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Do not Kill() the process; we didn't start it.
                process.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <inheritdoc/>
        protected override IProcess StartProcess(DebuggerSetup debuggerSetup)
        {
            return process;
        }
    }
}
