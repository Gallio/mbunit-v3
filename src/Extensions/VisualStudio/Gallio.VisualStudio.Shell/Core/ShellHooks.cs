using System;
using EnvDTE;

namespace Gallio.VisualStudio.Shell.Core
{
    /// <summary>
    /// Provides a mechanism for Shell components to register to receive certain
    /// top-level events from Visual Studio.
    /// </summary>
    public class ShellHooks
    {
        /// <summary>
        /// Hook type for <see cref="IDTCommandTarget.QueryStatus" />.
        /// </summary>
        public delegate void QueryStatusHook(string commandName, vsCommandStatusTextWanted neededText, ref vsCommandStatus statusOption, ref object commandText);

        /// <summary>
        /// Hook type for <see cref="IDTCommandTarget.Exec" />.
        /// </summary>
        public delegate void ExecHook(string commandName, vsCommandExecOption executeOption, ref object variantIn, ref object variantOut, ref bool handled);

        /// <summary>
        /// Hook event for <see cref="IDTCommandTarget.QueryStatus" />.
        /// </summary>
        public event QueryStatusHook QueryStatus;

        /// <summary>
        /// Hook event for <see cref="IDTCommandTarget.Exec" />.
        /// </summary>
        public event ExecHook Exec;

        internal void HandleQueryStatus(string commandName, vsCommandStatusTextWanted neededText, ref vsCommandStatus statusOption, ref object commandText)
        {
            if (QueryStatus != null)
                QueryStatus(commandName, neededText, ref statusOption, ref commandText);
        }

        internal void HandleExec(string commandName, vsCommandExecOption executeOption, ref object variantIn, ref object variantOut, ref bool handled)
        {
            if (Exec != null)
                Exec(commandName, executeOption, ref variantIn, ref variantOut, ref handled);
        }
    }
}
