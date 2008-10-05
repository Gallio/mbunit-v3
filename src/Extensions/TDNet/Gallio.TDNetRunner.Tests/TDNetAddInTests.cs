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

//#define RESIDENT

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
using TestDriven.Framework;
using TestDriven.Framework.Resident;

namespace Gallio.TDNetRunner.Tests
{
    // TODO: Verify logged output
    [TestFixture]
    [TestsOn(typeof(StubbedTestRunner))]
    public class TDNetAddInTests
    {
        [Test]
        public void RunAssemblyThrowsWhenTestListenerIsNull()
        {
            TestDriven.Framework.ITestRunner tr = new StubbedTestRunner();
            Assert.Throws<ArgumentNullException>(() => tr.RunAssembly(null, GetType().Assembly));
        }

        [Test]
        public void RunAssemblyThrowsWhenAssemblyIsNull()
        {
            TestDriven.Framework.ITestRunner tr = new StubbedTestRunner();
            Assert.Throws<ArgumentNullException>(() => tr.RunAssembly(MockRepository.GenerateStub < ITestListener>(), null));
        }

        [Test]
        public void RunMemberThrowsWhenTestListenerIsNull()
        {
            TestDriven.Framework.ITestRunner tr = new StubbedTestRunner();
            Assert.Throws<ArgumentNullException>(() => tr.RunMember(null, GetType().Assembly, GetType()));
        }

        [Test]
        public void RunMemberThrowsWhenAssemblyIsNull()
        {
            TestDriven.Framework.ITestRunner tr = new StubbedTestRunner();
            Assert.Throws<ArgumentNullException>(() => tr.RunMember(MockRepository.GenerateStub<ITestListener>(), null, GetType()));
        }

        [Test]
        public void RunMemberThrowsWhenMemberIsNull()
        {
            TestDriven.Framework.ITestRunner tr = new StubbedTestRunner();
            Assert.Throws<ArgumentNullException>(() => tr.RunMember(MockRepository.GenerateStub<ITestListener>(), GetType().Assembly, null));
        }

        [Test]
        public void RunNamespaceThrowsWhenTestListenerIsNull()
        {
            TestDriven.Framework.ITestRunner tr = new StubbedTestRunner();
            Assert.Throws<ArgumentNullException>(() => tr.RunNamespace(null, GetType().Assembly, "Foo"));
        }

        [Test]
        public void RunNamespaceThrowsWhenAssemblyIsNull()
        {
            TestDriven.Framework.ITestRunner tr = new StubbedTestRunner();
            Assert.Throws<ArgumentNullException>(() => tr.RunNamespace(MockRepository.GenerateStub<ITestListener>(), null, "Foo"));
        }

        [Test]
        public void RunNamespaceThrowsWhenMemberIsNull()
        {
            TestDriven.Framework.ITestRunner tr = new StubbedTestRunner();
            Assert.Throws<ArgumentNullException>(() => tr.RunNamespace(MockRepository.GenerateStub<ITestListener>(), GetType().Assembly, null));
        }

#if RESIDENT
        [Test]
        public void ResidentRunThrowsWhenTestListenerIsNull()
        {
            IResidentTestRunner tr = new StubbedTestRunner();
            Assert.Throws<ArgumentNullException>(() => tr.Run(null, "Assembly.dll", "T:Foo"));
        }

        [Test]
        public void ResidentRunThrowsWhenAssemblyPathIsNull()
        {
            IResidentTestRunner tr = new StubbedTestRunner();
            Assert.Throws<ArgumentNullException>(() => tr.Run(MockRepository.GenerateStub<ITestListener>(), null, "T:Foo"));
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

            ((IResidentTestRunner)tr).Run(MockRepository.GenerateStub<ITestListener>(), assemblyPath, null);
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

            ((IResidentTestRunner)tr).Run(MockRepository.GenerateStub<ITestListener>(), assemblyPath, "T:" + type.FullName);
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

            ((IResidentTestRunner)tr).Run(MockRepository.GenerateStub<ITestListener>(), assemblyPath,
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

            ((IResidentTestRunner)tr).Run(MockRepository.GenerateStub<ITestListener>(), assemblyPath, "N:" + @namespace);
        }
#endif

        [Test]
        [Row(ResultCode.Canceled, TestRunState.Error)]
        [Row(ResultCode.Failure, TestRunState.Failure)]
        [Row(ResultCode.FatalException, TestRunState.Error)]
        [Row(ResultCode.InvalidArguments, TestRunState.Error)]
        [Row(ResultCode.NoTests, TestRunState.Success)]
        [Row(ResultCode.Success, TestRunState.Success)]
        public void RunReturnsCorrectResultCode(int resultCode, TestRunState expectedRunState)
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

            TestRunState runResult = ((IResidentTestRunner)tr).Run(MockRepository.GenerateStub<ITestListener>(), assemblyPath, null);
            Assert.AreEqual(expectedRunState, runResult);
        }

        private static void AssertTestLauncherOptions(TestLauncher launcher, string assemblyFile, Filter<ITest> filter)
        {
            Assert.IsFalse(launcher.DoNotRun);
            Assert.IsFalse(launcher.EchoResults);
            Assert.AreEqual(filter.ToFilterExpr(), launcher.TestExecutionOptions.Filter.ToFilterExpr());
            Assert.IsInstanceOfType(typeof(FilteredLogger), launcher.Logger);
            Assert.IsInstanceOfType(typeof(LogProgressMonitorProvider), launcher.ProgressMonitorProvider);
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
