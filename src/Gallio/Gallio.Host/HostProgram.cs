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
using System.Runtime.Remoting;
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

            Console.WriteLine(String.Format("* Started at {0}.", DateTime.Now));

            try
            {
                using (BinaryIpcServerChannel serverChannel = new BinaryIpcServerChannel(Arguments.IpcPortName))
                {
                    using (new BinaryIpcClientChannel(Arguments.IpcPortName + @".Callback"))
                    {
                        using (HostService hostService = new HostService(WatchdogTimeout))
                        {
                            HostServiceChannelInterop.RegisterWithChannel(hostService, serverChannel);

                            Console.WriteLine(String.Format("* Listening for connections on IPC port: '{0}'", Arguments.IpcPortName));
                            hostService.WaitUntilDisposed();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(String.Format("* A fatal exception occurred: {0}", ex));
            }

            Console.WriteLine(String.Format("* Stopped at {0}.", DateTime.Now));
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
