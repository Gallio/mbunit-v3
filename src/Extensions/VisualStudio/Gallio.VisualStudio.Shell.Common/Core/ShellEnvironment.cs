using System;
using System.Diagnostics;

namespace Gallio.VisualStudio.Shell.Core
{
    /// <summary>
    /// Provides information about the context in which the Shell is running.
    /// </summary>
    public static class ShellEnvironment
    {
        /// <summary>
        /// Gets whether the Shell is running in Visual Studio.
        /// </summary>
        /// <remarks>
        /// This property can be used to determine whether Shell components are running
        /// inside of Visual Studio or if they have been invoked from some other context.
        /// For example, the Gallio Test Integration Provider can be called by MSTest
        /// or Visual Studio.  When called by MSTest, the Shell package and add-in are not
        /// initialized so neither are the shell extensions and other standard features.
        /// </remarks>
        public static bool IsRunningInVisualStudio
        {
            get
            {
                return Process.GetCurrentProcess().ProcessName
                    .Equals("devenv", StringComparison.InvariantCultureIgnoreCase);
            }
        }
    }
}
