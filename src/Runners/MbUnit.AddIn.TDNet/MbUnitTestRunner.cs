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
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Castle.Core.Logging;
using MbUnit.AddIn.TDNet.Properties;
using MbUnit.Core.Runner;
using MbUnit.Model.Filters;
using MbUnit.Model;
using TestDriven.Framework;
using TDF = TestDriven.Framework;

namespace MbUnit.AddIn.TDNet
{
    /// <summary>
    /// MbUnit test runner for TestDriven.NET.
    /// </summary>
    [Serializable]
    public class MbUnitTestRunner : TDF.ITestRunner
    {
        private readonly string reportType = @"html";

        #region TDF.ITestRunner Members

        /// <summary>
        /// TD.NET calls this method when you run an entire assemby (by right-clicking
        /// in a project an selecting "Run Test(s)")
        /// </summary>
        TestRunState TDF.ITestRunner.RunAssembly(ITestListener testListener, Assembly assembly)
        {
            if (assembly == null)
                throw new ArgumentNullException(@"assembly");

            return Run(testListener, assembly, new AssemblyFilter<ITest>(assembly.FullName));
        }

        /// <summary>
        /// TD.NET calls this method when you run either all the tests in a fixture or
        /// an individual test.
        /// </summary>
        TestRunState TDF.ITestRunner.RunMember(ITestListener testListener, Assembly assembly, MemberInfo member)
        {
            if (assembly == null)
                throw new ArgumentNullException(@"assembly");
            if (member == null)
                throw new ArgumentNullException(@"member");

            List<Filter<ITest>> filters = new List<Filter<ITest>>();
            filters.Add(new AssemblyFilter<ITest>(assembly.FullName));
            switch (member.MemberType)
            {
                case MemberTypes.TypeInfo:
                    Type type = (Type)member;
                    // FIXME: Should we always include derived types?
                    filters.Add(new TypeFilter<ITest>(type.FullName, true));
                    break;
                case MemberTypes.Method:
                    MethodInfo methodInfo = (MethodInfo)member;
                    // We look for the declaring type so we can also use a TypeFilter
                    // to avoid ambiguity
                    Type declaringType = methodInfo.DeclaringType;
                    // FIXME: Should we always include derived types?
                    filters.Add(new TypeFilter<ITest>(declaringType.FullName, false));
                    filters.Add(new MemberFilter<ITest>(member.Name));
                    break;
                default:
                    // This is not something we can run so just ignore it
                    InformNoTestsWereRun(testListener, String.Format(Resources.MbUnitTestRunner_MemberIsNotATest, member.Name));
                    return TestRunState.NoTests;
            }

            return Run(testListener, assembly, new AndFilter<ITest>(filters.ToArray()));
        }

        /// <summary>
        /// It appears this method never gets called.
        /// </summary>
        TestRunState TDF.ITestRunner.RunNamespace(ITestListener testListener, Assembly assembly, string ns)
        {
            if (assembly == null)
                throw new ArgumentNullException(@"assembly");
            if (String.IsNullOrEmpty(ns))
                throw new ArgumentNullException(@"ns");

            List<Filter<ITest>> filters = new List<Filter<ITest>>();
            filters.Add(new AssemblyFilter<ITest>(assembly.FullName));
            filters.Add(new NamespaceFilter<ITest>(ns));

            return Run(testListener, assembly, new AndFilter<ITest>(filters.ToArray()));
        }

        #endregion

        #region Private Methods

        private TestRunState Run(ITestListener testListener, Assembly assembly, Filter<ITest> filter)
        {
            if (testListener == null)
                throw new ArgumentNullException(@"testListener");
            if (filter == null)
                throw new ArgumentNullException(@"filter");

            TDNetLogger logger = new TDNetLogger(testListener);
            LogAddInVersion(logger);

            using (TestRunnerHelper testRunnerHelper = new TestRunnerHelper(
                new LogProgressMonitorProvider(logger),
                logger))
            {
                testRunnerHelper.Filter = filter;

                // This monitor will inform the user in real-time what's going on
                testRunnerHelper.CustomMonitors.Add(new TDNetLogMonitor(testListener, testRunnerHelper.ReportMonitor));

                testRunnerHelper.Package.EnableShadowCopy = true;

                string location = new Uri(assembly.CodeBase).LocalPath;
                testRunnerHelper.Package.AssemblyFiles.Add(location);

                testRunnerHelper.ReportFormats.Add(reportType);
                testRunnerHelper.ReportNameFormat = Path.GetFileName(location);
                testRunnerHelper.ReportDirectory = GetReportDirectory(logger);
                if (String.IsNullOrEmpty(testRunnerHelper.ReportDirectory))
                {
                    return TestRunState.Failure;
                }

                int result = testRunnerHelper.Run();

                // This will generate a link to the generated report
                Uri uri = new Uri(testRunnerHelper.GetReportFilename(reportType)); // yes, this really works.
                testListener.TestResultsUrl(uri.AbsoluteUri);

                return GetTDNetResult(testRunnerHelper, testListener, result);
            }
        }

        /// <summary>
        /// Gets a temporary folder to store the HTML report.
        /// </summary>
        /// <param name="logger">The logger</param>
        /// <returns>The full name of the folder or null if it could not be created.</returns>
        private string GetReportDirectory(ILogger logger)
        {
            try
            {
                DirectoryInfo reportDirectory = new DirectoryInfo(Path.Combine(Path.GetTempPath(), @"MbUnit-TD-AddIn"));
                if (reportDirectory.Exists)
                {
                    // Make sure the folder is empty
                    reportDirectory.Delete(true);
                }

                reportDirectory.Create();

                return reportDirectory.FullName;
            }
            catch (Exception e)
            {
                logger.Error("Could not create the report directory.", e);
                return null;
            }
        }

        /// <summary>
        /// Translates the test execution result into something understandable
        /// for TDNet.
        /// </summary>
        /// <param name="testRunnerHelper">The TestRunnerHelper instance that was used to run the tests.</param>
        /// <param name="testListener">An ITestListener object to write the message to.</param>
        /// <param name="result">The result code given by MbUnit.</param>
        /// <returns>The TestRunState value that should be returned to TDNet.</returns>
        private static TestRunState GetTDNetResult(TestRunnerHelper testRunnerHelper, ITestListener testListener, int result)
        {
            TestRunState state;

            switch (result)
            {
                case ResultCode.FatalException:
                case ResultCode.InvalidArguments:
                    state = TestRunState.Error;
                    break;
                case ResultCode.Failure:
                    state = TestRunState.Failure;
                    break;
                case ResultCode.NoTests:
                    state = TestRunState.NoTests;
                    InformNoTestsWereRun(testListener, Resources.MbUnitTestRunner_NoTestsFound);
                    break;
                default:
                    state = TestRunState.Success;
                    break;
            }

            //TODO: Check if TestCount may be 0 for another reason?
            if (testRunnerHelper.Statistics.TestCount == 0)
            {
                state = TestRunState.NoTests;
                InformNoTestsWereRun(testListener, String.Empty);
            }

            return state;
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
        private static void InformNoTestsWereRun(ITestListener testListener, string reason)
        {
            string message = String.Format("** {0}", Resources.MbUnitTestRunner_NoTestsWereRun);
            if (!String.IsNullOrEmpty(reason))
            {
                message += " (" + reason + ")";
            }
            message += " **";
            testListener.WriteLine(message, Category.Warning);
        }

        private void LogAddInVersion(ILogger logger)
        {
            Version appVersion = Assembly.GetCallingAssembly().GetName().Version;
            logger.Info(String.Format(Resources.RunnerNameAndVersion + "\n",
                appVersion.Major, appVersion.Minor, appVersion.Build));
        }

        #endregion
    }
}