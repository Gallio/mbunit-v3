using System;
using System.Runtime.InteropServices;
using System.Threading;
using EnvDTE;
using Gallio.Navigator.Native;
using Constants = Gallio.Navigator.Native.Constants;
using Thread = System.Threading.Thread;

namespace Gallio.Navigator
{
    internal class VisualStudioSupport
    {
        private static readonly string[] VisualStudioDTEProgIds = new[]
        {
            "VisualStudio.DTE.9.0",
            "VisualStudio.DTE.8.0",
            "VisualStudio.DTE"
        };

        /// <summary>
        /// Runs a block of code with the Visual Studio DTE either by finding the currently active instance or by starting one.
        /// </summary>
        /// <param name="action">The action to run</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="action"/> is null</exception>
        public static void WithDTE(Action<DTE> action)
        {
            if (action == null)
                throw new ArgumentNullException("action");

            Exception exception = null;
            var thread = new Thread((ThreadStart) delegate
            {
                IOleMessageFilter newFilter = new MessageFilter();
                IOleMessageFilter oldFilter;
                CoRegisterMessageFilter(newFilter, out oldFilter);
                try
                {
                    DTE dte = (DTE)GetDTEObject();
                    action(dte);
                }
                catch (Exception ex)
                {
                    exception = ex;
                }
                finally
                {
                    CoRegisterMessageFilter(oldFilter, out newFilter);
                }
            });

            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();

            if (exception != null)
                throw new ApplicationException("Could not perform the requested Visual Studio operation.", exception);
        }

        private static object GetDTEObject()
        {
            // If VisualStudio is running, get the active instance.
            foreach (string progId in VisualStudioDTEProgIds)
            {
                object obj = GetActiveObject(progId);
                if (obj != null)
                    return obj;
            }

            // Otherwise try to start a fresh instance.
            foreach (string progId in VisualStudioDTEProgIds)
            {
                Type dteType = Type.GetTypeFromProgID(progId);
                if (dteType != null)
                    return Activator.CreateInstance(dteType, true);
            }

            throw new ApplicationException("Could not find or start an instance of Visual Studio.");
        }

        private static object GetActiveObject(string progId)
        {
            try
            {
                return Marshal.GetActiveObject(progId);
            }
            catch (COMException)
            {
                return null;
            }
        }

        // Implement the IOleMessageFilter interface.
        [DllImport("Ole32.dll")]
        private static extern int CoRegisterMessageFilter(IOleMessageFilter newFilter, out IOleMessageFilter oldFilter);

        // This fancy bit of logic is from an MSDN article:
        // Fixing 'Application is Busy' and 'Call was Rejected By Callee' Errors
        // http://msdn.microsoft.com/en-us/library/ms228772(VS.80).aspx
        private sealed class MessageFilter : IOleMessageFilter
        {
            private const int MaxRetryMilliseconds = 30000;

            // IOleMessageFilter functions.
            // Handle incoming thread requests.
            int IOleMessageFilter.HandleInComingCall(int dwCallType, IntPtr hTaskCaller, int dwTickCount, IntPtr lpInterfaceInfo)
            {
                return Constants.SERVERCALL_ISHANDLED;
            }

            // Thread call was rejected, so try again.
            int IOleMessageFilter.RetryRejectedCall(IntPtr hTaskCallee, int dwTickCount, SERVERCALL dwRejectType)
            {
                if (dwTickCount < MaxRetryMilliseconds
                    && dwRejectType == SERVERCALL.SERVERCALL_RETRYLATER)
                {
                    // Retry the thread call after 100ms.
                    return 100;
                }

                // Too busy; cancel call.
                return -1;
            }

            int IOleMessageFilter.MessagePending(IntPtr hTaskCallee, int dwTickCount, int dwPendingType)
            {
                return Constants.PENDINGMSG_WAITDEFPROCESS;
            }
        }
    }
}
