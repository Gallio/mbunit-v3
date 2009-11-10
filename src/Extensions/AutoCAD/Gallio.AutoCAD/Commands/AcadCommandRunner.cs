using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
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
        public void Run(AcadCommand command, IProcess process)
        {
            var application = GetAcadApplication(process);
            SendCommand(application, command);
        }

        private static object GetAcadApplication(IProcess process)
        {
            var stopwatch = Stopwatch.StartNew();
            process.WaitForInputIdle(ReadyTimeout.Milliseconds);

            while (stopwatch.Elapsed < ReadyTimeout)
            {
                try
                {
                    var application = Marshal.GetActiveObject("AutoCAD.Application");
                    if (application != null)
                        return application;
                }
                catch (COMException e)
                {
                    if (e.ErrorCode != unchecked((int)0x800401E3) /* MK_E_UNAVAILABLE */)
                        throw;
                }

                Thread.Sleep(ReadyPollInterval);
            }

            throw new TimeoutException("Unable to acquire the AutoCAD automation object from the running object table.");
        }

        private static void SendCommand(object application, AcadCommand command)
        {
            var stopwatch = Stopwatch.StartNew();
            while (stopwatch.Elapsed < ReadyTimeout)
            {
                try
                {
                    var document = GetActiveDocument(application);
                    var parameters = new object[] { command.ToLispExpression() };
                    document.GetType().InvokeMember("SendCommand", BindingFlags.InvokeMethod, null, document, parameters);
                    return;
                }
                catch (COMException e)
                {
                    if (e.ErrorCode != unchecked((int)0x8001010A) /* RPC_E_SERVERCALL_RETRYLATER */)
                        throw;
                }

                Thread.Sleep(ReadyPollInterval);
                
            }

            throw new TimeoutException("Unable to send messages to the AutoCAD process.");
        }

        private static object GetActiveDocument(object application)
        {
            var document = application.GetType().InvokeMember("ActiveDocument", BindingFlags.GetProperty, null, application, null);
            if (document == null)
                throw new InvalidOperationException("Unable to acquire the active document from AutoCAD.");
            return document;
        }
    }
}