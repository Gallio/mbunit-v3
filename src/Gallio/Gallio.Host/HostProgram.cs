// Copyright 2008 MbUnit Project - http://www.mbunit.com/
// Portions Copyright 2000-2004 Jonathan De Halleux, Jamie Cansdale
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
using Gallio.Hosting.ConsoleSupport;
using Gallio.Hosting;
using Gallio.Hosting.Channels;

namespace Gallio.Host
{
    /// <summary>
    /// The Gallio host process.
    /// </summary>
    public sealed class HostProgram : ConsoleProgram<HostArguments>
    {
        private readonly TimeSpan WatchdogTimeout = TimeSpan.FromSeconds(30);

        /// <inheritdoc />
        protected override int RunImpl(string[] args)
        {
            if (!ParseArguments(args))
            {
                ShowHelp();
                return -1;
            }

            using (BinaryIpcServerChannel serverChannel = new BinaryIpcServerChannel(Arguments.IpcPortName))
            {
                using (HostService hostService = new HostService(WatchdogTimeout))
                {
                    HostServiceChannelInterop.RegisterWithChannel(hostService, serverChannel);

                    hostService.WaitUntilDisposed();
                }
            }

            return 0;
        }

        [STAThread]
        [LoaderOptimization(LoaderOptimization.MultiDomain)]
        internal static int Main(string[] args)
        {
            return new HostProgram().Run(NativeConsole.Instance, args);
        }
    }
}
