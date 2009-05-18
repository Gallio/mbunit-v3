using System;
using System.Collections.Generic;
using System.Text;
using Gallio.Runtime.ConsoleSupport;
using Gallio.Runtime.Extensibility;
using Gallio.Runtime.Logging;

namespace Gallio.Runtime.UtilityCommands
{
    /// <summary>
    /// A utility command is a small command-line tool that can be invoked
    /// via the Gallio.Utility.exe program.
    /// </summary>
    /// <remarks>
    /// <para>
    /// An example utility command is the VerifyInstallation command which checks
    /// that the plugin configuration appears to be consistent.
    /// </para>
    /// <para>
    /// The utility program provides a minor safety net in case the command throws
    /// an exception.  Upon catching the exception the program will log it as a fatal
    /// error and terminate with a failure exit code.  Of course, when a command throws
    /// an exception the problem should be interpreted as a programming error.
    /// </para>
    /// </remarks>
    [Traits(typeof(UtilityCommandTraits))]
    public interface IUtilityCommand
    {
        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="context">The command execution context, not null</param>
        /// <returns>The command exit code, zero for success, non-zero for errors</returns>
        int Execute(UtilityCommandContext context);

        /// <summary>
        /// Gets the type of a class used to provide arguments to the command.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The returns type should be decorated with attributes such as
        /// <see cref="DefaultCommandLineArgumentAttribute"/> and
        /// <see cref="CommandLineArgumentAttribute"/>.
        /// </para>
        /// </remarks>
        /// <returns>The argument class</returns>
        Type GetArgumentClass();

        /// <summary>
        /// Validates the command arguments.
        /// </summary>
        /// <param name="arguments">The arguments object, not null</param>
        /// <param name="errorReporter">The error reporter, not null</param>
        /// <returns>True if the arguments are valid</returns>
        bool ValidateArguments(object arguments, CommandLineErrorReporter errorReporter);
    }
}
