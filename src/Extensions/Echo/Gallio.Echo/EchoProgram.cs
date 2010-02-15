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
using System.Collections.Generic;
using System.IO;
using Gallio.Common;
using Gallio.Common.IO;
using Gallio.Common.Text;
using Gallio.Common.Xml;
using Gallio.Runner.Projects;
using Gallio.Runner.Projects.Schema;
using Gallio.Runtime.Debugging;
using Gallio.Runtime.Logging;
using Gallio.Runtime;
using Gallio.Echo.Properties;
using Gallio.Common.Collections;
using Gallio.Runtime.ConsoleSupport;
using Gallio.Runtime.ProgressMonitoring;
using Gallio.Model.Filters;
using Gallio.Common.Reflection;
using Gallio.Runner.Reports;
using Gallio.Runner;

namespace Gallio.Echo
{
    /// <summary>
    /// The Gallio console test runner program.
    /// </summary>
    public sealed class EchoProgram : ConsoleProgram<EchoArguments>
    {
        /// <summary>
        /// Creates an instance of the program.
        /// </summary>
        private EchoProgram()
        {
            ApplicationName = Resources.ApplicationName;
        }

        /// <inheritdoc />
        protected override int HandleFatalException(Exception ex)
        {
            base.HandleFatalException(ex);
            return ResultCode.FatalException;
        }

        /// <inheritdoc />
        protected override int RunImpl(string[] args)
        {
            ShowBanner();
            InstallCancelHandler();

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

            ILogger logger = CreateLogger();
            int resultCode = RunTests(logger);

            if (resultCode == ResultCode.Canceled)
                logger.Log(LogSeverity.Warning, Resources.MainClass_Canceled);

            return resultCode;
        }

        private int RunTests(ILogger logger)
        {
            logger.Log(LogSeverity.Debug, Arguments.ToString());

            var launcher = new TestLauncher
            {
                Logger = logger,
                ProgressMonitorProvider = CreateProgressMonitorProvider()
            };

            ConfigureLauncherFromArguments(launcher, Arguments);
            TestLauncherResult result = launcher.Run();
            DisplayResultSummary(result);
            return result.ResultCode;
        }

        internal static void ConfigureLauncherFromArguments(TestLauncher launcher, EchoArguments arguments)
        {
            launcher.RuntimeSetup = new RuntimeSetup();
            GenericCollectionUtils.ForEach(arguments.PluginDirectories, x => launcher.RuntimeSetup.AddPluginDirectory(x));

            if (arguments.ShadowCopy.HasValue)
                launcher.TestProject.TestPackage.ShadowCopy = arguments.ShadowCopy.Value;

            if (arguments.Debug.HasValue && arguments.Debug.Value)
                launcher.TestProject.TestPackage.DebuggerSetup = new DebuggerSetup();

            if (arguments.ApplicationBaseDirectory != null)
                launcher.TestProject.TestPackage.ApplicationBaseDirectory = new DirectoryInfo(arguments.ApplicationBaseDirectory);

            if (arguments.WorkingDirectory != null)
                launcher.TestProject.TestPackage.WorkingDirectory = new DirectoryInfo(arguments.WorkingDirectory);

            if (arguments.RuntimeVersion != null)
                launcher.TestProject.TestPackage.RuntimeVersion = arguments.RuntimeVersion;

            GenericCollectionUtils.ForEach(arguments.Files, launcher.AddFilePattern);

            foreach (string hintDirectory in arguments.HintDirectories)
                launcher.TestProject.TestPackage.AddHintDirectory(new DirectoryInfo(hintDirectory));

            if (arguments.ReportDirectory != null)
                launcher.TestProject.ReportDirectory = arguments.ReportDirectory;

            if (arguments.ReportNameFormat != null)
                launcher.TestProject.ReportNameFormat = arguments.ReportNameFormat;

            launcher.TestProject.ReportArchive = ReportArchive.Parse(arguments.ReportArchive);
            GenericCollectionUtils.ForEach(arguments.ReportTypes, launcher.AddReportFormat);

            if (arguments.RunnerType != null)
                launcher.TestProject.TestRunnerFactoryName = arguments.RunnerType;
            GenericCollectionUtils.ForEach(arguments.RunnerExtensions, x => launcher.TestProject.AddTestRunnerExtensionSpecification(x));

            foreach (string option in arguments.ReportFormatterProperties)
            {
                KeyValuePair<string, string> pair = StringUtils.ParseKeyValuePair(option);
                launcher.ReportFormatterOptions.AddProperty(pair.Key, pair.Value);
            }

            foreach (string option in arguments.RunnerProperties)
            {
                KeyValuePair<string, string> pair = StringUtils.ParseKeyValuePair(option);
                launcher.TestRunnerOptions.AddProperty(pair.Key, pair.Value);
            }

            launcher.DoNotRun = arguments.DoNotRun;
            launcher.IgnoreAnnotations = arguments.IgnoreAnnotations;

            if (!String.IsNullOrEmpty(arguments.Filter))
                launcher.TestExecutionOptions.FilterSet = FilterUtils.ParseTestFilterSet(arguments.Filter);

            launcher.EchoResults = !arguments.NoEchoResults;
            launcher.ShowReports = arguments.ShowReports;

            if (arguments.RunTimeLimitInSeconds >= 0)
                launcher.RunTimeLimit = TimeSpan.FromSeconds(arguments.RunTimeLimitInSeconds);
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
            var logger = new RichConsoleLogger(Console);
            return new FilteredLogger(logger, Arguments.Verbosity);
        }

        private IProgressMonitorProvider CreateProgressMonitorProvider()
        {
            if (Arguments.NoProgress)
                return NullProgressMonitorProvider.Instance;

            return new RichConsoleProgressMonitorProvider(Console);
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
            var setup = new RuntimeSetup();

            if (Arguments != null && Arguments.PluginDirectories != null)
            {
                GenericCollectionUtils.ForEach(Arguments.PluginDirectories, x => setup.AddPluginDirectory(x));
            }

            using (RuntimeBootstrap.Initialize(setup, CreateLogger()))
            {
                IReportManager reportManager = RuntimeAccessor.ServiceLocator.Resolve<IReportManager>();
                ShowRegisteredComponents("Supported report types:", reportManager.FormatterHandles,
                    h => h.GetTraits().Name, h => h.GetTraits().Description);

                ITestRunnerManager runnerManager = RuntimeAccessor.ServiceLocator.Resolve<ITestRunnerManager>();
                ShowRegisteredComponents("Supported runner types:", runnerManager.TestRunnerFactoryHandles,
                    h => h.GetTraits().Name, h => h.GetTraits().Description);
            }
        }

        private void ShowRegisteredComponents<T>(string heading, ICollection<T> handles, Func<T, string> getName, Func<T, string> getDescription)
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

        [STAThread]
        //[LoaderOptimization(LoaderOptimization.MultiDomain)] // Disabled due to bug: http://connect.microsoft.com/VisualStudio/feedback/ViewFeedback.aspx?FeedbackID=95157
        internal static int Main(string[] args)
        {
            return new EchoProgram().Run(NativeConsole.Instance, args);
        }
    }
}
