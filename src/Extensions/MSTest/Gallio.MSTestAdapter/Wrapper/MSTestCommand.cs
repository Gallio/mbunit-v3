using System;
using System.IO;

namespace Gallio.MSTestAdapter.Wrapper
{
    /// <summary>
    /// Runs an MSTest command.
    /// </summary>
    internal abstract class MSTestCommand
    {
        /// <summary>
        /// Runs MSTest with the specified arguments.
        /// </summary>
        /// <param name="executablePath">The path of MSTest.exe</param>
        /// <param name="workingDirectory">The current working directory to use</param>
        /// <param name="args">The command-line arguments</param>
        /// <param name="writer">The text writer to which the output and error streams should be directed</param>
        /// <returns>The exit code</returns>
        public abstract int Run(string executablePath, string workingDirectory, MSTestCommandArguments args, TextWriter writer);
    }
}
