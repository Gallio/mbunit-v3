// Copyright 2007 MbUnit Project - http://www.mbunit.com/
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
using System.Reflection;
using Castle.Core.Logging;
using MbUnit.Core.Reporting;
using MbUnit.Core.Runner;
using MbUnit.Core.Runner.CommandLine;
using MbUnit.Core.Services.Runtime;
using MbUnit.Framework.Kernel.Utilities;

namespace MbUnit.Echo
{
    /// <summary>
    /// The main process.
    /// </summary>
    public sealed class MainClass : IDisposable
    {
        private string applicationTitle;
        private LevelFilteredLogger logger = new PrettyConsoleLogger();
        private readonly MainArguments arguments = new MainArguments();
        private bool haltExecution = false;
        private int resultCode;

        public MainClass(string[] args)
        {
            if (args == null)
                throw new ArgumentNullException("args");
            SetUp(args);
        }

        public void SetUp(string[] args)
        {
            SetApplicationTitle();
            InstallCancelHandler();

            if (!ParseArguments(args))
            {
                haltExecution = true;
                resultCode = ResultCode.InvalidArguments;
            }

            if (arguments.Help)
            {
                ShowHelp();
                haltExecution = true;
                resultCode = ResultCode.Success;
            }

            if (resultCode == ResultCode.Success)
                SetUpLogger();
        }

        public int Run()
        {
            try
            {
                if (haltExecution)
                {
                    return resultCode;
                }
                resultCode = RunTests();
                CheckResultCode();
                return resultCode;
            }
            catch (Exception ex)
            {
                logger.FatalFormat(ex, "A fatal exception occurred.");
                return ResultCode.FatalException;
            }
        }

        private int RunTests()
        {
            using (TestRunnerHelper testRunnerHelper = new TestRunnerHelper
                (
                delegate { return new ConsoleProgressMonitor(); },
                logger,
                arguments.GetFilter()
                ))
            {
                testRunnerHelper.Package.EnableShadowCopy = arguments.ShadowCopyFiles;
                testRunnerHelper.Package.ApplicationBase = arguments.AppBaseDirectory;
                testRunnerHelper.Package.AssemblyFiles.AddRange(arguments.Assemblies);
                testRunnerHelper.Package.HintDirectories.AddRange(arguments.HintDirectories);

                testRunnerHelper.RuntimeSetup.PluginDirectories.AddRange(arguments.PluginDirectories);

                testRunnerHelper.ReportDirectory = arguments.ReportDirectory;
                testRunnerHelper.ReportNameFormat = arguments.ReportNameFormat;
                testRunnerHelper.ReportFormats.AddRange(arguments.ReportTypes);
                testRunnerHelper.TemplateModelFilename = arguments.SaveTemplateTree;
                testRunnerHelper.TestModelFilename = arguments.SaveTestTree;

                return testRunnerHelper.Run();
            }
        }

        private void SetUpLogger()
        {
            SetVerbosityLevel();
            logger.Debug(arguments.ToString());
        }

        private static void InstallCancelHandler()
        {
            // Disable ordinary cancelation handling.
            // We handle cancelation directly in ways that should result in the user
            // losing less data than if the OS just kills the process.
            ConsoleCancelHandler.Install();
        }

        private void CheckResultCode()
        {
            if (resultCode == ResultCode.Canceled)
                logger.Warn("Canceled!");
        }

        private bool ParseArguments(string[] args)
        {
            try
            {
                CommandLineUtility.ParseCommandLineArguments(args, arguments, delegate(string message)
                {
                    logger.FatalFormat("Error: {0}", message);
                });
            }
            catch (Exception)
            {
                ShowHelp();
                return false;
            }
            return true;
        }

        private void SetApplicationTitle()
        {
            Version appVersion = Assembly.GetCallingAssembly().GetName().Version;
            applicationTitle = string.Format("MbUnit Echo - Version {0}.{1} build {2}", appVersion.Major, appVersion.Minor, appVersion.Build);
            Console.Title = applicationTitle;
        }

        private void SetVerbosityLevel()
        {
            switch (arguments.Verbosity)
            {
                case Verbosity.Quiet:
                    logger.Level = LoggerLevel.Warn;
                    break;
                case Verbosity.Normal:
                case Verbosity.Verbose:
                    logger.Level = LoggerLevel.Info;
                    break;
                case Verbosity.Debug:
                    logger.Level = LoggerLevel.Debug;
                    break;
            }
        }

        private void ShowHelp()
        {
            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(applicationTitle);
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("Get the latest version at ");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("http://www.mbunit.com/");
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(new string('-', 78));

            Console.ResetColor();
            Console.WriteLine("Project Lead: Andrew Stopford");
            Console.WriteLine("Contributors: Ben Hall, Graham Hay, Johan Appelgren, Joey Calisay,");
            Console.WriteLine("              David Parkinson, Jeff Brown, Marc Stober, Mark Haley");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(new string('-', 78));

            Console.ResetColor();
            Console.WriteLine();
            CommandLineUtility.CommandLineArgumentsUsage(typeof(MainArguments));

            // Print out options related to the currently available set of plugins.
            RuntimeSetup setup = new RuntimeSetup();
            if (arguments.PluginDirectories != null)
                setup.PluginDirectories.AddRange(arguments.PluginDirectories);
            using (AutoRunner runner = AutoRunner.CreateRunner(setup))
            {
                IReportManager reportManager = runner.Runtime.Resolve<IReportManager>();

                Console.WriteLine();
                Console.WriteLine("Supported report types: {0}", 
                    string.Join(",", ListUtils.CopyAllToArray(reportManager.GetFormatterNames())));
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
        }

        #endregion
    }
}
