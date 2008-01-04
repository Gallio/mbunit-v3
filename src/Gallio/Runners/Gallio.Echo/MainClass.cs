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
using Gallio.Echo.Properties;
using Gallio.Collections;
using Gallio.Core.ConsoleSupport;
using Gallio.Core.ProgressMonitoring;
using Gallio.Model.Filters;
using Gallio.Runner.Reports;
using Gallio.Runner;
using Gallio.Hosting;

namespace Gallio.Echo
{
    /// <summary>
    /// The main process.
    /// </summary>
    public sealed class MainClass : IDisposable
    {
        private static readonly CommandLineArgumentParser argumentParser = new CommandLineArgumentParser(typeof(MainArguments));

        private readonly MainArguments arguments = new MainArguments();
        private readonly IRichConsole console;
        private readonly RichConsoleLogger logger;

        private string applicationTitle;

        public MainClass(IRichConsole console)
        {
            if (console == null)
                throw new ArgumentNullException(@"console");

            this.console = console;

            logger = new RichConsoleLogger(console);
            logger.Level = LoggerLevel.Warn; // temporary setting until arguments are parsed
        }

        /// <summary>
        /// Parses the arguments and runs the program.
        /// </summary>
        public int Run(string[] args)
        {
            try
            {
                if (args == null)
                    throw new ArgumentNullException(@"args");

                return RunMain(args);
            }
            catch (Exception ex)
            {
                logger.FatalFormat(ex, Resources.MainClass_FatalException);
                return ResultCode.FatalException;
            }
        }

        private int RunMain(string[] args)
        {
            SetApplicationTitle();
            ShowBanner();
            InstallCancelHandler();

            if (!argumentParser.Parse(args, arguments, PrintArgumentErrorMessage))
            {
                ShowHelp();
                return ResultCode.InvalidArguments;
            }

            if (arguments.Help)
            {
                ShowHelp();
                return ResultCode.Success;
            }

            SetUpLogger();

            int resultCode = RunTests();
            if (resultCode == ResultCode.Canceled)
                logger.Warn(Resources.MainClass_Canceled);

            return resultCode;
        }

        private int RunTests()
        {
            console.ForegroundColor = ConsoleColor.White;
            console.WriteLine(Resources.MainClass_Initializing);
            console.ResetColor();

            using (TestLauncher launcher = new TestLauncher())
            {
                launcher.Logger = logger;
                launcher.ProgressMonitorProvider = new RichConsoleProgressMonitorProvider(console);

                launcher.RuntimeSetup = new RuntimeSetup();
                launcher.RuntimeSetup.PluginDirectories.AddRange(arguments.PluginDirectories);

                launcher.TestPackageConfig.EnableShadowCopy = arguments.ShadowCopyFiles;
                launcher.TestPackageConfig.ApplicationBase = arguments.AppBaseDirectory;
                launcher.TestPackageConfig.AssemblyFiles.AddRange(arguments.Assemblies);
                launcher.TestPackageConfig.HintDirectories.AddRange(arguments.HintDirectories);

                launcher.ReportDirectory = arguments.ReportDirectory;
                launcher.ReportNameFormat = arguments.ReportNameFormat;

                GenericUtils.AddAll(arguments.ReportTypes, launcher.ReportFormats);

                launcher.DoNotRun = arguments.DoNotRun;

                if (!String.IsNullOrEmpty(arguments.Filter))
                    launcher.Filter = FilterUtils.ParseTestFilter(arguments.Filter);
                
                launcher.EchoResults = !arguments.NoEchoResults;
                launcher.ShowReports = arguments.ShowReports;

                TestLauncherResult result = launcher.Run();
                DisplayResultSummary(result);

                return result.ResultCode;
            }
        }

        private void DisplayResultSummary(TestLauncherResult result)
        {
            switch (result.ResultCode)
            {
                case ResultCode.Success:
                    console.ForegroundColor = ConsoleColor.Green;
                    break;
                case ResultCode.Failure:
                    console.ForegroundColor = ConsoleColor.Red;
                    break;
            }

            console.WriteLine();
            console.WriteLine(result.ResultSummary);
            console.WriteLine();
        }

        private void SetUpLogger()
        {
            SetVerbosityLevel();
            logger.Debug(arguments.ToString());
        }

        private void InstallCancelHandler()
        {
            // Disable ordinary cancelation handling.
            // We handle cancelation directly in ways that should result in the user
            // losing less data than if the OS just kills the process.
            console.IsCancelationEnabled = true;
        }

        private void SetApplicationTitle()
        {
            Version appVersion = Assembly.GetCallingAssembly().GetName().Version;
            applicationTitle = string.Format(Resources.MainClass_ApplicationTitle,
                appVersion.Major, appVersion.Minor, appVersion.Build);

            if (!console.IsRedirected)
                console.Title = applicationTitle;
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
            console.WriteLine();
            console.ForegroundColor = ConsoleColor.White;
            console.WriteLine(applicationTitle);
            console.ForegroundColor = ConsoleColor.Cyan;
            console.Write(Resources.MainClass_GetTheLatestVersionBanner);
            console.ForegroundColor = ConsoleColor.Blue;
            console.WriteLine(Resources.MainClass_MbUnitUrl);
            console.WriteLine();
            console.ResetColor();
        }

        private void PrintArgumentErrorMessage(string message)
        {
            console.ForegroundColor = ConsoleColor.Red;
            console.WriteLine(String.Format(Resources.MainClass_CommandLineArgumentErrorMessageFormat, message));
            console.ResetColor();
            console.WriteLine();
        }

        private void ShowHelp()
        {
            console.ForegroundColor = ConsoleColor.Yellow;
            console.WriteLine(new string('-', console.Width - 2));
            console.ResetColor();
            console.ForegroundColor = ConsoleColor.White;
            console.Write(@"  ");
            console.WriteLine(Resources.MainClass_AvailableOptionsHeader);
            console.ForegroundColor = ConsoleColor.Yellow;
            console.WriteLine(new string('-', console.Width - 2));

            console.ResetColor();
            console.WriteLine();
            argumentParser.ShowUsage(new CommandLineOutput(console));

            // Print out options related to the currently available set of plugins.
            RuntimeSetup setup = new RuntimeSetup();
            if (arguments.PluginDirectories != null)
                setup.PluginDirectories.AddRange(arguments.PluginDirectories);

            Runtime.Initialize(setup);
            try
            {
                IReportManager reportManager = Runtime.Instance.Resolve<IReportManager>();

                string[] formatterNames = GenericUtils.ToArray(reportManager.GetFormatterNames());
                Array.Sort(formatterNames);

                console.WriteLine();
                console.WriteLine(String.Format(Resources.MainClass_SupportedReportTypesMessage,
                    string.Join(@", ", formatterNames)));
            }
            finally
            {
                Runtime.Shutdown();
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
        }

        #endregion
    }
}
