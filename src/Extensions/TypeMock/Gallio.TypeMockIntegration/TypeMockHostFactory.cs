using System;
using System.Collections.Generic;
using System.Text;
using Gallio.Concurrency;
using Gallio.Hosting;

namespace Gallio.TypeMockIntegration
{
    /// <summary>
    /// A <see cref="IsolatedProcessHostFactory" /> that alters the command-line
    /// used to launch the process so as to run the host process inside of
    /// the NCover console application.
    /// </summary>
    public class TypeMockHostFactory : IsolatedProcessHostFactory
    {
        /// <inheritdoc />
        protected override ProcessTask CreateProcessTask(string executablePath, string arguments)
        {
            return new TypeMockProcessTask(executablePath, arguments);
        }
    }
}
