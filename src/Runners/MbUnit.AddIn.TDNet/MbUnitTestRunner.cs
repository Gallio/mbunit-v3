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
using MbUnit.Core.Runner;
using MbUnit.Framework.Kernel.Filters;
using MbUnit.Framework.Kernel.Model;
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
        private readonly TestResult testResult = new TestResult();
        private readonly string reportType = "html";
        private TDNetLogger logger = null;

        #region TDF.ITestRunner Members

        /// <summary>
        /// TD.NET calls this method when you run an entire assemby (by right-clicking
        /// in a project an selecting "Run Test(s)")
        /// </summary>
        TestRunState TDF.ITestRunner.RunAssembly(ITestListener testListener, Assembly assembly)
        {
            if (assembly == null)
                throw new ArgumentNullException("assembly");

            testResult.Name = assembly.FullName;
            return Run(testListener, assembly, new AssemblyFilter<ITest>(assembly.FullName));
        }

        /// <summary>
        /// TD.NET calls this method when you run either all the tests in a fixture or
        /// an individual test.
        /// </summary>
        TestRunState TDF.ITestRunner.RunMember(ITestListener testListener, Assembly assembly, MemberInfo member)
        {
            if (assembly == null)
                throw new ArgumentNullException("assembly");
            if (member == null)
                throw new ArgumentNullException("member");

            List<Filter<ITest>> filters = new List<Filter<ITest>>();
            filters.Add(new AssemblyFilter<ITest>(assembly.FullName));
            switch (member.MemberType)
            {
                case MemberTypes.TypeInfo:
                    Type type = (Type)member;
                    // FIXME: Should we always include derived types?
                    filters.Add(new TypeFilter<ITest>(type.FullName, true));
                    testResult.Name = type.FullName;
                    break;
                case MemberTypes.Method:
                    MethodInfo methodInfo = (MethodInfo)member;
                    // We look for the declaring type so we can also use a TypeFilter
                    // to avoid ambiguity
                    Type declaringType = methodInfo.DeclaringType;
                    // FIXME: Should we always include derived types?
                    filters.Add(new TypeFilter<ITest>(declaringType.FullName, false));
                    filters.Add(new MemberFilter<ITest>(member.Name));
                    testResult.Name = declaringType.FullName + "::" + member.Name;
                    break;
                default:
                    // This is not something we can run so just ignored it
                    testResult.State = TestState.Ignored;
                    testListener.TestFinished(testResult);
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
                throw new ArgumentNullException("assembly");
            if (String.IsNullOrEmpty(ns))
                throw new ArgumentNullException("ns");

            List<Filter<ITest>> filters = new List<Filter<ITest>>();
            filters.Add(new AssemblyFilter<ITest>(assembly.FullName));
            filters.Add(new NamespaceFilter<ITest>(ns));

            return Run(testListener, assembly, new AndFilter<ITest>(filters.ToArray()));
        }

        #endregion

        #region Private Methods

        private TestRunState Run(ITestListener listener, Assembly assembly, Filter<ITest> filter)
        {
            if (listener == null)
                throw new ArgumentNullException("listener");
            if (filter == null)
                throw new ArgumentNullException("filter");

            logger = new TDNetLogger(listener);
            LogAddInVersion();

            using (TestRunnerHelper testRunnerHelper = new TestRunnerHelper
                (
                delegate { return new RunnerProgressMonitor(logger); },
                logger,
                filter
                ))
            {
                string location = new Uri(assembly.CodeBase).LocalPath;
                testRunnerHelper.Package.AssemblyFiles.Add(location);

                testRunnerHelper.ReportFormats.Add(reportType);
                testRunnerHelper.ReportNameFormat = Path.GetFileName(location);
                testRunnerHelper.ReportDirectory = GetReportDirectory();
                if (String.IsNullOrEmpty(testRunnerHelper.ReportDirectory))
                {
                    return TestRunState.Failure;
                }

                int result = testRunnerHelper.Run();

                // This will generate a link to the generated report
                Uri uri = new Uri("file:" + testRunnerHelper.GetReportFileName(reportType).Replace(@"\", "/"));
                listener.TestResultsUrl(uri.AbsoluteUri);

                return GetTDNetResult(testRunnerHelper, listener, result);
            }
        }

        /// <summary>
        /// Gets a temporary folder to store the HTML report.
        /// </summary>
        /// <returns>The full name of the folder or null if it could not be created.</returns>
        private string GetReportDirectory()
        {
            try
            {
                DirectoryInfo reportDirectory =
                    new DirectoryInfo(Path.GetTempPath() + @"\MbUnit-TD-AddIn\");
                if (reportDirectory.Exists)
                {
                    // Make sure the folder is empty
                    reportDirectory.Delete(true);
                }
                reportDirectory.Create();

                return reportDirectory.FullName;
            }
            catch(Exception e)
            {
                testResult.Message = e.Message;
                testResult.StackTrace = e.StackTrace;
                return null;
            }
        }

        /// <summary>
        /// Translates the test execution result into something understandable
        /// for TDNet.
        /// </summary>
        /// <param name="testRunnerHelper">The TestRunnerHelper instance that was used to run the tests.</param>
        /// <param name="listener">The ITestListener object that TDNet used to call us.</param>
        /// <param name="result">The result code given by MbUnit.</param>
        /// <returns>The TestRunState value that should be returned to TDNet.</returns>
        private TestRunState GetTDNetResult(TestRunnerHelper testRunnerHelper, ITestListener listener, int result)
        {
            TestRunState state;

            switch (result)
            {
                case ResultCode.FatalException:
                case ResultCode.InvalidArguments:
                    state = TestRunState.Error;
                    testResult.State = TestState.Failed;
                    break;
                case ResultCode.Failure:
                    state = TestRunState.Failure;
                    testResult.State = TestState.Failed;
                    break;
                case ResultCode.NoTests:
                    state = TestRunState.NoTests;
                    testResult.State = TestState.Ignored;
                    break;
                default:
                    state = TestRunState.Success;
                    testResult.State = TestState.Passed;
                    testResult.Message = "All Passed!";
                    break;
            }

            if (testRunnerHelper.Statistics.FailureCount > 0)
            {
                //TODO: Use the stack trace of the failing tests
                testResult.Message = String.Format("{0} test(s) failed.", testRunnerHelper.Statistics.FailureCount);
            }
            else
            {
                testResult.Message = testRunnerHelper.ResultSummary;
            }
            listener.TestFinished(testResult);
            
            return state;
        }

        private void LogAddInVersion()
        {
            Version appVersion = Assembly.GetCallingAssembly().GetName().Version;
            logger.Info(String.Format("MbUnit.AddIn.TDNet - Version {0}.{1} build {2}", appVersion.Major, appVersion.Minor, appVersion.Build));
        }

        #endregion
    }
}