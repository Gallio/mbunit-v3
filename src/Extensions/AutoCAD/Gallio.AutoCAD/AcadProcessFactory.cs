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
using System.Diagnostics;
using System.IO;
using Gallio.Runner;
using Gallio.Runtime.Logging;

namespace Gallio.AutoCAD
{
    /// <summary>
    /// Creates <see cref="IAcadProcess"/> objects.
    /// </summary>
    public class AcadProcessFactory : IAcadProcessFactory
    {
        private readonly ILogger logger;

        /// <summary>
        /// Intializes a new <see cref="AcadProcessFactory"/> instance.
        /// </summary>
        /// <param name="logger">An <see cref="ILogger"/> instance.</param>
        public AcadProcessFactory(ILogger logger)
        {
            if (logger == null)
                throw new ArgumentNullException("logger");
            this.logger = logger;
        }

        /// <inheritdoc/>
        public IAcadProcess CreateProcess()
        {
            if (AttachToExistingProcess)
            {
                if (!String.IsNullOrEmpty(AcadExePath))
                {
                    logger.Log(LogSeverity.Warning, "Ignoring specified path to acad.exe and attaching to an existing process.");
                }
                
                Process[] processes = Process.GetProcessesByName("acad");
                if (processes.Length == 0)
                {
                    throw new RunnerException("Unable to attach to acad.exe: No existing acad.exe instances found.");
                }
                if (processes.Length > 1)
                {
                    logger.Log(LogSeverity.Warning, "Multiple acad.exe instances found. Choosing one arbitrarily.");
                }

                logger.Log(LogSeverity.Debug, String.Concat("Attaching to AutoCAD instance: ", processes[0].MainModule.FileName));
                return AcadProcess.Attach(processes[0]);
            }

            string executablePath;
            if (!String.IsNullOrEmpty(AcadExePath))
            {
                if (!File.Exists(AcadExePath))
                {
                    throw new RunnerException(String.Concat("Executable not found: \"", AcadExePath, "\"."));
                }
                executablePath = AcadExePath;
            }
            else
            {
                executablePath = AcadLocator.GetAcadLocation();
            }

            executablePath = Path.GetFullPath(executablePath);
            logger.Log(LogSeverity.Debug, String.Concat("Creating new AutoCAD instance: ", executablePath));
            return AcadProcess.Create(executablePath);
        }

        /// <summary>
        /// Set to <c>true</c> to have Gallio attach to an existing
        /// AutoCAD process; otherwise, set to false to always create
        /// new AutoCAD instances.
        /// </summary>
        public bool AttachToExistingProcess
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the path to <c>acad.exe</c>.
        /// </summary>
        public string AcadExePath
        {
            get;
            set;
        }
    }
}
