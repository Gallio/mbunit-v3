// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
// Portions Copyright 2000-2004 Jonathan de Halleux
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Diagnostics;
using System.IO;
using Gallio.Concurrency;

namespace Gallio.MSTestAdapter.Wrapper
{
    /// <summary>
    /// Runs MSTest as a separate process.
    /// </summary>
    internal sealed class StandaloneMSTestCommand : MSTestCommand
    {
        private StandaloneMSTestCommand()
        {
        }

        public static readonly StandaloneMSTestCommand Instance = new StandaloneMSTestCommand();

        public override int Run(string executablePath, string workingDirectory,
            MSTestCommandArguments args, TextWriter writer)
        {
            ProcessTask task = new ProcessTask(executablePath, args.ToString(), workingDirectory);
            task.ConsoleOutputDataReceived += MakeRedirector(writer);
            task.ConsoleErrorDataReceived += MakeRedirector(writer);

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
