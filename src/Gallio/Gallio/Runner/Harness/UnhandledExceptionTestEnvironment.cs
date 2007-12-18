using System;
using Gallio.Contexts;
using Gallio.Logging;

namespace Gallio.Runner.Harness
{
    /// <summary>
    /// Logs unhandled exceptions instead of killing the AppDomain.
    /// </summary>
    public class UnhandledExceptionTestEnvironment : ITestEnvironment
    {
        /// <inheritdoc />
        public IDisposable SetUp()
        {
            return new State();
        }

        private sealed class State : IDisposable
        {
            public State()
            {
                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            }

            public void Dispose()
            {
                AppDomain.CurrentDomain.UnhandledException -= CurrentDomain_UnhandledException;
            }

            void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
            {
                try
                {
                    Exception ex = e.ExceptionObject as Exception;
                    if (ex != null)
                        Context.CurrentContext.LogWriter[LogStreamNames.Warnings].WriteException(ex, "An unhandled exception occurred.");
                }
                catch
                {
                    // Ignore any exceptions we encounter logging the exception since there isn't
                    // much we can do anyways.
                }
            }
        }
    }
}