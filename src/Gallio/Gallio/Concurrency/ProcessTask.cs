// Copyright 2008 MbUnit Project - http://www.mbunit.com/
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
using System.Diagnostics;
using System.IO;
using System.Threading;
using Gallio.Hosting;
using Gallio.Logging;

namespace Gallio.Concurrency
{
    /// <summary>
    /// A process task provides support for launching external processes
    /// and collecting their output.  By default, the output is written
    /// to the <see cref="Log.Default" /> log stream.
    /// </summary>
    public class ProcessTask : Task
    {
        private readonly string executablePath;
        private readonly string arguments;

        private bool captureConsoleOutput = true;
        private bool captureConsoleError = true;
        private string workingDirectory;

        private bool logStreamWriterInitialized;
        private LogStreamWriter logStreamWriter;
        private StringWriter consoleOutputCaptureWriter;
        private StringWriter consoleErrorCaptureWriter;

        private Process process;
        private int exited;

        /// <summary>
        /// Creates a process task.
        /// </summary>
        /// <param name="executablePath">The path of the executable executable</param>
        /// <param name="arguments">The arguments for the executable</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="executablePath"/>
        /// or <paramref name="arguments"/> is null</exception>
        public ProcessTask(string executablePath, string arguments)
            : base(executablePath + @" " + arguments)
        {
            if (executablePath == null)
                throw new ArgumentNullException(@"executablePath");
            if (arguments == null)
                throw new ArgumentNullException(@"arguments");

            this.executablePath = Path.GetFullPath(executablePath);
            this.arguments = arguments;

            workingDirectory = Path.GetDirectoryName(this.executablePath);
        }

        /// <summary>
        /// Gets the executable path.
        /// </summary>
        public string ExecutablePath
        {
            get { return executablePath; }
        }

        /// <summary>
        /// Gets the arguments.
        /// </summary>
        public string Arguments
        {
            get { return arguments; }
        }

        /// <summary>
        /// Gets the <see cref="Process" /> that was started or null if the
        /// process has not been started yet.
        /// </summary>
        public Process Process
        {
            get { return process; }
        }

        /// <summary>
        /// <para>
        /// Gets or sets the <see cref="LogStreamWriter" /> to which the console output
        /// and error streams of the process should be logged along with the executable
        /// path, the arguments and the final exit code.  If this property is set to <c>null</c>,
        /// then logging will not occur.
        /// </para>
        /// <para>
        /// The default value is <see cref="Log.Default" />.
        /// </para>
        /// </summary>
        public LogStreamWriter LogStreamWriter
        {
            get
            {
                if (!logStreamWriterInitialized)
                    logStreamWriter = Log.Default;
                return logStreamWriter;
            }
            set
            {
                logStreamWriterInitialized = true;
                logStreamWriter = value;
            }
        }

        /// <summary>
        /// <para>
        /// Gets or sets whether console output stream of the process should be captured
        /// and made available via the <see cref="ConsoleOutput" /> property.
        /// </para>
        /// <para>
        /// The default value is <c>true</c>.
        /// </para>
        /// </summary>
        public bool CaptureConsoleOutput
        {
            get { return captureConsoleOutput; }
            set { captureConsoleOutput = value; }
        }

        /// <summary>
        /// <para>
        /// Gets or sets whether console error stream of the process should be captured
        /// and made available via the <see cref="ConsoleError" /> property.
        /// </para>
        /// <para>
        /// The default value is <c>true</c>.
        /// </para>
        /// </summary>
        public bool CaptureConsoleError
        {
            get { return captureConsoleError; }
            set { captureConsoleError = value; }
        }

        /// <summary>
        /// Gets the captured contents of the console output stream written by the process.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if the process has not been started
        /// or if <see cref="CaptureConsoleOutput" /> is <c>null</c></exception>
        public string ConsoleOutput
        {
            get
            {
                if (consoleOutputCaptureWriter == null)
                    throw new InvalidOperationException("The process has not been started or the console output stream is not being captured.");
                return consoleOutputCaptureWriter.ToString();
            }
        }

        /// <summary>
        /// Gets the captured contents of the console error stream written by the process.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if the process has not been started
        /// or if <see cref="CaptureConsoleError" /> is <c>null</c></exception>
        public string ConsoleError
        {
            get
            {
                if (consoleErrorCaptureWriter == null)
                    throw new InvalidOperationException("The process has not been started or the console error stream is not being captured.");
                return consoleErrorCaptureWriter.ToString();
            }
        }

        /// <summary>
        /// Gets the exit code of the process.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if the process has not completed</exception>
        public int ExitCode
        {
            get
            {
                if (process == null || ! process.HasExited)
                    throw new InvalidOperationException("The process has not completed.");
                return process.ExitCode;
            }
        }

        /// <summary>
        /// <para>
        /// Gets or sets the working directory path.
        /// </para>
        /// <para>
        /// By default, the working directory is the full path of the directory that
        /// contains <see cref="ExecutablePath" />.
        /// </para>
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null</exception>
        public string WorkingDirectory
        {
            get { return workingDirectory; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(@"value");
                workingDirectory = value;
            }
        }

        /// <inheritdoc />
        protected override void StartImpl()
        {
            if (process != null)
                throw new InvalidOperationException("The process has already been started.");

            if (captureConsoleOutput)
                consoleOutputCaptureWriter = new StringWriter();
            if (captureConsoleError)
                consoleErrorCaptureWriter = new StringWriter();

            ProcessStartInfo startInfo = new ProcessStartInfo(executablePath, arguments);

            startInfo.WorkingDirectory = workingDirectory;
            startInfo.UseShellExecute = false;
            startInfo.CreateNoWindow = true;
            startInfo.RedirectStandardOutput = LogStreamWriter != null || captureConsoleOutput;
            startInfo.RedirectStandardError = LogStreamWriter != null || captureConsoleError;

            process = StartProcess(startInfo);
            process.EnableRaisingEvents = true;

            StartLogging();

            // Handle process exit including the case where the process might already
            // have exited just prior to adding the event handler.
            process.Exited += delegate { HandleProcessExit(); };
            if (process.HasExited)
                HandleProcessExit();
        }

        /// <summary>
        /// Starts a <see cref="Process" />.
        /// </summary>
        /// <remarks>
        /// This method may be override to change how the process is created and
        /// started.
        /// </remarks>
        /// <param name="startInfo">The <see cref="ProcessStartInfo" /> that has been started</param>
        /// <returns>The process</returns>
        protected virtual Process StartProcess(ProcessStartInfo startInfo)
        {
            return Process.Start(startInfo);
        }

        private void HandleProcessExit()
        {
            if (Interlocked.Exchange(ref exited, 1) == 0)
            {
                StopLogging();
                NotifyTerminated(TaskResult.CreateFromValue(process.ExitCode));
            }
        }

        /// <inheritdoc />
        protected override void AbortImpl()
        {
            if (process != null && !process.HasExited)
            {
                LogAbort();

                process.Kill();
            }
        }

        /// <inheritdoc />
        protected override bool JoinImpl(TimeSpan timeout)
        {
            return process.WaitForExit((int) timeout.TotalMilliseconds);
        }

        private void StartLogging()
        {
            if (LogStreamWriter != null)
                BeginLogSection();

            if (process.StartInfo.RedirectStandardOutput)
            {
                if (LogStreamWriter != null)
                    process.OutputDataReceived += WriteDataReceivedToLog;

                if (consoleOutputCaptureWriter != null)
                    process.OutputDataReceived += delegate(object sender, DataReceivedEventArgs e) { consoleOutputCaptureWriter.WriteLine(e.Data); };

                process.BeginOutputReadLine();
            }

            if (process.StartInfo.RedirectStandardError)
            {
                if (LogStreamWriter != null)
                    process.ErrorDataReceived += WriteDataReceivedToLog;

                if (consoleErrorCaptureWriter != null)
                    process.ErrorDataReceived += delegate(object sender, DataReceivedEventArgs e) { consoleErrorCaptureWriter.WriteLine(e.Data); };

                process.BeginErrorReadLine();
            }
        }

        private void StopLogging()
        {
            if (process.StartInfo.RedirectStandardOutput)
                process.CancelOutputRead();
            if (process.StartInfo.RedirectStandardError)
                process.CancelErrorRead();

            if (LogStreamWriter != null)
                EndLogSection();
        }

        private void BeginLogSection()
        {
            LogStreamWriter.BeginSection(String.Concat("Run Process: " + executablePath, @" ", arguments));
        }

        private void WriteDataReceivedToLog(object sender, DataReceivedEventArgs e)
        {
            try
            {
                LogStreamWriter.WriteLine(e.Data);
            }
            catch (Exception ex)
            {
                Panic.UnhandledException("Cannot write process task output to the log stream.", ex);
            }
        }

        private void EndLogSection()
        {
            try
            {
                if (process != null)
                    LogStreamWriter.WriteLine("Exit Code: {0}", process.ExitCode);

                LogStreamWriter.EndSection();
            }
            catch (Exception ex)
            {
                Panic.UnhandledException("Cannot write process task section end message to the log stream.", ex);
            }
        }

        private void LogAbort()
        {
            try
            {
                if (LogStreamWriter != null)
                {
                    LogStreamWriter.BeginSection("Abort requested.  Killing the process!").Dispose();
                }
            }
            catch (Exception ex)
            {
                Panic.UnhandledException("Cannot write process task abort message to the log stream.", ex);
            }
        }
    }
}