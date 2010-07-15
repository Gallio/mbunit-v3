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
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using Gallio.Common.Policies;
using Gallio.Common.Platform;

namespace Gallio.Common.Concurrency
{
    /// <summary>
    /// A process task provides support for launching external processes
    /// and collecting their output.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The process task provides a guarantee that when you call <see cref="Task.Join" />
    /// all redirected output from the console output and error streams will already
    /// have been captured and delivered to the event handlers, as appropriate.
    /// </para>
    /// </remarks>
    public class ProcessTask : Task
    {
        private readonly string executablePath;
        private readonly string arguments;
        private readonly string workingDirectory;
        private Dictionary<string, string> environmentVariables;

        private bool captureConsoleOutput;
        private bool captureConsoleError;
        private bool useShellExecute;
        private bool createWindow;

        private StringWriter consoleOutputCaptureWriter;
        private StringWriter consoleErrorCaptureWriter;

        private Process process;
        private int exited;
        private bool loggingStarted;
        
        private ManualResetEvent exitedEvent;

        /// <summary>
        /// Creates a process task.
        /// </summary>
        /// <param name="executablePath">The path of the executable executable.</param>
        /// <param name="arguments">The arguments for the executable.</param>
        /// <param name="workingDirectory">The working directory.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="executablePath"/>,
        /// <paramref name="arguments"/> or <paramref name="workingDirectory"/> is null.</exception>
        public ProcessTask(string executablePath, string arguments, string workingDirectory)
            : base(executablePath + @" " + arguments)
        {
            if (executablePath == null)
                throw new ArgumentNullException(@"executablePath");
            if (arguments == null)
                throw new ArgumentNullException(@"arguments");
            if (workingDirectory == null)
                throw new ArgumentNullException("workingDirectory");

            this.executablePath = Path.GetFullPath(executablePath);
            this.arguments = arguments;
            this.workingDirectory = workingDirectory;
            
            exitedEvent = new ManualResetEvent(false);
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
        /// Gets the working directory path.
        /// </summary>
        public string WorkingDirectory
        {
            get { return workingDirectory; }
        }

        /// <summary>
        /// Gets the <see cref="Process" /> that was started or null if the
        /// process has not been started yet.
        /// </summary>
        /// <remarks>
        /// Please be aware that <see cref="Process" /> is not thread-safe.
        /// You must lock the process object before accessing it.
        /// </remarks>
        public Process Process
        {
            get { return process; }
        }

        /// <summary>
        /// Gets or sets whether console output stream of the process should be captured
        /// and made available via the <see cref="ConsoleOutput" /> property.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The default value is <c>false</c>.
        /// </para>
        /// </remarks>
        public bool CaptureConsoleOutput
        {
            get { return captureConsoleOutput; }
            set { captureConsoleOutput = value; }
        }

        /// <summary>
        /// Gets or sets whether console error stream of the process should be captured
        /// and made available via the <see cref="ConsoleError" /> property.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The default value is <c>false</c>.
        /// </para>
        /// </remarks>
        public bool CaptureConsoleError
        {
            get { return captureConsoleError; }
            set { captureConsoleError = value; }
        }

        /// <summary>
        /// Gets or sets whether to execute the command with the Windows shell.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The default value is <c>false</c>.
        /// </para>
        /// </remarks>
        public bool UseShellExecute
        {
            get { return useShellExecute; }
            set { useShellExecute = value; }
        }

        /// <summary>
        /// Gets or sets whether to create a window for the command prompt.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The default value is <c>false</c>.
        /// </para>
        /// </remarks>
        public bool CreateWindow
        {
            get { return createWindow; }
            set { createWindow = value; }
        }

        /// <summary>
        /// Gets the captured contents of the console output stream written by the process.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if the process has not been started
        /// or if <see cref="CaptureConsoleOutput" /> is <c>null</c>.</exception>
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
        /// or if <see cref="CaptureConsoleError" /> is <c>null</c>.</exception>
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
        /// Gets the exit code of the process, or -1 if the process did not run or has not exited.
        /// </summary>
        public int ExitCode
        {
            get
            {
                if (process == null)
                    return -1;
                
                lock (process)
                {
                    return process.HasExited ? process.ExitCode : -1;
                }
            }
        }

        /// <summary>
        /// Gets a description of the exit code, if available, or null otherwise.
        /// </summary>
        public string ExitCodeDescription
        {
            get
            {
                if (!DotNetRuntimeSupport.IsUsingMono)
                {
                    int exitCode = ExitCode;
                    if (exitCode < -1)
                    {
                        Exception ex = Marshal.GetExceptionForHR(exitCode);
                        if (ex != null && !ex.Message.Contains("HRESULT"))
                            return ex.Message;
                    }
                }

                return null;
            }
        }

        /// <summary>
        /// The event fired to configure <see cref="ProcessStartInfo" /> just before the process is started.
        /// </summary>
        public event EventHandler<ConfigureProcessStartInfoEventArgs> ConfigureProcessStartInfo;

        /// <summary>
        /// The event fired when each line of new output is received on the console output stream.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This event should be wired up before starting the task to ensure that
        /// no data is lost.
        /// </para>
        /// <para>
        /// Please note that the <see cref="DataReceivedEventArgs.Data" /> property
        /// will be null when the redirected stream is closed after the process exits.
        /// </para>
        /// </remarks>
        public event EventHandler<DataReceivedEventArgs> ConsoleOutputDataReceived;

        /// <summary>
        /// The event fired when each line of new output is received on the console error stream.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This event should be wired up before starting the task to ensure that
        /// no data is lost.
        /// </para>
        /// <para>
        /// Please note that the <see cref="DataReceivedEventArgs.Data" /> property
        /// will be null when the redirected stream is closed after the process exits.
        /// </para>
        /// </remarks>
        public event EventHandler<DataReceivedEventArgs> ConsoleErrorDataReceived;

        /// <summary>
        /// Sets an environment variable to be passed to the process when started.
        /// </summary>
        /// <param name="name">The variable name.</param>
        /// <param name="value">The variable value, or null to remove the environment variable
        /// so it will not be inherited by the child process.</param>
        /// <exception cref="InvalidOperationException">Thrown if the process has already been started.</exception>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/> is null.</exception>
        public void SetEnvironmentVariable(string name, string value)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (process != null)
                throw new InvalidOperationException("The process has already been started.");

            if (environmentVariables == null)
                environmentVariables = new Dictionary<string, string>();

            environmentVariables[name] = value;
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

            ProcessStartInfo startInfo = CreateProcessStartInfo();

            startInfo.WorkingDirectory = workingDirectory;
            startInfo.UseShellExecute = useShellExecute;
            startInfo.CreateNoWindow = ! createWindow;

            startInfo.RedirectStandardOutput = captureConsoleOutput || ConsoleOutputDataReceived != null;
            startInfo.RedirectStandardError = captureConsoleError || ConsoleErrorDataReceived != null;

            if (environmentVariables != null)
            {
                foreach (var pair in environmentVariables)
                {
                    if (pair.Value == null)
                        startInfo.EnvironmentVariables.Remove(pair.Key);
                    else
                        startInfo.EnvironmentVariables[pair.Key] = pair.Value;
                }
            }

            if (ConfigureProcessStartInfo != null)
                ConfigureProcessStartInfo(this, new ConfigureProcessStartInfoEventArgs(startInfo));

            process = StartProcess(startInfo);
            lock (process)
            {
                process.EnableRaisingEvents = true;
            }

            StartLogging();
            StartProcessExitDetection();
        }

        /// <summary>
        /// Starts a <see cref="Process" />.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method may be override to change how the process is created and
        /// started.
        /// </para>
        /// </remarks>
        /// <param name="startInfo">The <see cref="ProcessStartInfo" /> that has been started.</param>
        /// <returns>The process.</returns>
        protected virtual Process StartProcess(ProcessStartInfo startInfo)
        {
            return Process.Start(startInfo);
        }

        private void StartProcessExitDetection()
        {
            if (DotNetRuntimeSupport.IsUsingMono)
            {
                // On Mono 1.9.1, the process exited event does not seem to be reliably delivered.
                // So we have to hack around it...
                var thread = new Thread(() =>
                {
                    try
                    {
                        for (;;)
                        {
                            lock (process)
                            {
                                if (process.HasExited)
                                    break;
                            }
                            
                            Thread.Sleep(100);
                        }
                    }
                    catch
                    {
                        // Give up.
                    }
                    finally
                    {
                        HandleProcessExit();
                    }
                });

                thread.IsBackground = true;
                thread.Start();
            }
            else
            {
                // Handle process exit including the case where the process might already
                // have exited just prior to adding the event handler.
                lock (process)
                {
                    process.Exited += delegate { HandleProcessExit(); };
                    if (process.HasExited)
                        HandleProcessExit();
                }
            }
        }

        private void HandleProcessExit()
        {            
            // The process lock may already be held by this thread since the
            // Process object can call its Exited event handler as a side effect of
            // several operations on the Process object.  This could result in a
            // deadlock if the task's Terminated event waits on some other thread that
            // is also blocked on the process.  So we make it asynchronous instead.
            // Join then blocks on the exitedEvent to restore the required synchronization.
            if (Interlocked.Exchange(ref exited, 1) == 0)
            {
                ThreadPool.QueueUserWorkItem((state) =>
                {
                    try
                    {
                        WaitForConsoleToBeCompletelyReadOnceProcessHasExited();
                        NotifyTerminated(TaskResult<object>.CreateFromValue(ExitCode));
                    }
                    finally
                    {
                        exitedEvent.Set();
                    }
                }, null);
            }
        }

        /// <inheritdoc />
        protected override void AbortImpl()
        {
            if (process == null)
                return;

            lock (process)
            {
                if (!process.HasExited)
                    process.Kill();
            }
        }

        /// <inheritdoc />
        protected override bool JoinImpl(TimeSpan? timeout)
        {
            if (process == null)
                return true;

            // There might be multiple threads blocked on Join simultaneously with
            // different timeouts.  Unfortunately they can't all just wait at the
            // same time because the Process object is not threadsafe.  So instead
            // we only lock for short intervals.
            Stopwatch stopwatch = Stopwatch.StartNew();
            for (;;)
            {
                lock (process)
                {
                    int incrementalTimeout = timeout.HasValue && timeout.Value.Ticks >= 0
                        ? Math.Min((int) (timeout.Value - stopwatch.Elapsed).TotalMilliseconds, 100)
                        : 100;
                    if (incrementalTimeout < 0)
                        incrementalTimeout = 0;
                        
                    if (process.WaitForExit(incrementalTimeout))
                        break;
                        
                    if (incrementalTimeout == 0)
                        return false;
                }
            }

            WaitForConsoleToBeCompletelyReadOnceProcessHasExited();
            exitedEvent.WaitOne();
            return true;
        }

        private ProcessStartInfo CreateProcessStartInfo()
        {
            return DotNetRuntimeSupport.CreateReentrantProcessStartInfo(executablePath, arguments);
        }

        private void StartLogging()
        {
            lock (process)
            {
                if (process.StartInfo.RedirectStandardOutput)
                {
                    process.OutputDataReceived += LogOutputData;
                    process.BeginOutputReadLine();
                }

                if (process.StartInfo.RedirectStandardError)
                {
                    process.ErrorDataReceived += LogErrorData;
                    process.BeginErrorReadLine();
                }
            }

            loggingStarted = true;
        }

        private void LogOutputData(object sender, DataReceivedEventArgs e)
        {
            if (captureConsoleOutput && e.Data != null)
                consoleOutputCaptureWriter.WriteLine(e.Data);

            EventHandlerPolicy.SafeInvoke(ConsoleOutputDataReceived, this, e);
        }

        private void LogErrorData(object sender, DataReceivedEventArgs e)
        {
            if (captureConsoleError && e.Data != null)
                consoleErrorCaptureWriter.WriteLine(e.Data);

            EventHandlerPolicy.SafeInvoke(ConsoleErrorDataReceived, this, e);
        }

        private void WaitForConsoleToBeCompletelyReadOnceProcessHasExited()
        {
            if (!loggingStarted)
                return;

            // Wait for all pending I/O to be consumed.  It turns out that calling
            // WaitForExit with no timeout waits for the output and error streams
            // to be full read (when using BeginOutput/ErrorReadLine) but the same
            // courtesy is not extended to WaitForExit with a timeout.  Since the process
            // has already exited, we should only be waiting to read the streams
            // which ought to be pretty quick.
            lock (process)
            {
                process.WaitForExit();
            }
        }
    }
}