// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
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
using System.Net;
using System.ServiceProcess;
using System.Threading;
using Gallio.Ambience.Server.Properties;
using Gallio.Common.Platform;
using Gallio.Runner;
using Gallio.Runtime.ConsoleSupport;

namespace Gallio.Ambience.Server
{
    /// <summary>
    /// The Ambience server application.
    /// </summary>
    public class AmbienceServerProgram : ConsoleProgram<AmbienceServerArguments>
    {
        /// <summary>
        /// Creates an instance of the program.
        /// </summary>
        public AmbienceServerProgram()
        {
            ApplicationName = Resources.ApplicationName;
        }

        /// <inheritdoc />
        protected override int RunImpl(string[] args)
        {
            if (!ParseArguments(args))
            {
                ShowHelp();
                return ResultCode.InvalidArguments;
            }

            if (Arguments.Help)
            {
                ShowHelp();
                return ResultCode.Success;
            }

            AmbienceServerConfiguration configuration = new AmbienceServerConfiguration();
            configuration.Port = Arguments.Port;
            configuration.Credential = new NetworkCredential(Arguments.UserName, Arguments.Password);
            if (Arguments.DatabasePath != null)
                configuration.DatabasePath = Arguments.DatabasePath;

            if (ProcessSupport.ProcessType == ProcessType.Service)
                return RunAsService(configuration);

            return RunAsApplication(configuration);
        }

        private int RunAsService(AmbienceServerConfiguration configuration)
        {
            ServiceBase[] services = new ServiceBase[] { new AmbienceService(configuration) };
            ServiceBase.Run(services);
            return ResultCode.Success;
        }

        private int RunAsApplication(AmbienceServerConfiguration configuration)
        {
            ManualResetEvent waitHandle = new ManualResetEvent(false);
            Console.Cancel += delegate { waitHandle.Set(); };
            Console.IsCancelationEnabled = true;

            ShowBanner();

            using (AmbienceServer server = new AmbienceServer(configuration))
            {
                Console.WriteLine("Starting...");
                server.Start();

                Console.WriteLine("Running...  (Press Ctrl-C to stop)");
                waitHandle.WaitOne();

                Console.WriteLine("Stopping...");
                server.Stop();
                Console.WriteLine("Stopped.");
            }

            return ResultCode.Success;
        }

        protected override void ShowHelp()
        {
            ShowBanner();
            base.ShowHelp();
        }

        private void ShowBanner()
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(ApplicationTitle);
            Console.WriteLine();
            Console.ResetColor();
        }

        [STAThread]
        internal static int Main(string[] args)
        {
            return new AmbienceServerProgram().Run(NativeConsole.Instance, args);
        }
    }
}
