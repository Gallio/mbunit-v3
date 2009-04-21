// Copyright 2005-2009 Gallio Project - http://www.gallio.org/
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
using System.IO;
using Gallio.Runtime;
using Gallio.Runtime.Extensibility;
using Gallio.Runtime.Logging;
using Gallio.Runtime.ConsoleSupport;
using Gallio.Utility.Properties;

namespace Gallio.Utility
{
    /// <summary>
    /// The Gallio utility program.
    /// </summary>
    public sealed class UtilityProgram : ConsoleProgram<UtilityArguments>
    {
        /// <summary>
        /// Creates an instance of the program.
        /// </summary>
        private UtilityProgram()
        {
            ApplicationName = Resources.ApplicationName;
        }

        /// <inheritdoc />
        protected override int RunImpl(string[] args)
        {
            ShowBanner();
            InstallCancelHandler();

            if (! ParseArguments(args))
            {
                ShowHelp();
                return 1;
            }

            if (Arguments.Help)
            {
                ShowHelp();
                return 0;
            }

            ILogger logger = CreateLogger();

            int resultCode = RunCommand(logger);
            return resultCode;
        }

        private int RunCommand(ILogger logger)
        {
            switch (Arguments.Command.ToLowerInvariant())
            {
                case "verifyinstallation":
                    return VerifyInstallation(logger);

                case "clearcurrentuserplugincache":
                    return ClearCurrentUserPluginCache(logger);

                default:
                    logger.Log(LogSeverity.Error, "Unrecognized command.");
                    return 1;
            }
        }

        private int VerifyInstallation(ILogger logger)
        {
            var runtimeSetup = new RuntimeSetup();
            using (RuntimeBootstrap.Initialize(runtimeSetup, logger))
            {
                IRuntime runtime = RuntimeAccessor.Instance;

                return runtime.VerifyInstallation() ? 0 : 1;
            }
        }

        private int ClearCurrentUserPluginCache(ILogger logger)
        {
            logger.Log(LogSeverity.Important, "Clearing the current user's plugin cache.");

            CachingPluginLoader.ClearCurrentUserPluginCache();
            return 0;
        }

        private ILogger CreateLogger()
        {
            RichConsoleLogger logger = new RichConsoleLogger(Console);
            return new FilteredLogger(logger, Arguments.Verbosity);
        }

        private void InstallCancelHandler()
        {
            // Disable ordinary cancelation handling.
            // We handle cancelation directly in ways that should result in the user
            // losing less data than if the OS just kills the process.
            Console.IsCancelationEnabled = true;
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
        //[LoaderOptimization(LoaderOptimization.MultiDomain)] // Disabled due to bug: http://connect.microsoft.com/VisualStudio/feedback/ViewFeedback.aspx?FeedbackID=95157
        internal static int Main(string[] args)
        {
            return new UtilityProgram().Run(NativeConsole.Instance, args);
        }
    }
}