using System;
using System.Collections.Generic;
using System.Text;

namespace MbUnit.Core.Runner
{
    /// <summary>
    /// Encapsulates concerns relating to handling console-based
    /// cancelation events.
    /// </summary>
    /// <todo author="jeff">
    /// Setup a rule so that repeated attempts to cancel the application
    /// with Control+C within a short time-frame simply are eventually
    /// answered by allowing the framework to terminate the application.
    /// eg. 3 occurrences of Control+C within 0.5 secs.
    /// </todo>
    public static class ConsoleCancelHandler
    {
        private static readonly object syncRoot = new object();

        private static bool isCanceled;
        private static event EventHandler cancelHandlers;

        /// <summary>
        /// Installs the handler.
        /// </summary>
        public static void Install()
        {
            Console.TreatControlCAsInput = false;
            Console.CancelKeyPress += HandleCancelKeyPress;
        }

        /// <summary>
        /// Raised when console cancelation occurs.
        /// If the console cancelation signal is already set when
        /// an event handler is added, the event handler will be
        /// automatically invoked.
        /// </summary>
        public static event EventHandler Cancel
        {
            add
            {
                lock (syncRoot)
                {
                    cancelHandlers += value;

                    if (!isCanceled)
                        return;
                }

                value(null, EventArgs.Empty);
            }
            remove
            {
                lock (syncRoot)
                {
                    cancelHandlers -= value;
                }
            }
        }

        /// <summary>
        /// Gets or sets whether cancelation has occurred.
        /// The <see cref="Cancel" /> event handlers will be called
        /// when the value of <see cref="IsCanceled" /> transitions from
        /// false to true.  The value will remain true unless it is reset.
        /// </summary>
        public static bool IsCanceled
        {
            get
            {
                lock (syncRoot)
                    return isCanceled;
            }
            set
            {
                EventHandler currentCancelHandlers;

                lock (syncRoot)
                {
                    if (isCanceled == value)
                        return;

                    isCanceled = value;
                    if (!isCanceled)
                        return;

                    currentCancelHandlers = cancelHandlers;
                }

                if (currentCancelHandlers != null)
                    currentCancelHandlers(null, EventArgs.Empty);
            }
        }

        private static void HandleCancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            e.Cancel = true;
            IsCanceled = true;
        }
    }
}
