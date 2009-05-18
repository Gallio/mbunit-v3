using System;
using System.Collections.Generic;
using System.Text;
using Gallio.Runtime.ConsoleSupport;

namespace Gallio.Runtime.UtilityCommands
{
    /// <summary>
    /// Abstract base class for a utility command.
    /// </summary>
    /// <typeparam name="TArguments">The arguments type, may be <see cref="object" />
    /// for commands with no arguments</typeparam>
    public abstract class BaseUtilityCommand<TArguments> : IUtilityCommand
    {
        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="context">The command execution context, not null</param>
        /// <param name="arguments">The typed command arguments, not null</param>
        /// <returns>The command exit code, zero for success, non-zero for errors</returns>
        public abstract int Execute(UtilityCommandContext context, TArguments arguments);

        /// <summary>
        /// Validates the command arguments.
        /// </summary>
        /// <param name="arguments">The arguments object, not null</param>
        /// <param name="errorReporter">The error reporter, not null</param>
        /// <returns>True if the arguments are valid</returns>
        public virtual bool ValidateArguments(TArguments arguments, CommandLineErrorReporter errorReporter)
        {
            return true;
        }

        int IUtilityCommand.Execute(UtilityCommandContext context)
        {
            return Execute(context, (TArguments)context.Arguments);
        }

        Type IUtilityCommand.GetArgumentClass()
        {
            return typeof(TArguments);
        }

        bool IUtilityCommand.ValidateArguments(object arguments, CommandLineErrorReporter errorReporter)
        {
            return ValidateArguments((TArguments)arguments, errorReporter);
        }
    }
}
