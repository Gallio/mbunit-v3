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
using System.Reflection;
using Castle.Core.Logging;
using Gallio.Echo.Properties;
using Gallio.Collections;
using Gallio.Hosting.ConsoleSupport;
using Gallio.Hosting.ProgressMonitoring;
using Gallio.Model.Filters;
using Gallio.Runner.Reports;
using Gallio.Runner;
using Gallio.Hosting;

namespace Gallio.Echo
{
    /// <summary>
    /// The Gallio console test runner program.
    /// </summary>
    public sealed class EchoProgram : ConsoleProgram<EchoArguments>
    {
        private string applicationTitle;

        /// <inheritdoc />
        protected override int HandleFatalException(Exception ex)
        {
            base.HandleFatalException(ex);
            return ResultCode.FatalException;
        }

        /// <inheritdoc />
        protected override int RunImpl(string[] args)
        {
            SetApplicationTitle();
            ShowBanner();
            InstallCancelHandler();

            if (! ParseArguments(args))
            {
                ShowHelp();
                return ResultCode.InvalidArguments;
            }

            if (Arguments.Help)
            {
                ShowHelp();
                return ResultCode.Success;
            }

            ILogger logger = CreateLogger();

            int resultCode = RunTests(logger);
            if (resultCode == ResultCode.Canceled)
                logger.Warn(Resources.MainClass_Canceled);

            return resultCode;
        }

        private int RunTests(ILogger logger)
        {
            logger.Debug(Arguments.ToString());

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(Resources.MainClass_Initializing);
            Console.ResetColor();

            using (TestLauncher launcher = new TestLauncher())
            {
                launcher.Logger = logger;
                launcher.ProgressMonitorProvider = new RichConsoleProgressMonitorProvider(Console);

                launcher.RuntimeSetup = new RuntimeSetup();
                launcher.RuntimeSetup.PluginDirectories.AddRange(Arguments.PluginDirectories);

                launcher.TestPackageConfig.EnableShadowCopy = Arguments.ShadowCopyFiles;
                launcher.TestPackageConfig.ApplicationBase = Arguments.AppBaseDirectory;
                launcher.TestPackageConfig.AssemblyFiles.AddRange(Arguments.Assemblies);
                launcher.TestPackageConfig.HintDirectories.AddRange(Arguments.HintDirectories);

                launcher.ReportDirectory = Arguments.ReportDirectory;
                launcher.ReportNameFormat = Arguments.ReportNameFormat;

                GenericUtils.AddAll(Arguments.ReportTypes, launcher.ReportFormats);

                launcher.DoNotRun = Arguments.DoNotRun;

                if (!String.IsNullOrEmpty(Arguments.Filter))
                    launcher.Filter = FilterUtils.ParseTestFilter(Arguments.Filter);

                launcher.EchoResults = !Arguments.NoEchoResults;
                launcher.ShowReports = Arguments.ShowReports;

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
                    Console.ForegroundColor = ConsoleColor.Green;
                    break;
                case ResultCode.Failure:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
            }

            Console.WriteLine();
            Console.WriteLine(result.ResultSummary);
            Console.WriteLine();
        }

        private ILogger CreateLogger()
        {
            RichConsoleLogger logger = new RichConsoleLogger(Console);

            switch (Arguments.Verbosity)
            {
                case Verbosity.Quiet:
                    logger.Level = LoggerLevel.Error;
                    break;
                default:
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

            return logger;
        }

        private void InstallCancelHandler()
        {
            // Disable ordinary cancelation handling.
            // We handle cancelation directly in ways that should result in the user
            // losing less data than if the OS just kills the process.
            Console.IsCancelationEnabled = true;
        }

        private void SetApplicationTitle()
        {
            Version appVersion = Assembly.GetCallingAssembly().GetName().Version;
            applicationTitle = string.Format(Resources.MainClass_ApplicationTitle,
                appVersion.Major, appVersion.Minor, appVersion.Build);

            if (!Console.IsRedirected)
                Console.Title = applicationTitle;
        }

        private void ShowBanner()
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(applicationTitle);
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write(Resources.MainClass_GetTheLatestVersionBanner);
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine(Resources.MainClass_MbUnitUrl);
            Console.WriteLine();
            Console.ResetColor();
        }

        protected override void ShowHelp()
        {
            // Show argument only help first because what we do next might take a little while
            // and we want to make the program appear responsive.
            base.ShowHelp();

            // Print out options related to the currently available set of plugins.
            RuntimeSetup setup = new RuntimeSetup();
            if (Arguments != null && Arguments.PluginDirectories != null)
                setup.PluginDirectories.AddRange(Arguments.PluginDirectories);

            Runtime.Initialize(setup, CreateLogger());
            try
            {
                IReportManager reportManager = Runtime.Instance.Resolve<IReportManager>();

                string[] formatterNames = GenericUtils.ToArray(reportManager.GetFormatterNames());
                Array.Sort(formatterNames);

                Console.WriteLine();
                Console.WriteLine(String.Format(Resources.MainClass_SupportedReportTypesMessage,
                    string.Join(@", ", formatterNames)));
            }
            finally
            {
                Runtime.Shutdown();
            }
        }

        [STAThread]
        [LoaderOptimization(LoaderOptimization.MultiDomain)]
        internal static int Main(string[] args)
        {
            return new EchoProgram().Run(NativeConsole.Instance, args);
        }
    }
}
