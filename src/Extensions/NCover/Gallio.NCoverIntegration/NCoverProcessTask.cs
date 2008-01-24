using System;
using System.Diagnostics;
using Gallio.Concurrency;
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
        /// <summary>
        /// Creates a process task.
        /// </summary>
        /// <param name="executablePath">The path of the executable executable</param>
        /// <param name="arguments">The arguments for the executable</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="executablePath"/>
        /// or <paramref name="arguments"/> is null</exception>
        public NCoverProcessTask(string executablePath, string arguments)
            : base(executablePath, arguments)
        {
        }

        /// <inheritdoc />
        protected override Process StartProcess(ProcessStartInfo startInfo)
        {
            ProfilerSettings settings = new ProfilerSettings();
            settings.CommandLineExe = startInfo.FileName;
            settings.CommandLineArgs = startInfo.Arguments;
            settings.WorkingDirectory = startInfo.WorkingDirectory;
            settings.NoLog = true;
            settings.CoverageXml = "Coverage.xml";

            return StartProfiler(settings, startInfo.RedirectStandardOutput | startInfo.RedirectStandardError);
        }

        private Process StartProfiler(ProfilerSettings settings, bool redirectOutput)
        {
            ProfilerDriver driver = new ProfilerDriver(settings);
            driver.ConfigureProfiler();

            Terminated += delegate
            {
                driver.WaitForExit();
                driver.UnregisterProfilerForUser();
            };

            driver.RegisterProfilerForUser();

            driver.Start(redirectOutput);
            driver.MessageCenter.SetDriverReadyEvent();

            if (!settings.NoLog)
                driver.StartLogging();

            return driver.Process;
        }
    }
}
