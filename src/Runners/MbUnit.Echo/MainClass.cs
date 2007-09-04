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
using System.Diagnostics;
using System.Reflection;
using Castle.Core.Logging;
using MbUnit.Core.ConsoleSupport;
using MbUnit.Core.Reporting;
using MbUnit.Core.Runner;
using MbUnit.Core.ConsoleSupport.CommandLine;
using MbUnit.Core.Runtime;
using MbUnit.Framework.Kernel.Utilities;

namespace MbUnit.Echo
{
    /// <summary>
    /// The main process.
    /// </summary>
    public sealed class MainClass : IDisposable
    {
        private string applicationTitle;
        private readonly LevelFilteredLogger logger = new SharedConsoleLogger();
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
            ShowBanner();
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
                delegate { return new SharedConsoleProgressMonitor(); },
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

                testRunnerHelper.EchoResults = arguments.EchoResults;

                int result = testRunnerHelper.Run();
                OpenReports(testRunnerHelper);
                DisplayResultSummary(testRunnerHelper, result);

                return result;
            }
        }

        private static void DisplayResultSummary(TestRunnerHelper testRunnerHelper, int result)
        {
            switch (result)
            {
                case ResultCode.Success:
                    Console.ForegroundColor = ConsoleColor.Green;
                    break;
                case ResultCode.Failure:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
            }
            Console.WriteLine("\n" + testRunnerHelper.ResultSummary + "\n");
        }

        private void OpenReports(TestRunnerHelper testRunnerHelper)
        {
            if (arguments.ShowReports)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("Opening reports.");
                Console.ResetColor();
                foreach (string reportType in arguments.ReportTypes)
                {
                    Process.Start(testRunnerHelper.GetReportFilename(reportType));
                }
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
            SharedConsole.InstallCancelationHandler();
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
                    logger.Level = LoggerLevel.Error;
                    break;
                case Verbosity.Normal:
                    logger.Level = LoggerLevel.Warn;
                    break;
                case Verbosity.Verbose:
                    logger.Level = LoggerLevel.Info;
                    break;
                case Verbosity.Debug:
                    logger.Level = LoggerLevel.Debug;
                    break;
            }
        }

        private void ShowBanner()
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(applicationTitle);
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("Get the latest version at ");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("http://www.mbunit.com/");
            Console.WriteLine();
            Console.ResetColor();
        }

        private void ShowHelp()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(new string('-', 78));
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("  Available options:");
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
                    string.Join(", ", ListUtils.CopyAllToArray(reportManager.GetFormatterNames())));
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
        }

        #endregion
    }
}
