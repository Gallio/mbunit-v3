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
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using Gallio.AutoCAD.Commands;
using Gallio.AutoCAD.Native;
using Gallio.Common.Concurrency;
using Gallio.Common.Text;
using Gallio.Runner;
using Gallio.Model;

namespace Gallio.AutoCAD
{
    /// <summary>
    /// Provides support for managing the AutoCAD process.
    /// </summary>
    public class AcadProcess : IAcadProcess
    {
        private static readonly TimeSpan ReadyTimeout = TimeSpan.FromSeconds(60);
        private static readonly TimeSpan ReadyPollInterval = TimeSpan.FromSeconds(0.5);

        private bool shutdownAcadOnDispose;
        private AcadProcessTask processTask;

        private AcadProcess(AcadProcessTask processTask)
        {
            if (processTask == null)
                throw new ArgumentNullException("processTask");

            this.processTask = processTask;

            shutdownAcadOnDispose = !processTask.IsAttached;
        }

        /// <summary>
        /// Creates a new <see cref="AcadProcess"/> instance
        /// by attaching to an existing <c>acad.exe</c> instance.
        /// </summary>
        public static AcadProcess Attach(Process existingProcess)
        {
            if (existingProcess == null)
                throw new ArgumentNullException("existingProcess");

            return new AcadProcess(AcadProcessTask.Attach(existingProcess));
        }

        /// <summary>
        /// Creates a new <see cref="AcadProcess"/> instance
        /// by creating a new <c>acad.exe</c> process.
        /// </summary>
        /// <param name="executablePath">The path to <c>acad.exe</c>.</param>
        public static AcadProcess Create(string executablePath)
        {
            if (executablePath == null)
                throw new ArgumentNullException("executablePath");

            return new AcadProcess(AcadProcessTask.Create(executablePath));
        }

        /// <inheritdoc />
        public void Start(string ipcPortName)
        {
            if (ipcPortName == null)
                throw new ArgumentNullException("ipcPortName");

            processTask.Start();

            // Load the AutoCAD plugin.
            string pluginFileName = AcadPluginLocator.GetAcadPluginLocation();
            bool pluginIsLoaded = false;
            for (var stopwatch = Stopwatch.StartNew(); !pluginIsLoaded && stopwatch.Elapsed < ReadyTimeout; )
            {
                SendCommand(new NetLoadCommand()
                {
                    AssemblyPath = pluginFileName
                });
                Thread.Sleep(200);
                processTask.Process.Refresh();
                pluginIsLoaded = ModuleIsLoaded(processTask.Process, pluginFileName);
            }
            if (!pluginIsLoaded)
            {
                throw new TimeoutException("Unable to load AutoCAD plugin.");
            }

            // Create the remote endpoint. This will block on acad.exe's UI thread.
            //
            // NOTE: AutoCAD will not pump messages until after
            //       Shutdown() is called on the remote service.
            //
            SendCommand(new CreateEndpointAndWaitCommand()
            {
                IpcPortName = ipcPortName,
                SendAsynchronously = true
            });
        }

        private static bool ModuleIsLoaded(Process process, string fileName)
        {
            foreach (ProcessModule module in process.Modules)
            {
                if (module.FileName.Equals(fileName, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Frees any resources unknown to the GC.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If the <see cref="AcadProcess"/> created a new <c>acad.exe</c> instance
        /// the finalizer tries to shut it down.
        /// </para>
        /// </remarks>
        ~AcadProcess()
        {
            Dispose(false);
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Frees any resources owned by the <see cref="AcadProcess"/>.
        /// </summary>
        /// <param name="disposing"><c>false</c> if called from the finalizer; otherwise, <c>true</c>.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (processTask == null)
                return;

            if (disposing)
            {
            }

            // Try to kill the acad.exe process, even if Dispose(bool)
            // is being called from the finalizer.
            if (shutdownAcadOnDispose && processTask != null)
            {
                var task = Interlocked.Exchange(ref processTask, null);
                if (task != null && task.IsRunning)
                    task.Abort(); // TODO: be polite about shutting down the acad.exe process. :)
            }
        }

        private bool IsRunning
        {
            get
            {
                if (processTask == null)
                    return false;
                return processTask.IsRunning;
            }
        }

        private void SendCommand(AcadCommand command)
        {
            if (!IsRunning)
                throw new InvalidOperationException("The process is not running.");

            Process actualProcess = processTask.Process;

            WaitForMessagePump(actualProcess, ReadyTimeout, ReadyPollInterval);
            string lispExpr = CreateLispExpression(command);

            if (command.SendAsynchronously)
            {
                new Thread(() => SendCopyDataMessage(new HandleRef(null, actualProcess.MainWindowHandle), lispExpr)).Start();
            }
            else
            {
                SendCopyDataMessage(new HandleRef(null, actualProcess.MainWindowHandle), lispExpr);
            }
        }

        private static string CreateLispExpression(AcadCommand command)
        {
            string globalName = String.Concat("_", command.GlobalName);

            var builder = new StringBuilder();
            builder.Append(String.Concat("(command ", StringUtils.ToStringLiteral(globalName)));
            foreach (string arg in command.Arguments)
            {
                builder.Append(String.Concat(" ", StringUtils.ToStringLiteral(arg ?? String.Empty)));
            }
            builder.Append(")\n");

            return builder.ToString();
        }

        private static void SendCopyDataMessage(HandleRef hwnd, string message)
        {
            var cds = new COPYDATASTRUCT(message);
            NativeMethods.SendMessage(hwnd, ref cds);
            GC.KeepAlive(cds);
        }

        private static void WaitForMessagePump(Process process, TimeSpan timeout, TimeSpan pollInterval)
        {
            var stopwatch = Stopwatch.StartNew();

            // Poll the process until it creates a "main" window. Using MainWindowHandle
            // may become problematic if acad.exe created multiple top-level unowned windows.
            // See http://blogs.msdn.com/oldnewthing/archive/2008/02/20/7806911.aspx for discussion.
            while (process.MainWindowHandle == IntPtr.Zero)
            {
                if (stopwatch.Elapsed > timeout)
                    throw new ModelException("Timeout waiting for AutoCAD to create message pump.");

                Thread.Sleep(pollInterval);
                process.Refresh();
            }

            var remaining = timeout - stopwatch.Elapsed;
            if (remaining <= TimeSpan.Zero || !process.WaitForInputIdle((int)remaining.TotalMilliseconds))
                throw new ModelException("Timeout waiting for AutoCAD to enter an idle state.");
        }

        private class AcadProcessTask : ProcessTask
        {
            private readonly Process attachedProcess;

            private AcadProcessTask(string executablePath, string arguments, string workingDirectory, Process existing)
                : base(executablePath, arguments, workingDirectory)
            {
                attachedProcess = existing;
            }

            public static AcadProcessTask Create(string executablePath)
            {
                return new AcadProcessTask(executablePath, String.Empty, Path.GetDirectoryName(executablePath), null);
            }

            public static AcadProcessTask Attach(Process existingProcess)
            {
                string executablePath = Path.GetFullPath(existingProcess.MainModule.FileName);
                string arguments = existingProcess.StartInfo.Arguments;
                string workingDirectory = existingProcess.StartInfo.WorkingDirectory;
                return new AcadProcessTask(executablePath, arguments, workingDirectory, existingProcess);
            }

            /// <inheritdoc/>
            protected override Process StartProcess(ProcessStartInfo startInfo)
            {
                if (attachedProcess != null)
                    return attachedProcess;
                return base.StartProcess(startInfo);
            }

            /// <summary>
            /// Gets whether the <see cref="AcadProcessTask"/> was created
            /// by attaching to an existing AutoCAD process.
            /// </summary>
            public bool IsAttached
            {
                get { return attachedProcess != null; }
            }
        }
    }
}
