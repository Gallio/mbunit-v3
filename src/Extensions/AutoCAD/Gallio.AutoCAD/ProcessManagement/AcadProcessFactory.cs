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
using System.IO;
using System.Runtime.InteropServices;
using Gallio.AutoCAD.Commands;
using Gallio.AutoCAD.Preferences;
using Gallio.Common.Concurrency;
using Gallio.Common.IO;
using Gallio.Model.Isolation;
using Gallio.Runtime.Debugging;
using Gallio.Runtime.Logging;

namespace Gallio.AutoCAD.ProcessManagement
{
    /// <summary>
    /// Creates <see cref="IAcadProcess"/> objects.
    /// </summary>
    public class AcadProcessFactory : IAcadProcessFactory
    {
        private readonly ILogger logger;
        private readonly IFileSystem fileSystem;
        private readonly IProcessFinder processFinder;
        private readonly IProcessCreator processCreator;
        private readonly IDebuggerManager debuggerManager;
        private readonly IAcadPreferenceManager preferenceManager;
        private readonly IAcadLocator acadLocator;
        private readonly IAcadPluginLocator pluginLocator;

        /// <summary>
        /// Intializes a new <see cref="AcadProcessFactory"/> instance.
        /// </summary>
        /// <param name="logger">An <see cref="ILogger"/> instance.</param>
        /// <param name="fileSystem">An <see cref="IFileSystem"/> instance.</param>
        /// <param name="processFinder">A process finder.</param>
        /// <param name="processCreator">A process creator.</param>
        /// <param name="debuggerManager">An <see cref="IDebuggerManager"/> instance.</param>
        /// <param name="preferenceManager">The AutoCAD preference manager.</param>
        /// <param name="acadLocator">The AutoCAD locator.</param>
        /// <param name="pluginLocator">An AutoCAD plugin locator.</param>
        public AcadProcessFactory(ILogger logger, IFileSystem fileSystem,
            IProcessFinder processFinder, IProcessCreator processCreator,
            IDebuggerManager debuggerManager, IAcadPreferenceManager preferenceManager,
            IAcadLocator acadLocator, IAcadPluginLocator pluginLocator)
        {
            if (logger == null)
                throw new ArgumentNullException("logger");
            if (fileSystem == null)
                throw new ArgumentNullException("fileSystem");
            if (processFinder == null)
                throw new ArgumentNullException("processFinder");
            if (processCreator == null)
                throw new ArgumentNullException("processCreator");
            if (debuggerManager == null)
                throw new ArgumentNullException("debuggerManager");
            if (preferenceManager == null)
                throw new ArgumentNullException("preferenceManager");
            if (acadLocator == null)
                throw new ArgumentNullException("acadLocator");
            if (pluginLocator == null)
                throw new ArgumentNullException("pluginLocator");

            this.logger = logger;
            this.fileSystem = fileSystem;
            this.processFinder = processFinder;
            this.processCreator = processCreator;
            this.debuggerManager = debuggerManager;
            this.preferenceManager = preferenceManager;
            this.acadLocator = acadLocator;
            this.pluginLocator = pluginLocator;
        }

        /// <inheritdoc/>
        /// <remarks>
        /// <para>
        /// <c>CreateProcess</c> creates <see cref="IAcadProcess"/> objects that use AutoCAD's
        /// OLE Automation support for the initial communication with the AutoCAD process.
        /// Unfortunately, AutoCAD does not register each process with a unique moniker so
        /// retrieving the "AutoCAD.Application" object for a specific process using
        /// <see cref="Marshal.GetActiveObject(string)"/> is not possible. To handle this the
        /// <see cref="CreateProcess"/> method throws <see cref="NotSupportedException"/>
        /// whenever it is unsure what AutoCAD process it will acquire from the ROT.
        /// </para>
        /// </remarks>
        /// <exception cref="InvalidOperationException">
        /// If attaching to an existing process and none can be found.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// If creating a new AutoCAD process and an existing one is found.
        /// If attaching to an existing process and more than one is found.
        /// </exception>
        public IAcadProcess CreateProcess(TestIsolationOptions options)
        {
            if (options == null)
                throw new ArgumentNullException("options");

            var startupAction = GetStartupAction(options, preferenceManager);
            if (startupAction == StartupAction.AttachToExisting)
            {
                var existing = GetExistingAcadProcess();
                return AttachToProcess(existing);
            }

            var executable = GetExecutable(options, preferenceManager, acadLocator);
            var workingDirectory = GetWorkingDirectory(preferenceManager);
            var arguments = GetArguments(preferenceManager);
            return CreateNewProcess(executable, workingDirectory, arguments);
        }

        private IProcess GetExistingAcadProcess()
        {
            var processes = processFinder.GetProcessesByName("acad");
            if (processes.Length == 0)
            {
                const string errorMessage = "Unable to attach to acad.exe. No existing acad.exe instances found.";
                logger.Log(LogSeverity.Error, string.Concat(errorMessage, " Ensure AutoCAD is running or change the AutoCAD startup action."));
                throw new InvalidOperationException(errorMessage);
            }

            if (processes.Length > 1)
            {
                logger.Log(LogSeverity.Error,
                   "Error attaching to AutoCAD. Multiple AutoCAD processes are running. " +
                   "Ensure a single AutoCAD process is running and try again.");
                throw new NotSupportedException("Multiple AutoCAD processes found.");
            }

            return processes[0];
        }

        private IAcadProcess AttachToProcess(IProcess process)
        {
            return new ExistingAcadProcess(logger, new AcadCommandRunner(), process, pluginLocator);
        }

        private IAcadProcess CreateNewProcess(string executable, string workingDirectory, string arguments)
        {
            ValidateExecutablePath(executable);
            ValidateWorkingDirectory(workingDirectory);

            if (processFinder.GetProcessesByName("acad").Length > 0)
            {
                logger.Log(LogSeverity.Error,
                    "Error starting AutoCAD. An AutoCAD process is already running. " +
                    "Close any existing AutoCAD instances and try again.");

                throw new NotSupportedException("An AutoCAD process is already running.");
            }

            var process = new CreatedAcadProcess(logger, new AcadCommandRunner(), executable, processCreator, debuggerManager, pluginLocator)
                              {
                                  Arguments = arguments,
                                  WorkingDirectory = workingDirectory
                              };

            return process;
        }

        private void ValidateExecutablePath(string executable)
        {
            if (executable == null)
                throw new ArgumentException("Exectuable path can't be null.");
            if (executable.IndexOfAny(Path.GetInvalidPathChars()) != -1)
                throw new ArgumentException("Path contains invalid characters.", "executable");
            if (!fileSystem.FileExists(executable))
                throw new FileNotFoundException("File not found.", executable);
        }

        private void ValidateWorkingDirectory(string workingDirectory)
        {
            if (string.IsNullOrEmpty(workingDirectory))
                return;

            if (workingDirectory.IndexOfAny(Path.GetInvalidPathChars()) != -1)
                throw new ArgumentException("Path contains invalid characters.", "workingDirectory");
            if (!fileSystem.DirectoryExists(workingDirectory))
                throw new DirectoryNotFoundException(String.Concat("Directory not found: \"", workingDirectory, "\"."));
        }

        private static StartupAction GetStartupAction(TestIsolationOptions options, IAcadPreferenceManager preferenceManager)
        {
            // Startup actions from test isolation options take precedence.
            var attach = GetAttachOption(options);
            if (attach.HasValue && attach.Value)
                return StartupAction.AttachToExisting;

            var executable = options.Properties.GetValue("AcadExePath");
            if (!string.IsNullOrEmpty(executable))
                return StartupAction.StartUserSpecified;

            // Fall back to preference manager, ensuring that "AcadAttachToExisting=false" is honored.
            var preference = preferenceManager.StartupAction;
            if (preference == StartupAction.AttachToExisting && attach.HasValue)
                return StartupAction.StartMostRecentlyUsed;

            return preference;
        }

        private static bool? GetAttachOption(TestIsolationOptions options)
        {
            var attachOption = options.Properties.GetValue("AcadAttachToExisting");
            if (attachOption == null)
                return null;

            bool attach;
            if (!bool.TryParse(attachOption, out attach))
                throw new ArgumentException(string.Format("Error parsing AcadAttachToExisting value: \"{0}\"", attachOption));

            return attach;
        }

        private static string GetExecutable(TestIsolationOptions options, IAcadPreferenceManager preferenceManager, IAcadLocator acadLocator)
        {
            var executable = options.Properties.GetValue("AcadExePath");
            if (!string.IsNullOrEmpty(executable))
                return executable;

            if (preferenceManager.StartupAction == StartupAction.StartUserSpecified)
                return preferenceManager.UserSpecifiedExecutable;

            return acadLocator.GetMostRecentlyUsed();
        }

        private static string GetWorkingDirectory(IAcadPreferenceManager manager)
        {
            var dir = manager.WorkingDirectory;
            return string.IsNullOrEmpty(dir) ? null : dir;
        }

        private static string GetArguments(IAcadPreferenceManager manager)
        {
            var args = manager.CommandLineArguments;
            return string.IsNullOrEmpty(args) ? null : args;
        }
    }
}