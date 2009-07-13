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
using System.Collections.Specialized;
using System.IO;
using Gallio.Common.Collections;
using Gallio.Common.Policies;
using Gallio.Model;
using Gallio.Model.Filters;
using Gallio.Runner;
using Gallio.Runner.Reports;
using Gallio.Runner.Reports.Schema;
using Gallio.Runtime;
using Gallio.Runtime.Logging;
using Gallio.Runtime.ProgressMonitoring;
using Gallio.TDNetRunner.Facade;
using Gallio.TDNetRunner.Properties;

namespace Gallio.TDNetRunner.Core
{
    public class RemoteProxyTestRunner : BaseProxyTestRunner
    {
        private const string ShortReportType = @"html-condensed";
        private const string LongReportType = @"html";
        private const int ShortReportThreshold = 100;

        private TestLauncher launcher;

        public RemoteProxyTestRunner()
        {
            launcher = new TestLauncher();
        }

        protected override void Dispose(bool disposing)
        {
            launcher = null;
        }

        protected override void AbortImpl()
        {
            if (launcher != null)
                launcher.Cancel();
        }

        protected override FacadeTestRunState RunImpl(IFacadeTestListener testListener, string assemblyPath, string cref, FacadeOptions facadeOptions)
        {
            if (cref == null)
                return RunAssembly(testListener, assemblyPath, facadeOptions);

            if (cref.Length >= 2)
            {
                char descriptor = cref[0];

                switch (descriptor)
                {
                    case 'T':
                        return RunType(testListener, assemblyPath, cref.Substring(2), facadeOptions);

                    case 'N':
                        return RunNamespace(testListener, assemblyPath, cref.Substring(2), facadeOptions);

                    case 'M':
                    case 'F':
                    case 'P':
                    case 'E':
                        int paramsPos = cref.IndexOf('(');
                        if (paramsPos < 0)
                            paramsPos = cref.Length;

                        string memberNameWithType = cref.Substring(2, paramsPos - 2);
                        int memberPos = memberNameWithType.LastIndexOf('.');
                        if (memberPos < 0)
                            break;

                        string typeName = memberNameWithType.Substring(0, memberPos);
                        string memberName = memberNameWithType.Substring(memberPos + 1);
                        return RunMember(testListener, assemblyPath, typeName, memberName, facadeOptions);
                }
            }

            return FacadeTestRunState.NoTests;
        }

        private FacadeTestRunState RunAssembly(IFacadeTestListener testListener, string assemblyPath, FacadeOptions facadeOptions)
        {
            return Run(testListener, assemblyPath, new AnyFilter<ITestDescriptor>(), facadeOptions);
        }

        private FacadeTestRunState RunNamespace(IFacadeTestListener testListener, string assemblyPath, string @namespace, FacadeOptions facadeOptions)
        {
            return Run(testListener, assemblyPath, new AndFilter<ITestDescriptor>(new Filter<ITestDescriptor>[]
            { 
                new NamespaceFilter<ITestDescriptor>(new EqualityFilter<string>(@namespace))
            }), facadeOptions);
        }

        private FacadeTestRunState RunType(IFacadeTestListener testListener, string assemblyPath, string typeName, FacadeOptions facadeOptions)
        {
            return Run(testListener, assemblyPath, new AndFilter<ITestDescriptor>(new Filter<ITestDescriptor>[]
            { 
                new TypeFilter<ITestDescriptor>(new EqualityFilter<string>(typeName), true)
            }), facadeOptions);
        }

        private FacadeTestRunState RunMember(IFacadeTestListener testListener, string assemblyPath, string typeName, string memberName, FacadeOptions facadeOptions)
        {
            return Run(testListener, assemblyPath, new AndFilter<ITestDescriptor>(new Filter<ITestDescriptor>[]
            { 
                new TypeFilter<ITestDescriptor>(new EqualityFilter<string>(typeName), true),
                new MemberFilter<ITestDescriptor>(new EqualityFilter<string>(memberName))
            }), facadeOptions);
        }

        public override object InitializeLifetimeService()
        {
            return null;
        }

        /// <summary>
        /// Provided so that the unit tests can override test execution behavior.
        /// </summary>
        internal virtual TestLauncherResult RunLauncher(TestLauncher launcher)
        {
            return launcher.Run();
        }

        private static Filter<ITestDescriptor> ToCategoryFilter(IList<string> categoryNames)
        {
            return new MetadataFilter<ITestDescriptor>(MetadataKeys.Category, new OrFilter<string>(GenericCollectionUtils.ConvertAllToArray(categoryNames,
                categoryName => new EqualityFilter<string>(categoryName))));
        }

        private FacadeTestRunState Run(IFacadeTestListener testListener, string assemblyPath, Filter<ITestDescriptor> filter, FacadeOptions facadeOptions)
        {
            if (testListener == null)
                throw new ArgumentNullException(@"testListener");
            if (assemblyPath == null)
                throw new ArgumentNullException("assemblyPath");
            if (facadeOptions == null)
                throw new ArgumentNullException("facadeOptions");

            var filterRules = new List<FilterRule<ITestDescriptor>>();
            switch (facadeOptions.FilterCategoryMode)
            {
                case FacadeFilterCategoryMode.Disabled:
                    filterRules.Add(new FilterRule<ITestDescriptor>(FilterRuleType.Inclusion, filter));
                    break;

                case FacadeFilterCategoryMode.Include:
                    filterRules.Add(new FilterRule<ITestDescriptor>(FilterRuleType.Inclusion,
                        new AndFilter<ITestDescriptor>(new[] { filter, ToCategoryFilter(facadeOptions.FilterCategoryNames) })));
                    break;

                case FacadeFilterCategoryMode.Exclude:
                    filterRules.Add(new FilterRule<ITestDescriptor>(FilterRuleType.Exclusion, ToCategoryFilter(facadeOptions.FilterCategoryNames)));
                    filterRules.Add(new FilterRule<ITestDescriptor>(FilterRuleType.Inclusion, filter));
                    break;
            }

            var filterSet = new FilterSet<ITestDescriptor>(filterRules);

            ILogger logger = new FilteredLogger(new TDNetLogger(testListener), LogSeverity.Info);

            launcher.Logger = logger;
            launcher.ProgressMonitorProvider = new LogProgressMonitorProvider(logger);
            launcher.TestExecutionOptions.FilterSet = filterSet;
            launcher.TestProject.TestRunnerFactoryName = StandardTestRunnerFactoryNames.IsolatedAppDomain;

            // This monitor will inform the user in real-time what's going on
            launcher.TestProject.AddTestRunnerExtension(new TDNetExtension(testListener));

            launcher.TestProject.TestPackage.AddFile(new FileInfo(assemblyPath));

            string assemblyDirectory = Path.GetDirectoryName(assemblyPath);
            //launcher.TestPackageConfig.ShadowCopy = true;
            launcher.TestProject.TestPackage.ApplicationBaseDirectory = new DirectoryInfo(assemblyDirectory);
            launcher.TestProject.TestPackage.WorkingDirectory = new DirectoryInfo(assemblyDirectory);

            TestLauncherResult result = RunLauncher(launcher);

            string reportDirectory = GetReportDirectory(logger);
            if (reportDirectory != null)
            {
                var reportFormatterOptions = new ReportFormatterOptions();
                result.GenerateReports(reportDirectory, Path.GetFileName(assemblyPath),
                    new[] {DetermineReportFormat(result.Report)}, reportFormatterOptions,
                    RuntimeAccessor.ServiceLocator.Resolve<IReportManager>(), NullProgressMonitor.CreateInstance());

                // This will generate a link to the generated report
                if (result.ReportDocumentPaths.Count != 0)
                {
                    Uri rawUrl = new Uri(result.ReportDocumentPaths[0]);
                    string displayUrl = "file:///" + rawUrl.LocalPath.Replace(" ", "%20").Replace(@"\", @"/");

                    // TDNet just prints the link on its own but it's not always clear to users what it represents.
                    // testListener.TestResultsUrl(displayUrl);

                    testListener.WriteLine("\nTest Report: " + displayUrl, FacadeCategory.Info);
                }
            }

            // Inform no tests run, if necessary.
            if (result.ResultCode == ResultCode.NoTests)
                InformNoTestsWereRun(testListener, Resources.MbUnitTestRunner_NoTestsFound);
            else if (result.Statistics.TestCount == 0)
                InformNoTestsWereRun(testListener, null);

            return GetTestRunState(result);
        }

        private static string DetermineReportFormat(Report report)
        {
            return report.TestPackageRun == null || report.TestPackageRun.Statistics.RunCount < ShortReportThreshold
                ? LongReportType : ShortReportType;
        }

        /// <summary>
        /// Gets a temporary folder to store the HTML report.
        /// </summary>
        /// <param name="logger">The logger</param>
        /// <returns>The full name of the folder or null if it could not be created.</returns>
        private static string GetReportDirectory(ILogger logger)
        {
            try
            {
                DirectoryInfo reportDirectory = new DirectoryInfo(Path.Combine(SpecialPathPolicy.For("TDNetRunner").GetTempDirectory().FullName, "Report"));
                if (reportDirectory.Exists)
                {
                    // Make sure the folder is empty
                    try
                    {
                        reportDirectory.Delete(true);
                    }
                    catch (IOException)
                    {
                        // If we cannot delete the directory (perhaps it is still in use), then
                        // create a new directory with a unique name.
                        reportDirectory = SpecialPathPolicy.For("TDNetRunner").CreateTempDirectoryWithUniqueName();
                    }
                }

                reportDirectory.Create();

                return reportDirectory.FullName;
            }
            catch (Exception e)
            {
                logger.Log(LogSeverity.Error, "Could not create the report directory.", e);
                return null;
            }
        }

        /// <summary>
        /// Inform the user that no tests were run and the reason for it. TD.NET displays
        /// a message like "0 Passed, 0 Failed, 0 Skipped" but it does it in the status bar,
        /// which may be harder to notice for the user. Be aware that this message will
        /// only be displayed when the user runs an individual test or fixture (TD.NET
        /// ignores the messages we send when it's running an entire assembly).
        /// </summary>
        /// <param name="testListener">An ITestListener object to write the message to.</param>
        /// <param name="reason">The reason no tests were run for.</param>
        private static void InformNoTestsWereRun(IFacadeTestListener testListener, string reason)
        {
            if (String.IsNullOrEmpty(reason))
                reason = @"";
            else
                reason = @" (" + reason + @")";

            string message = String.Format("** {0}{1} **", Resources.MbUnitTestRunner_NoTestsWereRun, reason);

            testListener.WriteLine(message, FacadeCategory.Warning);
        }

        private static FacadeTestRunState GetTestRunState(TestLauncherResult result)
        {
            switch (result.ResultCode)
            {
                default:
                    return FacadeTestRunState.Error;

                case ResultCode.Failure:
                case ResultCode.Canceled:
                    return FacadeTestRunState.Failure;

                case ResultCode.NoTests:
                case ResultCode.Success:
                    return FacadeTestRunState.Success;
            }
        }
    }
}
