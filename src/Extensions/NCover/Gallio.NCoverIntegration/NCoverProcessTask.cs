// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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
using Gallio.Runtime.Logging;
using Gallio.Concurrency;
using Gallio.Runtime.Hosting;
using Gallio.Runtime;
using NCover.Framework;

namespace Gallio.NCoverIntegration
{
    /// <summary>
    /// A <see cref="ProcessTask" /> that uses the NCover framework to
    /// start the process with NCover attached.
    /// </summary>
    /// <todo author="jeff">
    /// Support NCover configuration settings using the test runner options collection.
    /// </todo>
    public class NCoverProcessTask : ProcessTask
    {
        // Note: NCover can take a long time to finish writing out its results.
        private readonly TimeSpan WaitForExitTimeout = TimeSpan.FromSeconds(120);

        private readonly ILogger logger;
        private ProfilerDriver driver;
        private ThreadTask waitForExitTask;

        /// <summary>
        /// Creates a process task.
        /// </summary>
        /// <param name="executablePath">The path of the executable executable</param>
        /// <param name="arguments">The arguments for the executable</param>
        /// <param name="workingDirectory">The working directory</param>
        /// <param name="logger">The logger</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="executablePath"/>,
        /// <paramref name="arguments"/>, <paramref name="workingDirectory"/> or <paramref name="logger" /> is null</exception>
        public NCoverProcessTask(string executablePath, string arguments, string workingDirectory, ILogger logger)
            : base(executablePath, arguments, workingDirectory)
        {
            if (logger == null)
                throw new ArgumentNullException("logger");

            this.logger = logger;
        }

        /// <inheritdoc />
        protected override Process StartProcess(ProcessStartInfo startInfo)
        {
            string outputDirectory = startInfo.WorkingDirectory;

            ProfilerSettings settings = new ProfilerSettings();
            settings.CommandLineExe = startInfo.FileName;
            settings.CommandLineArgs = startInfo.Arguments;
            settings.WorkingDirectory = startInfo.WorkingDirectory;
            settings.NoLog = true;
            //settings.LogFile = Path.Combine(outputDirectory, "Coverage.log");
            settings.CoverageXml = Path.Combine(outputDirectory, "Coverage.xml");
            //settings.CoverageHtmlPath = Path.Combine(outputDirectory, "Coverage.html");
            settings.RegisterForUser = true;

            return RegisterAndStartProfiler(settings, startInfo.RedirectStandardOutput | startInfo.RedirectStandardError);
        }

        private Process RegisterAndStartProfiler(ProfilerSettings settings, bool redirectOutput)
        {
            logger.Log(LogSeverity.Info, "* Starting NCover profiler.");

            driver = new ProfilerDriver(settings);

            RegisterProfilerIfNeeded();

            driver.Start(redirectOutput);
            if (!driver.MessageCenter.WaitForProfilerReadyEvent())
            {
                logger.Log(LogSeverity.Error, "* Timed out waiting for the NCover profiler to become ready.  The launch may have failed because this version of NCover does not support running programs in 64bit mode.");
                throw new HostException("Timed out waiting for the NCover profiler to become ready.");
            }

            driver.ConfigureProfiler();
            driver.MessageCenter.SetDriverReadyEvent();

            if (!settings.NoLog)
                driver.StartLogging();

            waitForExitTask = new ThreadTask("NCover Profiler Wait for Exit", (Action) WaitForExit);
            waitForExitTask.Start();

            return driver.Process;
        }

        protected override void OnTerminated()
        {
            try
            {
                if (driver != null)
                {
                    if (driver.Process != null)
                    {
                        try
                        {
                            driver.Stop();
                        }
                        catch (InvalidOperationException)
                        {
                            // An invalid operation exception may occur when attempting to kill a process
                            // that has already stopped so we ignore it.
                        }
                    }

                    UnregisterProfilerIfNeeded();
                }
            }
            catch (Exception ex)
            {
                UnhandledExceptionPolicy.Report("An exception occurred while shutting down the NCover profiler.", ex);
            }
            finally
            {
                // Allow some time for the final processing to take place such as writing out the 
                // XML reports.  If it really takes too long then abort it.
                logger.Log(LogSeverity.Info, "* Waiting for NCover to exit.");
                if (waitForExitTask != null && ! waitForExitTask.Join(WaitForExitTimeout))
                {
                    logger.Log(LogSeverity.Error, "* Timed out.  Aborting NCover.");
                    waitForExitTask.Abort();
                }

                driver = null;
                waitForExitTask = null;
                base.OnTerminated();
            }
        }

        private void WaitForExit()
        {
            try
            {
                ProfilerDriver cachedDriver = driver;
                if (cachedDriver == null)
                    return;

                cachedDriver.WaitForExit();
            }
            catch (Exception ex)
            {
                UnhandledExceptionPolicy.Report("An exception occurred while waiting for the NCover profiler to exit.\n"
                   + "If the exception is an EndOfStreamException or similar notice then we have encountered a "
                   + "known bug in NCover v1.5.8 that occurs when the profiling trace is too large.  Unfortunately there is "
                   + "no workaround at this time except to upgrade to NCover v2+ or use a different profiler.", ex);
            }
        }

        private void RegisterProfilerIfNeeded()
        {
            if (driver.Settings.RegisterForUser)
                driver.RegisterProfilerForUser();
        }

        private void UnregisterProfilerIfNeeded()
        {
            try
            {
                if (driver.Settings.RegisterForUser)
                    driver.UnregisterProfilerForUser();
            }
            catch (ArgumentException)
            {
                // The exception we are ignoring here looks like this:
                //     "System.ArgumentException: Cannot delete a subkey tree because the subkey does not exist."
                //
                // NCover does not check whether the driver has already been unregistered
                // by other means.  This can happen if the user runs other instances NCover
                // concurrently and one of those processes unregisters the profiler before
                // we reach this code.  Fortunately the registration is only needed on application
                // startup.
            }
        }
    }
}
