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
using Gallio.Utilities;

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

            TestLauncher launcher = new TestLauncher
                                        {
                                            Logger = logger,
                                            ProgressMonitorProvider = new RichConsoleProgressMonitorProvider(Console)
                                        };

            ConfigureLauncherFromArguments(launcher, Arguments);

            TestLauncherResult result = launcher.Run();
            DisplayResultSummary(result);

            return result.ResultCode;
        }

        internal static void ConfigureLauncherFromArguments(TestLauncher launcher, EchoArguments arguments)
        {
            launcher.RuntimeSetup = new RuntimeSetup();
            launcher.RuntimeSetup.PluginDirectories.AddRange(arguments.PluginDirectories);

            // Set the installation path explicitly to ensure that we do not encounter problems
            // when the test assembly contains a local copy of the primary runtime assemblies
            // which will confuse the runtime into searching in the wrong place for plugins.
            launcher.RuntimeSetup.RuntimePath = Path.GetDirectoryName(AssemblyUtils.GetFriendlyAssemblyLocation(typeof(EchoProgram).Assembly));

            launcher.TestPackageConfig.HostSetup.ShadowCopy = arguments.ShadowCopy;
            launcher.TestPackageConfig.HostSetup.Debug = arguments.Debug;
            launcher.TestPackageConfig.HostSetup.ApplicationBaseDirectory = arguments.ApplicationBaseDirectory;
            launcher.TestPackageConfig.HostSetup.WorkingDirectory = arguments.WorkingDirectory;

            // add assemblies to testpackageconfig
            foreach (string assembly in arguments.Assemblies)
            {
                if (!CheckAssembly(launcher, arguments, assembly))
                    break;
            }
            launcher.TestPackageConfig.HintDirectories.AddRange(arguments.HintDirectories);

            launcher.ReportDirectory = arguments.ReportDirectory;
            launcher.ReportNameFormat = arguments.ReportNameFormat;

            GenericUtils.AddAll(arguments.ReportTypes, launcher.ReportFormats);

            launcher.TestRunnerFactoryName = arguments.RunnerType;
            GenericUtils.AddAll(arguments.RunnerExtensions, launcher.TestRunnerExtensionSpecifications);

            foreach (string option in arguments.ReportFormatterProperties)
                launcher.ReportFormatterOptions.Properties.Add(StringUtils.ParseKeyValuePair(option));

            foreach (string option in arguments.RunnerProperties)
                launcher.TestRunnerOptions.Properties.Add(StringUtils.ParseKeyValuePair(option));

            launcher.DoNotRun = arguments.DoNotRun;
            launcher.IgnoreAnnotations = arguments.IgnoreAnnotations;

            if (!String.IsNullOrEmpty(arguments.Filter))
                launcher.TestExecutionOptions.FilterSet = FilterUtils.ParseTestFilterSet(arguments.Filter);

            launcher.EchoResults = !arguments.NoEchoResults;
            launcher.ShowReports = arguments.ShowReports;

            if (arguments.RunTimeLimitInSeconds >= 0)
                launcher.RunTimeLimit = TimeSpan.FromSeconds(arguments.RunTimeLimitInSeconds);
        }

        private static bool CheckAssembly(TestLauncher launcher, EchoArguments arguments, string assembly)
        {
            if (Path.GetExtension(assembly) == Project.Extension)
            {
                if (arguments.Assemblies.Length > 1)
                    throw new ArgumentException("Please don't mix and match gallio project files and assemblies!");

                ProjectUtils projectUtils = new ProjectUtils(new FileSystem(), new DefaultXmlSerializer());
                Project project = projectUtils.LoadProject(assembly);
                launcher.TestPackageConfig = project.TestPackageConfig;

                // add test runner extensions from project to command line args
                List<string> testRunnerExtensions = project.TestRunnerExtensions;
                testRunnerExtensions.AddRange(arguments.RunnerExtensions);
                arguments.RunnerExtensions = testRunnerExtensions.ToArray();

                return false;
            }
                
            if (File.Exists(assembly))
                launcher.TestPackageConfig.AssemblyFiles.Add(assembly);
            else 
            {
                // could be a wildcarded string
                string path = Environment.CurrentDirectory;
                if (Path.IsPathRooted(assembly))
                    path = Path.GetDirectoryName(assembly);
                var info = new DirectoryInfo(path);
                var files = info.GetFiles(Path.GetFileName(assembly));

                if (files.Length == 0)
                {
                    System.Console.WriteLine(string.Format("Could not find any match for assembly: {0}", assembly));
                    return true;
                }

                foreach (var file in files)
                    launcher.TestPackageConfig.AssemblyFiles.Add(Path.Combine(path, file.Name));
            }
            return true;
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
                IReportManager reportManager = RuntimeAccessor.ServiceLocator.Resolve<IReportManager>();
                ShowRegisteredComponents("Supported report types:", reportManager.FormatterHandles,
                    h => h.GetTraits().Name, h => h.GetTraits().Description);

                ITestRunnerManager runnerManager = RuntimeAccessor.ServiceLocator.Resolve<ITestRunnerManager>();
                ShowRegisteredComponents("Supported runner types:", runnerManager.TestRunnerFactoryHandles,
                    h => h.GetTraits().Name, h => h.GetTraits().Description);
            }
        }

        private void ShowRegisteredComponents<T>(string heading, ICollection<T> handles,
            Func<T, string> getName, Func<T, string> getDescription)
        {
            Console.WriteLine(heading);
            Console.WriteLine();

            T[] sortedHandles = GenericUtils.ToArray(handles);
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
