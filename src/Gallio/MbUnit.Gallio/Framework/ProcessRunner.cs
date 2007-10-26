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
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using MbUnit.Logging;

namespace MbUnit.Framework
{
    /// <summary>
    /// A process runner provides support for launching external processes
    /// as part of integration tests.  By default, the output of the process
    /// is copied to the test log.
    /// </summary>
    public class ProcessRunner
    {
        private readonly string executablePath;
        private readonly string arguments;

        private bool captureConsoleOutput = true;
        private bool captureConsoleError = true;
        private string workingDirectory;

        private LogStreamWriter logStreamWriter;
        private StringWriter consoleOutputCaptureWriter;
        private StringWriter consoleErrorCaptureWriter;

        private Process process;

        /// <summary>
        /// Creates a process runner.
        /// </summary>
        /// <param name="executablePath">The path of the executable executable</param>
        /// <param name="arguments">The arguments for the executable</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="executablePath"/>
        /// or <paramref name="arguments"/> is null</exception>
        public ProcessRunner(string executablePath, string arguments)
        {
            if (executablePath == null)
                throw new ArgumentNullException(@"executablePath");
            if (arguments == null)
                throw new ArgumentNullException(@"arguments");

            this.executablePath = Path.GetFullPath(executablePath);
            this.arguments = arguments;

            workingDirectory = Path.GetDirectoryName(this.executablePath);
            logStreamWriter = Log.Default;
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
            get { return logStreamWriter; }
            set { logStreamWriter = value; }
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

        /// <summary>
        /// Runs the process synchronously with no timeout.
        /// </summary>
        /// <returns>The exit code of the process</returns>
        /// <exception cref="InvalidOperationException">Thrown if the process has already been started</exception>
        public int Run()
        {
            return Run(Timeout.Infinite);
        }

        /// <summary>
        /// <para>
        /// Runs the process synchronously with the specified timeout.
        /// </para>
        /// <para>
        /// If the process runs for longer than <paramref name="timeoutMilliseconds"/>
        /// then it is automatically killed.
        /// </para>
        /// </summary>
        /// <param name="timeoutMilliseconds">The timeout in milliseconds, or <see cref="Timeout.Infinite" /> if none</param>
        /// <returns>The exit code of the process</returns>
        /// <exception cref="InvalidOperationException">Thrown if the process has already been started</exception>
        public int Run(int timeoutMilliseconds)
        {
            Start();

            if (!WaitForExit(timeoutMilliseconds))
                Kill();

            return ExitCode;
        }

        /// <summary>
        /// Starts the process asynchronously.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if the process has already been started</exception>
        public void Start()
        {
            if (process != null)
                throw new InvalidOperationException("The process has already been started.");

            if (captureConsoleOutput)
                consoleOutputCaptureWriter = new StringWriter();
            if (captureConsoleError)
                consoleErrorCaptureWriter = new StringWriter();

            // Prepare process start parameters.
            ProcessStartInfo startInfo = new ProcessStartInfo(executablePath, arguments);

            startInfo.WorkingDirectory = workingDirectory;
            startInfo.UseShellExecute = false;
            startInfo.CreateNoWindow = true;

            startInfo.RedirectStandardOutput = logStreamWriter != null || consoleOutputCaptureWriter != null;
            startInfo.RedirectStandardError = logStreamWriter != null || consoleErrorCaptureWriter != null;

            // Start the process.
            process = Process.Start(startInfo);

            // Configure the streams.
            ConfigureLogging();
            ConfigureOutputCapture();
            ConfigureErrorCapture();

            if (startInfo.RedirectStandardOutput)
                process.BeginOutputReadLine();
            if (startInfo.RedirectStandardError)
                process.BeginErrorReadLine();
        }

        /// <summary>
        /// Kills the process if it is running.
        /// </summary>
        public void Kill()
        {
            if (process != null && !process.HasExited)
                process.Kill();
        }

        /// <summary>
        /// <para>
        /// Waits for the process to exit.
        /// </para>
        /// <para>
        /// Returns immediately if the process has already exited.
        /// </para>
        /// </summary>
        /// <param name="timeoutMilliseconds">The number of milliseconds to wait, or 
        /// <see cref="Timeout.Infinite" /> to wait indefinitely</param>
        /// <returns>True if the process exited, false if a timeout occurred</returns>
        /// <exception cref="InvalidOperationException">Thrown if the process has not been started</exception>
        public bool WaitForExit(int timeoutMilliseconds)
        {
            return process.WaitForExit(timeoutMilliseconds);
        }

        private void ConfigureOutputCapture()
        {
            if (consoleOutputCaptureWriter != null)
            {
                process.OutputDataReceived += delegate(object sender, DataReceivedEventArgs e)
                {
                    consoleOutputCaptureWriter.WriteLine(e.Data);
                };
            }
        }

        private void ConfigureErrorCapture()
        {
            if (consoleErrorCaptureWriter != null)
            {
                process.ErrorDataReceived += delegate(object sender, DataReceivedEventArgs e)
                {
                    consoleErrorCaptureWriter.WriteLine(e.Data);
                };
            }
        }

        private void ConfigureLogging()
        {
            if (logStreamWriter == null)
                return;

            logStreamWriter.BeginSection(String.Concat("Run Process: " + executablePath, @" ", arguments));

            process.OutputDataReceived += WriteDataReceivedToLog;
            process.ErrorDataReceived += WriteDataReceivedToLog;

            process.EnableRaisingEvents = true;
            process.Exited += EndLogSection;
        }

        private void WriteDataReceivedToLog(object sender, DataReceivedEventArgs e)
        {
            try
            {
                logStreamWriter.WriteLine(e.Data);
            }
            catch
            {
                // Ignore exceptions.  It can happen that the test
                // aborts before the process terminates such that the log
                // stream becomes no longer writable.
            }
        }

        private void EndLogSection(object sender, EventArgs e)
        {
            try
            {
                if (process != null)
                    logStreamWriter.WriteLine("Exit Code: {0}", process.ExitCode);

                logStreamWriter.EndSection();
            }
            catch
            {
                // Ignore exceptions.  It can happen that the test
                // aborts before the process terminates such that the
                // stream becomes no longer writable.
            }
        }
    }
}
