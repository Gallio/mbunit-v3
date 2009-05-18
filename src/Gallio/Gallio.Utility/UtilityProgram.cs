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
using System.Collections.Generic;
using Gallio.Common;
using Gallio.Common.Collections;
using Gallio.Runtime;
using Gallio.Runtime.Extensibility;
using Gallio.Runtime.Logging;
using Gallio.Runtime.ConsoleSupport;
using Gallio.Runtime.ProgressMonitoring;
using Gallio.Runtime.UtilityCommands;
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

            if (! ParseArguments(args) || Arguments.CommandAndArguments.Length == 0)
            {
                ShowHelp();
                return Arguments.Help ? 0 : 1;
            }

            ILogger logger = CreateLogger();
            IProgressMonitorProvider progressMonitorProvider = Arguments.NoProgress
                ? (IProgressMonitorProvider) NullProgressMonitorProvider.Instance
                : new RichConsoleProgressMonitorProvider(Console);

            string commandName = Arguments.CommandAndArguments[0];
            string[] commandRawArguments = new string[Arguments.CommandAndArguments.Length - 1];
            Array.Copy(Arguments.CommandAndArguments, 1, commandRawArguments, 0, commandRawArguments.Length);

            IUtilityCommand command = GetSpecialCommand(commandName);
            bool isSpecialCommand = command != null;

            var runtimeSetup = new RuntimeSetup();
            using (isSpecialCommand ? null : RuntimeBootstrap.Initialize(runtimeSetup, logger))
            {
                if (command == null)
                {
                    var commandManager = RuntimeAccessor.ServiceLocator.Resolve<IUtilityCommandManager>();
                    command = commandManager.GetCommand(commandName);
                    if (command == null)
                    {
                        ShowErrorMessage(string.Format("Unrecognized utility command name: '{0}'.", commandName));
                        ShowHelp();
                        return 1;
                    }
                }

                Type commandArgumentsClass = command.GetArgumentClass();
                var commandArgumentParser = new CommandLineArgumentParser(commandArgumentsClass, null);

                if (Arguments.Help)
                {
                    ShowHelpForParticularCommand(commandName, commandArgumentParser);
                    return 0;
                }

                object commandArguments = Activator.CreateInstance(commandArgumentsClass);

                if (! commandArgumentParser.Parse(commandRawArguments, commandArguments, ShowErrorMessage)
                    ||  ! command.ValidateArguments(commandArguments, ShowErrorMessage))
                {
                    ShowHelpForParticularCommand(commandName, commandArgumentParser);
                    return 1;
                }

                UtilityCommandContext commandContext = new UtilityCommandContext(commandArguments, Console, logger, progressMonitorProvider, Arguments.Verbosity);
                return command.Execute(commandContext);
            }
        }

        protected override void ShowHelp()
        {
            // Show argument only help first because what we do next might take a little while
            // and we want to make the program appear responsive.
            base.ShowHelp();

            // Print out options related to the currently available set of plugins.
            RuntimeSetup setup = new RuntimeSetup();

            using (RuntimeAccessor.IsInitialized ? null : RuntimeBootstrap.Initialize(setup, CreateLogger()))
            {
                IUtilityCommandManager utilityCommandManager = RuntimeAccessor.ServiceLocator.Resolve<IUtilityCommandManager>();
                ShowRegisteredComponents("Supported utility commands:", utilityCommandManager.CommandHandles,
                    h => h.GetTraits().Name, h => h.GetTraits().Description);
            }
        }

        private void ShowHelpForParticularCommand(string commandName, CommandLineArgumentParser parser)
        {
            base.ShowHelp();

            Console.WriteLine(string.Format("Additional options for command '{0}'.", commandName));
            Console.WriteLine();

            parser.ShowUsage(CommandLineOutput);
        }

        private void ShowRegisteredComponents<T>(string heading, ICollection<T> handles,
            Func<T, string> getName, Func<T, string> getDescription)
        {
            Console.WriteLine(heading);
            Console.WriteLine();

            T[] sortedHandles = GenericCollectionUtils.ToArray(handles);
            Array.Sort(sortedHandles, (x, y) => getName(x).CompareTo(getName(y)));
            if (sortedHandles.Length == 0)
            {
                CommandLineOutput.PrintArgumentHelp("", "<none>", null, null, null, null);
            }
            else
            {
                foreach (T handle in sortedHandles)
                {
                    CommandLineOutput.PrintArgumentHelp("", getName(handle), null, getDescription(handle), null, null);
                    Console.WriteLine();
                }
            }
        }

        private static IUtilityCommand GetSpecialCommand(string commandName)
        {
            // These commands are special and are hardcoded here because we want to be
            // able to run them without initializing the runtime itself.
            switch (commandName.ToLowerInvariant())
            {
                case "clearcurrentuserplugincache":
                    return new ClearCurrentUserPluginCacheCommand();

                default:
                    return null;
            }
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