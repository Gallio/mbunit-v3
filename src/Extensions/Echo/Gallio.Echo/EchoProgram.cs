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
using Gallio.Runner.Projects;
using Gallio.Runtime.Logging;
using Gallio.Runtime;
using Gallio.Echo.Properties;
using Gallio.Collections;
using Gallio.Runtime.ConsoleSupport;
using Gallio.Runtime.ProgressMonitoring;
using Gallio.Model.Filters;
using Gallio.Reflection;
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
        public EchoProgram()
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
                logger.Log(LogSeverity.Warning, Resources.MainClass_Canceled);

            return resultCode;
        }

        private int RunTests(ILogger logger)
        {
            logger.Log(LogSeverity.Debug, Arguments.ToString());

            TestLauncher launcher = new TestLauncher();
            launcher.Logger = logger;
            launcher.ProgressMonitorProvider = new RichConsoleProgressMonitorProvider(Console);

            launcher.RuntimeSetup = new RuntimeSetup();
            launcher.RuntimeSetup.PluginDirectories.AddRange(Arguments.PluginDirectories);

            // Set the installation path explicitly to ensure that we do not encounter problems
            // when the test assembly contains a local copy of the primary runtime assemblies
            // which will confuse the runtime into searching in the wrong place for plugins.
            launcher.RuntimeSetup.RuntimePath = Path.GetDirectoryName(AssemblyUtils.GetFriendlyAssemblyLocation(typeof(EchoProgram).Assembly));

            launcher.TestPackageConfig.HostSetup.ShadowCopy = Arguments.ShadowCopy;
            launcher.TestPackageConfig.HostSetup.Debug = Arguments.Debug;
            launcher.TestPackageConfig.HostSetup.ApplicationBaseDirectory = Arguments.ApplicationBaseDirectory;
            launcher.TestPackageConfig.HostSetup.WorkingDirectory = Arguments.WorkingDirectory;

            // add assemblies to testpackageconfig
            foreach (string assembly in Arguments.Assemblies)
            {
                if (!File.Exists(assembly))
                    continue;

                if (Path.GetExtension(assembly) == ".gallio")
                {
                    if (Arguments.Assemblies.Length > 1)
                        throw new ArgumentException("Please don't mix and match gallio project files and assemblies!");

                    Project project = ProjectUtils.LoadProject(assembly);
                    launcher.TestPackageConfig = project.TestPackageConfig;
                    break;
                }
                launcher.TestPackageConfig.AssemblyFiles.Add(assembly);
            }
            launcher.TestPackageConfig.HintDirectories.AddRange(Arguments.HintDirectories);

            launcher.ReportDirectory = Arguments.ReportDirectory;
            launcher.ReportNameFormat = Arguments.ReportNameFormat;

            GenericUtils.AddAll(Arguments.ReportTypes, launcher.ReportFormats);

            launcher.TestRunnerFactoryName = Arguments.RunnerType;
            GenericUtils.AddAll(Arguments.RunnerExtensions, launcher.TestRunnerExtensionSpecifications);

            launcher.DoNotRun = Arguments.DoNotRun;
            launcher.IgnoreAnnotations = Arguments.IgnoreAnnotations;

            if (!String.IsNullOrEmpty(Arguments.Filter))
                launcher.TestExecutionOptions.Filter = FilterUtils.ParseTestFilter(Arguments.Filter);

            launcher.EchoResults = !Arguments.NoEchoResults;
            launcher.ShowReports = Arguments.ShowReports;

            if (Arguments.RunTimeLimitInSeconds >= 0)
                launcher.RunTimeLimit = TimeSpan.FromSeconds(Arguments.RunTimeLimitInSeconds);

            TestLauncherResult result = launcher.Run();
            DisplayResultSummary(result);

            return result.ResultCode;
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

            LogSeverity minSeverity;
            switch (Arguments.Verbosity)
            {
                case Verbosity.Quiet:
                    minSeverity = LogSeverity.Warning;
                    break;
                case Verbosity.Verbose:
                    minSeverity = LogSeverity.Info;
                    break;
                case Verbosity.Debug:
                    minSeverity = LogSeverity.Debug;
                    break;
                default:
                    minSeverity = LogSeverity.Important;
                    break;
            }

            return new FilteredLogger(logger, minSeverity);
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
            RuntimeSetup setup = new RuntimeSetup();
            if (Arguments != null && Arguments.PluginDirectories != null)
                setup.PluginDirectories.AddRange(Arguments.PluginDirectories);

            using (RuntimeBootstrap.Initialize(setup, CreateLogger()))
            {
                IReportManager reportManager = RuntimeAccessor.Instance.Resolve<IReportManager>();
                ShowRegisteredComponents("Supported report types:", reportManager.FormatterResolver);

                ITestRunnerManager runnerManager = RuntimeAccessor.Instance.Resolve<ITestRunnerManager>();
                ShowRegisteredComponents("Supported runner types:", runnerManager.FactoryResolver);
            }
        }

        private void ShowRegisteredComponents<T>(string heading, IRegisteredComponentResolver<T> resolver)
            where T : class, IRegisteredComponent
        {
            Console.WriteLine();
            Console.WriteLine(heading);
            Console.WriteLine();

            string[] names = GenericUtils.ToArray(resolver.GetNames());
            if (names.Length == 0)
            {
                CommandLineOutput.PrintArgumentHelp("", "<none>", null, null, null, null);
            }
            else
            {
                Array.Sort(names);
                foreach (string name in names)
                {
                    T component = resolver.Resolve(name);
                    CommandLineOutput.PrintArgumentHelp("", name, null, component.Description, null, null);
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
