using Gallio.Concurrency;
using Gallio.Hosting;

namespace Gallio.NCoverIntegration
{
    /// <summary>
    /// A <see cref="IsolatedProcessHostFactory" /> that alters the command-line
    /// used to launch the process so as to run the host process inside of
    /// the NCover console application.
    /// </summary>
    /// <todo author="jeff">
    /// We might actually want to directly link to the NCover.Framework assembly
    /// instead of launching the console out of process.
    /// </todo>
    public class NCoverHostFactory : IsolatedProcessHostFactory
    {
        /// <inheritdoc />
        protected override ProcessTask CreateProcessTask(string executablePath, string arguments)
        {
            return new NCoverProcessTask(executablePath, arguments);
        }
    }
}
