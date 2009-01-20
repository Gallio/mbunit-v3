using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Gallio.MSTestAdapter.Wrapper
{
    internal abstract class BaseMSTestCommand : IMSTestCommand
    {
        private readonly string executablePath;

        public BaseMSTestCommand(Version frameworkVersion)
        {
            if (frameworkVersion == null)
                throw new ArgumentException("frameworkVersion");

            executablePath = MSTestResolver.FindMSTestPathForFrameworkVersion(frameworkVersion);
        }

        /// <inheritdoc />
        public abstract int Run(string workingDirectory, MSTestCommandArguments args, TextWriter outputWriter, TextWriter errorWriter);

        protected string ExecutablePath
        {
            get { return executablePath; }
        }
    }
}
