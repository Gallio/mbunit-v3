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
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using Gallio.Common;
using Gallio.Common.Concurrency;

namespace Gallio.AutoCAD.Commands
{
    /// <summary>
    /// Runs commands in a remote AutoCAD process via COM interop.
    /// </summary>
    public class AcadCommandRunner : IAcadCommandRunner
    {
        private static readonly TimeSpan ReadyTimeout = TimeSpan.FromSeconds(60);
        private static readonly TimeSpan ReadyPollInterval = TimeSpan.FromSeconds(0.25);

        /// <inheritdoc/>
        public IAsyncResult BeginRun(AcadCommand command, IProcess process, AsyncCallback completionCallback, object asyncState)
        {
            if (command == null)
                throw new ArgumentNullException("command");
            if (process == null)
                throw new ArgumentNullException("process");
            if (process.HasExited)
                throw new ArgumentNullException("process", "Process has exited.");

            var task = new CommandTask(command, process, completionCallback, asyncState);
            return task.Begin();
        }

        /// <inheritdoc/>
        public void EndRun(IAsyncResult result)
        {
            if (result == null)
                throw new ArgumentNullException("result");

            var taskResult = result as CommandTaskResult;
            if (taskResult == null)
                throw new ArgumentException("Unknown result type.", "result");

            taskResult.Get();
        }

        private class CommandTask
        {
            private readonly AcadCommand command;
            private readonly IProcess process;
            private readonly CommandTaskResult result;

            public CommandTask(AcadCommand command, IProcess process, AsyncCallback completionCallback, object asyncState)
            {
                this.command = command;
                this.process = process;
                this.result = new CommandTaskResult(completionCallback, asyncState);
            }

            public CommandTaskResult Begin()
            {
                CreateSTAThread(Run).Start();
                return result;
            }

            private void Run()
            {
                object application = GetAcadApplication(process);
                using (MessageFilter.Register(Stopwatch.StartNew()))
                {
                    try
                    {
                        SendCommand(application, command);

                        result.Complete(null);
                    }
                    catch (Exception e)
                    {
                        result.Complete(e);
                    }
                }
            }

            private static ThreadTask CreateSTAThread(Action start)
            {
                var task = new ThreadTask("AutoCAD Command Runner", start)
                               {
                                   ApartmentState = ApartmentState.STA
                               };
                return task;
            }

            private static object GetAcadApplication(IProcess process)
            {
                Stopwatch stopwatch = Stopwatch.StartNew();
                process.WaitForInputIdle(ReadyTimeout.Milliseconds);
                while (stopwatch.Elapsed < ReadyTimeout)
                {
                    try
                    {
                        var application = Marshal.GetActiveObject("AutoCAD.Application");
                        if (application != null)
                            return application;
                    }
                    catch (COMException)
                    {
                    }

                    Thread.Sleep(ReadyPollInterval);
                }

                throw new TimeoutException("Unable to acquire the AutoCAD automation object from the running object table.");
            }

            private static void SendCommand(object application, AcadCommand command)
            {
                try
                {
                    var document = GetActiveDocument(application);
                    var parameters = new object[] { command.ToLispExpression(application) };
                    document.GetType().InvokeMember("SendCommand", BindingFlags.InvokeMethod, null, document, parameters);
                    GC.KeepAlive(application);
                }
                catch (COMException e)
                {
                    throw new TimeoutException("Unable to send messages to the AutoCAD process.", e);
                }
            }

            private static object GetActiveDocument(object application)
            {
                var document = application.GetType().InvokeMember("ActiveDocument", BindingFlags.GetProperty, null, application, null);
                if (document == null)
                    throw new InvalidOperationException("Unable to acquire the active document from AutoCAD.");
                return document;
            }
        }

        private class CommandTaskResult : IAsyncResult
        {
            private readonly object syncLock = new object();
            private readonly object asyncState;
            private readonly AsyncCallback completionCallback;
            private ManualResetEvent waitHandle;
            private bool isCompleted;
            private Exception exception;

            public CommandTaskResult(AsyncCallback completionCallback, object asyncState)
            {
                this.completionCallback = completionCallback;
                this.asyncState = asyncState;
            }

            internal void Complete(Exception exception)
            {
                lock (syncLock)
                {
                    this.exception = exception;
                    isCompleted = true;
                    if (waitHandle != null)
                        waitHandle.Set();
                }

                if (completionCallback != null)
                    completionCallback(this);
            }

            internal void Get()
            {
                if (IsCompleted == false)
                {
                    try
                    {
                        AsyncWaitHandle.WaitOne();
                    }
                    finally
                    {
                        AsyncWaitHandle.Close();
                    }
                }

                if (exception != null)
                    throw exception;
            }

            public bool IsCompleted
            {
                get { return isCompleted; }
            }

            public WaitHandle AsyncWaitHandle
            {
                get
                {
                    lock (syncLock)
                    {
                        if (waitHandle == null)
                            waitHandle = new ManualResetEvent(isCompleted);
                    }
                    return waitHandle;
                }
            }

            public object AsyncState
            {
                get { return asyncState; }
            }

            public bool CompletedSynchronously
            {
                get { return false; }
            }
        }

        [ComImport]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        [Guid("00000016-0000-0000-C000-000000000046")]
        private interface IOleMessageFilter
        {
            [PreserveSig]
            int HandleInComingCall(
                [In] uint dwCallType,
                [In] IntPtr htaskCaller,
                [In] uint dwTickCount,
                [In] IntPtr lpInterfaceInfo);

            [PreserveSig]
            int RetryRejectedCall(
                [In] IntPtr htaskCallee,
                [In] uint dwTickCount,
                [In] uint dwRejectType);

            int MessagePending(
                [In] IntPtr htaskCallee,
                [In] uint dwTickCount,
                [In] uint dwPendingType);
        }

        private class MessageFilter : IOleMessageFilter, IDisposable
        {
            private readonly Stopwatch elapsed;
            private IOleMessageFilter oldFilter;

            private MessageFilter(Stopwatch elapsed)
            {
                this.elapsed = elapsed;
            }

            internal static IDisposable Register(Stopwatch elapsed)
            {
                var filter = new MessageFilter(elapsed);
                CoRegisterMessageFilter(filter, out filter.oldFilter);
                return filter;
            }

            public void Dispose()
            {
                IOleMessageFilter dummy;
                CoRegisterMessageFilter(oldFilter, out dummy);
            }

            int IOleMessageFilter.HandleInComingCall(uint dwCallType, IntPtr htaskCaller, uint dwTickCount, IntPtr lpInterfaceInfo)
            {
                if (oldFilter == null)
                    return 0; // SERVERCALL_ISHANDLED

                return oldFilter.HandleInComingCall(dwCallType, htaskCaller, dwTickCount, lpInterfaceInfo);
            }

            int IOleMessageFilter.RetryRejectedCall(IntPtr htaskCallee, uint dwTickCount, uint dwRejectType)
            {
                if (dwRejectType == 2) // SERVERCALL_RETRYLATER
                    return 100; // retry in 100 ms

                if (dwRejectType == 1)
                {
                    if (elapsed.Elapsed > ReadyTimeout)
                        return -1;

                    return 100;
                }

                return -1;
            }

            int IOleMessageFilter.MessagePending(IntPtr htaskCallee, uint dwTickCount, uint dwPendingType)
            {
                if (oldFilter == null)
                    return 1; // PENDINGMSG_WAITNOPROCESS

                return oldFilter.MessagePending(htaskCallee, dwTickCount, dwPendingType);
            }

            [DllImport("ole32.dll")]
            private static extern int CoRegisterMessageFilter(IOleMessageFilter lpMessageFilter, out IOleMessageFilter lplpMessageFilter);
        }
    }
}
