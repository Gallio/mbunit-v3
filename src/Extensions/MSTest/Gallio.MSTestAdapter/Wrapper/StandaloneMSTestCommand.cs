using System;
using System.Diagnostics;
using System.IO;
using Gallio.Concurrency;

namespace Gallio.MSTestAdapter.Wrapper
{
    /// <summary>
    /// An MSTest command implementation that runs MSTest as a separate process.
    /// </summary>
    internal class StandaloneMSTestCommand : IMSTestCommand
    {
        public static readonly StandaloneMSTestCommand Instance = new StandaloneMSTestCommand();

        private StandaloneMSTestCommand()
        {
        }

        public int Run(string workingDirectory, MSTestCommandArguments args,
            TextWriter outputWriter, TextWriter errorWriter)
        {
            string executable = MSTestResolver.FindDefaultMSTestPath();
            if (executable == null)
                return -1;

            ProcessTask task = new ProcessTask(executable, args.ToString(), workingDirectory);
            task.ConsoleOutputDataReceived += MakeRedirector(outputWriter);
            task.ConsoleErrorDataReceived += MakeRedirector(errorWriter);

            task.Run(null);

            return task.ExitCode;
        }

        private static EventHandler<DataReceivedEventArgs> MakeRedirector(TextWriter writer)
        {
            return delegate(object sender, DataReceivedEventArgs e)
            {
                if (e.Data != null)
                    writer.WriteLine(e.Data);
            };
        }
    }
}
