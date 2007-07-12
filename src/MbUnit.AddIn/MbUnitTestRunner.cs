// QuickGraph Library 
// 
// Copyright (c) 2004 Jonathan de Halleux
//
// This software is provided 'as-is', without any express or implied warranty. 
// 
// In no event will the authors be held liable for any damages arising from 
// the use of this software.
// Permission is granted to anyone to use this software for any purpose, 
// including commercial applications, and to alter it and redistribute it 
// freely, subject to the following restrictions:
//
//		1. The origin of this software must not be misrepresented; 
//		you must not claim that you wrote the original software. 
//		If you use this software in a product, an acknowledgment in the product 
//		documentation would be appreciated but is not required.
//
//		2. Altered source versions must be plainly marked as such, and must 
//		not be misrepresented as being the original software.
//
//		3. This notice may not be removed or altered from any source 
//		distribution.
//		
//		QuickGraph Library HomePage: http://mbunit.tigris.org
//		Author: Jonathan de Halleux

using System;
using System.IO;
using System.Reflection;

using TestDriven.Framework;
using TDF = TestDriven.Framework;

#if false

namespace MbUnit.AddIn
{
	/// <summary>
	/// MbUnit test runner for TestDriven.Net.
	/// </summary>
	[Serializable]
	public class MbUnitTestRunner : ITestRunner
	{
        protected const string AppPathRootName = "MbUnit";
        protected const string AppPathReportsName = "Reports";

        private ITestListener testListener = null;
        private int assemblySetUpCount = 0;
        private int assemblyTearDownCount = 0;
        private int testFixtureSetUpCount = 0;
        private int testFixtureTearDownCount = 0;
        private int failureCount = 0;
        private int successCount = 0;
        private int ignoreCount = 0;
        private int testCount = 0;
        private int skipCount = 0;

        public TestRunState RunAssembly(ITestListener testListener, Assembly assembly)
        {
            AnyFixtureFilter filter = new AnyFixtureFilter();
            return Run(testListener, assembly, filter);
        }

        public TestRunState RunNamespace(ITestListener testListener, Assembly assembly, string ns)
        {
            NamespaceFixtureFilter filter = new NamespaceFixtureFilter(ns);
            return Run(testListener, assembly, filter);
        }

        public TestRunState RunMember(ITestListener testListener, Assembly assembly, MemberInfo member)
        {
            Type type = member as Type;
            if (type != null)
            {
                TypeFixtureFilter filter = new TypeFixtureFilter(type.FullName);
                TypeFilterBase typeFilter = TypeFilters.Type(type.FullName);
                return Run(testListener, assembly, filter, new AnyRunPipeFilter(), typeFilter);
            }
            else
            {
                TypeFixtureFilter filter = new TypeFixtureFilter(member.DeclaringType.FullName);
                TypeFilterBase typeFilter = TypeFilters.Type(member.DeclaringType.FullName);
                ContainsMemberRunPipeFilter runPipeFilter = new ContainsMemberRunPipeFilter(member);

                return Run(testListener, assembly, filter, runPipeFilter, typeFilter);
            }
        }

        protected TestRunState Run(
            ITestListener testListener, 
            Assembly assembly, 
            IFixtureFilter filter
            )
        {
            return Run(testListener,assembly,filter, new AnyRunPipeFilter(), TypeFilters.Any);
        }

        /// <summary> 
        /// This method is used to provide assembly location resolver. It is called on event as needed by the CLR. 
        /// Refer to document related to AppDomain.CurrentDomain.AssemblyResolve 
        /// </summary> 
        private Assembly AssemblyResolveHandler(object sender, ResolveEventArgs e)
        {
            try
            {
                string[] assemblyDetail = e.Name.Split(',');
                string assemblyBasePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                Assembly assembly = Assembly.LoadFrom(assemblyBasePath + @"\" + assemblyDetail[0] + ".dll");
                return assembly;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed resolving assembly", ex);
            }
        } 

        protected TestRunState Run(
            ITestListener testListener, 
            Assembly assembly, 
            IFixtureFilter filter,
            RunPipeFilterBase runPipeFilter,
            TypeFilterBase typeFilter
            )
        {
            this.testListener = testListener;

            assemblySetUpCount = 0;
            assemblyTearDownCount = 0;
            testFixtureSetUpCount = 0;
            testFixtureTearDownCount = 0;
            failureCount = 0;
            successCount = 0;
            ignoreCount = 0;
            skipCount = 0;

            string assemblyPath = new Uri(assembly.CodeBase).LocalPath;
            testListener.WriteLine("Starting the MbUnit Test Execution", Category.Info);
			testListener.WriteLine("Exploring " + assembly.FullName, Category.Info);
            testListener.WriteLine(String.Format("MbUnit {0} Addin", typeof(RunPipe).Assembly.GetName().Version),Category.Info);

            try
			{
                using (AssemblyTestDomain domain = new AssemblyTestDomain(assembly))
                {
                    //define an assembly resolver routine in case the CLR cannot find our assemblies. 
                    AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyResolveHandler);
            
                    domain.TypeFilter = typeFilter;
                    domain.Filter = filter;
                    domain.RunPipeFilter = runPipeFilter;
                    domain.Load();
                    // check found tests
                    testCount = domain.TestEngine.GetTestCount().RunCount;
                    if (testCount==0)
                    {
                        testListener.WriteLine("No tests found",Category.Info);
                        return TestRunState.NoTests;
                    }

                    testListener.WriteLine(String.Format("Found {0} tests", testCount),Category.Info);
                    // add listeners
                    domain.TestEngine.FixtureRunner.AssemblySetUp+=new ReportSetUpAndTearDownEventHandler(TestEngine_AssemblySetUp);
                    domain.TestEngine.FixtureRunner.AssemblyTearDown += new ReportSetUpAndTearDownEventHandler(TestEngine_AssemblyTearDown);
                    domain.TestEngine.FixtureRunner.TestFixtureSetUp += new ReportSetUpAndTearDownEventHandler(TestEngine_TestFixtureSetUp);
                    domain.TestEngine.FixtureRunner.TestFixtureTearDown += new ReportSetUpAndTearDownEventHandler(TestEngine_TestFixtureTearDown);
                    domain.TestEngine.FixtureRunner.RunResult += new ReportRunEventHandler(TestEngine_RunResult);

                    try
                    {
                        domain.TestEngine.RunPipes();
                    }
                    finally
                    {
                        if (domain.TestEngine != null)
                        {
                            domain.TestEngine.FixtureRunner.AssemblySetUp -= new ReportSetUpAndTearDownEventHandler(TestEngine_AssemblySetUp);
                            domain.TestEngine.FixtureRunner.AssemblyTearDown -= new ReportSetUpAndTearDownEventHandler(TestEngine_AssemblyTearDown);
                            domain.TestEngine.FixtureRunner.TestFixtureSetUp -= new ReportSetUpAndTearDownEventHandler(TestEngine_TestFixtureSetUp);
                            domain.TestEngine.FixtureRunner.TestFixtureTearDown -= new ReportSetUpAndTearDownEventHandler(TestEngine_TestFixtureTearDown);
                            domain.TestEngine.FixtureRunner.RunResult -= new ReportRunEventHandler(TestEngine_RunResult);
                        }
                    }

                    testListener.WriteLine("[reports] generating HTML report",Category.Info);
                    this.GenerateReports(testListener, assembly, domain.TestEngine.Report.Result);

                    return toTestRunState(domain.TestEngine.Report.Result);
                }
            }
			catch(Exception ex)
			{
                testListener.WriteLine("[critical-failure]",Category.Error);
                testListener.WriteLine(ex.ToString(),Category.Error);
                throw new Exception("Test execution failed", ex);
            }
		}

        static TestRunState toTestRunState(ReportResult reportResult)
        {
            if (reportResult.Counter.FailureCount > 0)
            {
                return TestRunState.Failure;
            }

            if (reportResult.Counter.RunCount > 0)
            {
                return TestRunState.Success;
            }

            return TestRunState.NoTests;
        }

        private void GenerateReports(ITestListener testListener, Assembly assembly,ReportResult result)
        {
            try
            {
                string outputPath = GetAppDataPath("");
                string nameFormat = assembly.GetName().Name + ".Tests";

                string file = HtmlReport.RenderToHtml(result, outputPath, nameFormat);

                if (file != "")
                {
                    Uri uri = new Uri("file:" + Path.GetFullPath(file));
                    testListener.TestResultsUrl(uri.AbsoluteUri);
                }
                else
                {
                    testListener.WriteLine("Skipping report generation", Category.Info);
                }
            }
            catch (Exception ex)
            {
                testListener.WriteLine("failed to create reports",Category.Error);
                testListener.WriteLine(ex.ToString(), Category.Error);
            }
        }

        private static string GetAppDataPath(string outputPath)
        {
            if (outputPath == null || outputPath.Length == 0)
            {
                string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                outputPath = Path.Combine(appDataPath, AppPathRootName + @"\" + AppPathReportsName);
            }
            return outputPath;
        }

        private static void DirectoryCheckCreate(string outputPath)
        {
            if (!Directory.Exists(outputPath))
                Directory.CreateDirectory(outputPath);
        }

        void RenderSetUpOrTearDownFailure(string context, ReportSetUpAndTearDown setup)
        {
            TestResult summary = new TestResult();
            switch (setup.Result)
            {
                case ReportRunResult.Failure:
                    summary.State = TDF.TestState.Failed;
                    break;
                case ReportRunResult.Success:
                    summary.State = TDF.TestState.Passed;
                    break;
            }

            summary.TotalTests = this.testCount;
            summary.Name = String.Format("{0} {1}",context, setup.Name);
            summary.TimeSpan = new TimeSpan(0, 0, 0, 0, (int)(setup.Duration * 1000));
            if (setup.Exception != null)
            {
                summary.Message = setup.Exception.Message;

                StringWriter sw = new StringWriter();
                ReportException ex = setup.Exception;
                summary.StackTrace = ex.ToString();
            }

            this.testListener.TestFinished(summary);
        }

        void TestEngine_AssemblySetUp(object sender, ReportSetUpAndTearDownEventArgs e)
        {
            this.assemblySetUpCount++;
            if (e.SetUpAndTearDown.Result != ReportRunResult.Success)
                RenderSetUpOrTearDownFailure("[assembly-setup]", e.SetUpAndTearDown);
            else
                testListener.WriteLine("[assembly-setup] success", Category.Info);
        }

        void TestEngine_AssemblyTearDown(object sender, ReportSetUpAndTearDownEventArgs e)
        {
            this.assemblyTearDownCount++;
            if (e.SetUpAndTearDown.Result != ReportRunResult.Success)
                RenderSetUpOrTearDownFailure("[assembly-teardown]", e.SetUpAndTearDown);
            else
                testListener.WriteLine("[assembly-teardown] success", Category.Info);
        }

        void TestEngine_TestFixtureSetUp(object sender, ReportSetUpAndTearDownEventArgs e)
        {
            this.testFixtureSetUpCount++;
            if (e.SetUpAndTearDown.Result != ReportRunResult.Success)
                RenderSetUpOrTearDownFailure("[fixture-setup]", e.SetUpAndTearDown);
            else
                testListener.WriteLine("[fixture-setup] success", Category.Info);
        }

        void TestEngine_TestFixtureTearDown(object sender, ReportSetUpAndTearDownEventArgs e)
        {
            this.testFixtureTearDownCount++;
            if (e.SetUpAndTearDown.Result != ReportRunResult.Success)
                RenderSetUpOrTearDownFailure("[fixture-teardown]", e.SetUpAndTearDown);
            else
                testListener.WriteLine("[fixture-teardown] success", Category.Info);
        }

        void TestEngineSuccess(ReportRun run)
        {
            this.successCount++;

            TestResult summary = new TestResult();
            summary.State = TDF.TestState.Passed;
            summary.TotalTests = this.testCount;
            summary.Name = run.Name;
            summary.TimeSpan = new TimeSpan(0, 0, 0, 0, (int)(run.Duration * 1000));

            testListener.WriteLine(String.Format("[success] {0}", run.Name), Category.Info);
            this.testListener.TestFinished(summary);
        }

        void TestEngineFailure(ReportRun run)
        {
            this.failureCount++;

            TestResult summary = new TestResult();
            summary.State = TDF.TestState.Failed;
            summary.TotalTests = this.testCount;
            summary.Name = run.Name;
            summary.TimeSpan = new TimeSpan(0, 0, 0, 0, (int)(run.Duration * 1000));

            if (run.Exception != null)
            {
                ReportException ex = run.Exception;

                summary.Message = run.Exception.Message;
                summary.StackTrace = ex.ToString();
            }

            testListener.WriteLine(String.Format("[failure] {0}", run.Name), Category.Info);
            this.testListener.TestFinished(summary);
        }

        void TestEngineSkip(ReportRun run)
        {
            this.skipCount++;

            TestResult summary = new TestResult();
            summary.State = TDF.TestState.Ignored;
            summary.TotalTests = this.testCount;
            summary.Name = run.Name;
            summary.TimeSpan = new TimeSpan(0, 0, 0, 0, 0);

            if (run.Exception != null)
            {
                ReportException ex = run.Exception;

                summary.Message = run.Exception.Message;
                summary.StackTrace = ex.ToString();
            }

            testListener.WriteLine(String.Format("[skipped] {0}", run.Name), Category.Info);
            this.testListener.TestFinished(summary);
        }

        void TestEngineIgnore(ReportRun run)
        {
            this.ignoreCount++;

            TestResult summary = new TestResult();
            summary.State = TDF.TestState.Ignored;
            summary.TotalTests = this.testCount;
            summary.Name = run.Name;
            summary.TimeSpan = new TimeSpan(0, 0, 0, 0, (int)(run.Duration * 1000));

            testListener.WriteLine(String.Format("[ignored] {0}", run.Name), Category.Info);
            this.testListener.TestFinished(summary);
        }

        void TestEngine_RunResult(object sender, ReportRunEventArgs e)
        {
            switch (e.Run.Result)
            {
                case ReportRunResult.Success:
                    TestEngineSuccess(e.Run);
                    break;
                case ReportRunResult.Failure:
                    TestEngineFailure(e.Run);
                    break;
                case ReportRunResult.Ignore:
                    TestEngineIgnore(e.Run);
                    break;
                case ReportRunResult.Skip:
                    TestEngineSkip(e.Run);
                    break;
            }
        }
    }
}

#endif