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
using System.Diagnostics;
using System.Threading;
using Gallio.AutoCAD.Commands;
using Gallio.Common.Concurrency;
using Gallio.Runtime.Debugging;
using Gallio.Runtime.Logging;

namespace Gallio.AutoCAD.ProcessManagement
{
    /// <summary>
    /// Implementation of <see cref="IAcadProcess"/> that represents
    /// a newly created AutoCAD process.
    /// </summary>
    /// <remarks>
    /// The AutoCAD process will be shut down when it is no longer in use by Gallio.
    /// </remarks>
    public class CreatedAcadProcess : AcadProcessBase
    {
        private readonly ProcessStartInfo startInfo;
        private readonly IProcessCreator processCreator;
        private readonly IDebuggerManager debuggerManager;
        private IProcess process;

        /// <summary>
        /// Creates a new <see cref="CreatedAcadProcess"/> instance.
        /// </summary>
        /// <param name="logger">A logger.</param>
        /// <param name="commandRunner">A AutoCAD command runner.</param>
        /// <param name="executable">The path to the AutoCAD executable.</param>
        /// <param name="processCreator">A process creator.</param>
        /// <param name="debuggerManager">A debugger mananger.</param>
        public CreatedAcadProcess(ILogger logger, IAcadCommandRunner commandRunner,
            string executable, IProcessCreator processCreator, IDebuggerManager debuggerManager)
            : base(logger, commandRunner)
        {
            if (executable == null)
                throw new ArgumentNullException("executable");
            if (processCreator == null)
                throw new ArgumentNullException("processCreator");
            if (debuggerManager == null)
                throw new ArgumentNullException("debuggerManager");

            startInfo = new ProcessStartInfo(executable);
            this.processCreator = processCreator;
            this.debuggerManager = debuggerManager;
        }

        /// <inheritdoc/>
        protected override IProcess StartProcess(DebuggerSetup debuggerSetup)
        {
            if (process != null)
                throw new InvalidOperationException("Process already started.");

            process = StartProcessWithDebugger(debuggerSetup) ?? processCreator.Start(startInfo);
            return process;
        }

        private IProcess StartProcessWithDebugger(DebuggerSetup debuggerSetup)
        {
            if (debuggerSetup == null)
                return null;

            var debugger = debuggerManager.GetDebugger(debuggerSetup, Logger);
            var actual = debugger.LaunchProcess(startInfo);
            return new ProcessWrapper(actual);
        }

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            if (disposing && process != null)
            {
                var ownedProcess = Interlocked.Exchange(ref process, null);
                if (ownedProcess == null)
                    return;

                if (!ownedProcess.HasExited)
                    ownedProcess.Kill();

                ownedProcess.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Gets the command line arguments passed to the AutoCAD process.
        /// </summary>
        public string Arguments
        {
            get { return startInfo.Arguments; }
            set { startInfo.Arguments = value; }
        }

        /// <summary>
        /// Gets the path the to AutoCAD executable.
        /// </summary>
        public string FileName
        {
            get { return startInfo.FileName; }
        }

        /// <summary>
        /// Gets the working directory used to start a new AutoCAD process.
        /// </summary>
        public string WorkingDirectory
        {
            get { return startInfo.WorkingDirectory; }
            set { startInfo.WorkingDirectory = value; }
        }
    }
}