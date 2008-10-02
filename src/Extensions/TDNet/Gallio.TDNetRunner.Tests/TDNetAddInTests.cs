// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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
using System.Reflection;
using Gallio.Runtime.Logging;
using Gallio.Runtime.ProgressMonitoring;
using Gallio.Model;
using Gallio.Model.Filters;
using Gallio.Reflection;
using Gallio.Runner;
using Gallio.Runner.Reports;
using MbUnit.Framework;
using Rhino.Mocks;
using TestDriven.TestRunner.Framework;

namespace Gallio.TDNetRunner.Tests
{
    // TODO: Verify logged output
    [TestFixture]
    [TestsOn(typeof(StubbedTestRunner))]
    public class TDNetAddInTests
    {
        [Test]
        public void RunThrowsWhenTestListenerIsNull()
        {
            StubbedTestRunner tr = new StubbedTestRunner();
            Assert.Throws<ArgumentNullException>(() => tr.Run(null, MockRepository.GenerateStub<ITraceListener>(), "foo", "bar"));
        }

        [Test]
        public void RunThrowsWhenTraceListenerIsNull()
        {
            StubbedTestRunner tr = new StubbedTestRunner();
            Assert.Throws<ArgumentNullException>(() => tr.Run(MockRepository.GenerateStub<ITestListener>(), null, "foo", "bar"));
        }

        [Test]
        public void RunThrowsWhenAssemblyPathIsNull()
        {
            StubbedTestRunner tr = new StubbedTestRunner();
            Assert.Throws<ArgumentNullException>(() => tr.Run(MockRepository.GenerateStub<ITestListener>(), MockRepository.GenerateStub<ITraceListener>(), null, "bar"));
        }

        [Test]
        public void RunAssemblyPassesCorrectOptionsToTheLauncher()
        {
            StubbedTestRunner tr = new StubbedTestRunner();

            Assembly assembly = typeof(TDNetAddInTests).Assembly;
            string assemblyPath = AssemblyUtils.GetAssemblyLocalPath(assembly);

            tr.SetRunLauncherAction(delegate(TestLauncher launcher)
            {
                AssertTestLauncherOptions(launcher, assemblyPath, new AnyFilter<ITest>());

                return new TestLauncherResult(new Report());
            });

            tr.Run(MockRepository.GenerateStub<ITestListener>(), MockRepository.GenerateStub<ITraceListener>(), assemblyPath, null);
        }

        [Test]
        public void RunMemberWithTypePassesCorrectOptionsToTheLauncher()
        {
            StubbedTestRunner tr = new StubbedTestRunner();

            Type type = typeof(TDNetAddInTests);
            Assembly assembly = type.Assembly;
            string assemblyPath = AssemblyUtils.GetAssemblyLocalPath(assembly);

            tr.SetRunLauncherAction(delegate(TestLauncher launcher)
            {
                AssertTestLauncherOptions(launcher, assemblyPath,
                    new TypeFilter<ITest>(new EqualityFilter<string>(type.FullName), true));

                return new TestLauncherResult(new Report());
            });

            tr.Run(MockRepository.GenerateStub<ITestListener>(), MockRepository.GenerateStub<ITraceListener>(), assemblyPath, "T:" + type.FullName);
        }

        [Test]
        public void RunMemberWithMethodPassesCorrectOptionsToTheLauncher()
        {
            StubbedTestRunner tr = new StubbedTestRunner();

            MethodBase method = Reflector.GetExecutingFunction().Resolve(true);
            Assembly assembly = method.DeclaringType.Assembly;
            string assemblyPath = AssemblyUtils.GetAssemblyLocalPath(assembly);

            tr.SetRunLauncherAction(delegate(TestLauncher launcher)
            {
                AssertTestLauncherOptions(launcher, AssemblyUtils.GetAssemblyLocalPath(assembly),
                    new AndFilter<ITest>(new Filter<ITest>[] {
                        new TypeFilter<ITest>(new EqualityFilter<string>(method.DeclaringType.FullName), true),
                        new MemberFilter<ITest>(new EqualityFilter<string>(method.Name))
                    }));

                return new TestLauncherResult(new Report());
            });

            tr.Run(MockRepository.GenerateStub<ITestListener>(), MockRepository.GenerateStub<ITraceListener>(), assemblyPath,
                "M:" + method.DeclaringType.FullName + "." + method.Name);
        }

        [Test]
        public void RunNamespacePassesCorrectOptionsToTheLauncher()
        {
            StubbedTestRunner tr = new StubbedTestRunner();

            Assembly assembly = typeof(TDNetAddInTests).Assembly;
            string assemblyPath = AssemblyUtils.GetAssemblyLocalPath(assembly);
            string @namespace = "Foo";

            tr.SetRunLauncherAction(delegate(TestLauncher launcher)
            {
                AssertTestLauncherOptions(launcher, AssemblyUtils.GetAssemblyLocalPath(assembly),
                    new NamespaceFilter<ITest>(new EqualityFilter<string>(@namespace)));

                return new TestLauncherResult(new Report());
            });

            tr.Run(MockRepository.GenerateStub<ITestListener>(), MockRepository.GenerateStub<ITraceListener>(), assemblyPath, "N:" + @namespace);
        }

        [Test]
        [Row(ResultCode.Canceled, true, false, true)]
        [Row(ResultCode.Failure, true, false, true)]
        [Row(ResultCode.FatalException, true, false, true)]
        [Row(ResultCode.InvalidArguments, true, false, true)]
        [Row(ResultCode.NoTests, true, true, false)]
        [Row(ResultCode.Success, true, true, false)]
        public void RunReturnsCorrectResultCode(int resultCode, bool expectedExecuted, bool expectedSuccess, bool expectedFailure)
        {
            StubbedTestRunner tr = new StubbedTestRunner();

            Assembly assembly = typeof(TDNetAddInTests).Assembly;
            string assemblyPath = AssemblyUtils.GetAssemblyLocalPath(assembly);
            tr.SetRunLauncherAction(delegate
            {
                TestLauncherResult result = new TestLauncherResult(new Report());
                result.SetResultCode(resultCode);
                return result;
            });

            TestRunResult runResult = tr.Run(MockRepository.GenerateStub<ITestListener>(), MockRepository.GenerateStub<ITraceListener>(), assemblyPath, null);
            Assert.AreEqual(expectedExecuted, runResult.IsExecuted);
            Assert.AreEqual(expectedSuccess, runResult.IsSuccess);
            Assert.AreEqual(expectedFailure, runResult.IsFailure);
        }

        private static void AssertTestLauncherOptions(TestLauncher launcher, string assemblyFile, Filter<ITest> filter)
        {
            Assert.IsFalse(launcher.DoNotRun);
            Assert.IsFalse(launcher.EchoResults);
            Assert.AreEqual(filter.ToFilterExpr(), launcher.TestExecutionOptions.Filter.ToFilterExpr());
            Assert.IsInstanceOfType(typeof(FilteredLogger), launcher.Logger);
            Assert.IsInstanceOfType(typeof(LogProgressMonitorProvider), launcher.ProgressMonitorProvider);
            Assert.AreEqual(Path.Combine(Path.GetTempPath(), @"Gallio.TDNetRunner"), launcher.ReportDirectory);
            Assert.AreEqual(0, launcher.ReportFormatOptions.Count);
            Assert.AreElementsEqual(new string[] { "html" }, launcher.ReportFormats);
            Assert.AreEqual(Path.GetFileName(assemblyFile), launcher.ReportNameFormat);
            Assert.IsFalse(launcher.ShowReports);
            Assert.AreEqual(StandardTestRunnerFactoryNames.Local, launcher.TestRunnerFactoryName);

            Assert.IsNull(launcher.RuntimeSetup);

            Assert.AreElementsEqual(new string[] { assemblyFile }, launcher.TestPackageConfig.AssemblyFiles);
            Assert.AreElementsEqual(new string[] { }, launcher.TestPackageConfig.HintDirectories);

            Assert.AreEqual(Path.GetDirectoryName(assemblyFile), launcher.TestPackageConfig.HostSetup.ApplicationBaseDirectory);
            Assert.IsFalse(launcher.TestPackageConfig.HostSetup.ShadowCopy);
            Assert.AreEqual(Path.GetDirectoryName(assemblyFile), launcher.TestPackageConfig.HostSetup.WorkingDirectory);
        }
    }
}
