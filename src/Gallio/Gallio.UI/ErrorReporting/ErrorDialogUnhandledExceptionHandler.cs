using System;
using System.Windows.Forms;
using Gallio.Common.Concurrency;
using Gallio.Common.Policies;

namespace Gallio.UI.ErrorReporting
{
    /// <summary>
    /// Installs an unhandled exception handler that displays an error dialog.
    /// </summary>
    public static class ErrorDialogUnhandledExceptionHandler
    {
        private static readonly object syncRoot = new object();
        private static Form installedOwner;

        /// <summary>
        /// Installs the handler.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The handler is automatically removed when the form is disposed.
        /// </para>
        /// </remarks>
        /// <param name="owner">The owner window.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="owner"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown if there is already a handler installed for a different owner.</exception>
        public static void Install(Form owner)
        {
            if (owner == null)
                throw new ArgumentNullException("owner");

            lock (syncRoot)
            {
                if (installedOwner != null)
                {
                    if (installedOwner != owner)
                        throw new InvalidOperationException("There is already an error dialog unhandled exception handler installed for a different owner window.");
                    return;
                }

                installedOwner = owner;
            }

            UnhandledExceptionPolicy.ReportUnhandledException += ReportUnhandledException;
        }

        /// <summary>
        /// Uninstalls the handler.
        /// </summary>
        public static void Uninstall()
        {
            UnhandledExceptionPolicy.ReportUnhandledException -= ReportUnhandledException;
        }

        /// <summary>
        /// Runs an application 
        /// </summary>
        /// <param name="mainForm">The main form.</param>
        public static void RunApplicationWithHandler(Form mainForm)
        {
            using (mainForm)
            {
                try
                {
                    Install(mainForm);

                    Application.Run(mainForm);
                }
                finally
                {
                    Uninstall();
                }
            }
        }

        private static void ReportUnhandledException(object sender, CorrelatedExceptionEventArgs e)
        {
            Form currentOwner;
            lock (syncRoot)
            {
                if (installedOwner == null || installedOwner.IsDisposed)
                {
                    Uninstall();
                    return;
                }

                currentOwner = installedOwner;
            }

            Sync.Invoke(currentOwner, () => ErrorDialog.Show(currentOwner, "Unhandled Exception", e.Message, e.GetDetails()));
        }
    }
}
