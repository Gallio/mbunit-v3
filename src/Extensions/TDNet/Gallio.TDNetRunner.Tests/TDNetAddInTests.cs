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
using System.Reflection;
using Gallio.Runtime.Logging;
using Gallio.Runtime.ProgressMonitoring;
using Gallio.Model;
using Gallio.Model.Filters;
using Gallio.Reflection;
using Gallio.Runner;
using Gallio.Runner.Reports;
using Gallio.TDNetRunner.Facade;
using MbUnit.Framework;
using Rhino.Mocks;
using TestDriven.Framework;
using TestDriven.Framework.Resident;
using ITestRunner = TestDriven.Framework.ITestRunner;

namespace Gallio.TDNetRunner.Tests
{
    // TODO: Verify logged output
    [TestFixture]
    [TestsOn(typeof(StubbedLocalTestRunner))]
    public class TDNetAddInTests
    {
        [Test]
        public void RunAssemblyThrowsWhenTestListenerIsNull()
        {
            GallioTestRunner tr = new GallioTestRunner();
            tr.TestRunner = new StubbedLocalTestRunner();
            Assert.Throws<ArgumentNullException>(() => tr.RunAssembly(null, GetType().Assembly));
        }

        [Test]
        public void RunAssemblyThrowsWhenAssemblyIsNull()
        {
            GallioTestRunner tr = new GallioTestRunner();
            tr.TestRunner = new StubbedLocalTestRunner();
            Assert.Throws<ArgumentNullException>(() => tr.RunAssembly(MockRepository.GenerateStub<ITestListener>(), null));
        }

        [Test]
        public void RunMemberThrowsWhenTestListenerIsNull()
        {
            GallioTestRunner tr = new GallioTestRunner();
            tr.TestRunner = new StubbedLocalTestRunner();
            Assert.Throws<ArgumentNullException>(() => tr.RunMember(null, GetType().Assembly, GetType()));
        }

        [Test]
        public void RunMemberThrowsWhenAssemblyIsNull()
        {
            GallioTestRunner tr = new GallioTestRunner();
            tr.TestRunner = new StubbedLocalTestRunner();
            Assert.Throws<ArgumentNullException>(() => tr.RunMember(MockRepository.GenerateStub<ITestListener>(), null, GetType()));
        }

        [Test]
        public void RunMemberThrowsWhenMemberIsNull()
        {
            GallioTestRunner tr = new GallioTestRunner();
            tr.TestRunner = new StubbedLocalTestRunner();
            Assert.Throws<ArgumentNullException>(() => tr.RunMember(MockRepository.GenerateStub<ITestListener>(), GetType().Assembly, null));
        }

        [Test]
        public void RunNamespaceThrowsWhenTestListenerIsNull()
        {
            GallioTestRunner tr = new GallioTestRunner();
            tr.TestRunner = new StubbedLocalTestRunner();
            Assert.Throws<ArgumentNullException>(() => tr.RunNamespace(null, GetType().Assembly, "Foo"));
        }

        [Test]
        public void RunNamespaceThrowsWhenAssemblyIsNull()
        {
            GallioTestRunner tr = new GallioTestRunner();
            tr.TestRunner = new StubbedLocalTestRunner();
            Assert.Throws<ArgumentNullException>(() => tr.RunNamespace(MockRepository.GenerateStub<ITestListener>(), null, "Foo"));
        }

        [Test]
        public void RunNamespaceThrowsWhenMemberIsNull()
        {
            GallioTestRunner tr = new GallioTestRunner();
            tr.TestRunner = new StubbedLocalTestRunner();
            Assert.Throws<ArgumentNullException>(() => tr.RunNamespace(MockRepository.GenerateStub<ITestListener>(), GetType().Assembly, null));
        }

        [Test]
        public void ResidentRunThrowsWhenTestListenerIsNull()
        {
            GallioResidentTestRunner tr = new GallioResidentTestRunner();
            tr.TestRunner = new StubbedLocalTestRunner();
            Assert.Throws<ArgumentNullException>(() => tr.Run(null, "Assembly.dll", "T:Foo"));
        }

        [Test]
        public void ResidentRunThrowsWhenAssemblyPathIsNull()
        {
            GallioResidentTestRunner tr = new GallioResidentTestRunner();
            tr.TestRunner = new StubbedLocalTestRunner();
            Assert.Throws<ArgumentNullException>(() => tr.Run(MockRepository.GenerateStub<ITestListener>(), null, "T:Foo"));
        }

        [Test]
        public void RunAssemblyPassesCorrectOptionsToTheLauncher()
        {
            StubbedLocalTestRunner tr = new StubbedLocalTestRunner();

            Assembly assembly = typeof(TDNetAddInTests).Assembly;
            string assemblyPath = AssemblyUtils.GetAssemblyLocalPath(assembly);

            tr.SetRunLauncherAction(delegate(TestLauncher launcher)
            {
                AssertTestLauncherOptions(launcher, assemblyPath, new AnyFilter<ITest>());

                return new TestLauncherResult(new Report());
            });

            tr.Run(MockRepository.GenerateStub<IFacadeTestListener>(), assemblyPath, null);
        }

        [Test]
        public void RunMemberWithTypePassesCorrectOptionsToTheLauncher()
        {
            StubbedLocalTestRunner tr = new StubbedLocalTestRunner();

            Type type = typeof(TDNetAddInTests);
            Assembly assembly = type.Assembly;
            string assemblyPath = AssemblyUtils.GetAssemblyLocalPath(assembly);

            tr.SetRunLauncherAction(delegate(TestLauncher launcher)
            {
                AssertTestLauncherOptions(launcher, assemblyPath,
                    new TypeFilter<ITest>(new EqualityFilter<string>(type.FullName), true));

                return new TestLauncherResult(new Report());
            });

            tr.Run(MockRepository.GenerateStub<IFacadeTestListener>(), assemblyPath, "T:" + type.FullName);
        }

        [Test]
        public void RunMemberWithMethodPassesCorrectOptionsToTheLauncher()
        {
            StubbedLocalTestRunner tr = new StubbedLocalTestRunner();

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

            tr.Run(MockRepository.GenerateStub<IFacadeTestListener>(), assemblyPath,
                "M:" + method.DeclaringType.FullName + "." + method.Name);
        }

        [Test]
        public void RunNamespacePassesCorrectOptionsToTheLauncher()
        {
            StubbedLocalTestRunner tr = new StubbedLocalTestRunner();

            Assembly assembly = typeof(TDNetAddInTests).Assembly;
            string assemblyPath = AssemblyUtils.GetAssemblyLocalPath(assembly);
            string @namespace = "Foo";

            tr.SetRunLauncherAction(delegate(TestLauncher launcher)
            {
                AssertTestLauncherOptions(launcher, AssemblyUtils.GetAssemblyLocalPath(assembly),
                    new NamespaceFilter<ITest>(new EqualityFilter<string>(@namespace)));

                return new TestLauncherResult(new Report());
            });

            tr.Run(MockRepository.GenerateStub<IFacadeTestListener>(), assemblyPath, "N:" + @namespace);
        }

        [Test]
        [Row(ResultCode.Canceled, FacadeTestRunState.Failure)]
        [Row(ResultCode.Failure, FacadeTestRunState.Failure)]
        [Row(ResultCode.FatalException, FacadeTestRunState.Error)]
        [Row(ResultCode.InvalidArguments, FacadeTestRunState.Error)]
        [Row(ResultCode.NoTests, FacadeTestRunState.Success)]
        [Row(ResultCode.Success, FacadeTestRunState.Success)]
        public void RunReturnsCorrectResultCode(int resultCode, FacadeTestRunState expectedRunState)
        {
            StubbedLocalTestRunner tr = new StubbedLocalTestRunner();

            Assembly assembly = typeof(TDNetAddInTests).Assembly;
            string assemblyPath = AssemblyUtils.GetAssemblyLocalPath(assembly);
            tr.SetRunLauncherAction(delegate
            {
                TestLauncherResult result = new TestLauncherResult(new Report());
                result.SetResultCode(resultCode);
                return result;
            });

            FacadeTestRunState runResult = tr.Run(MockRepository.GenerateStub<IFacadeTestListener>(), assemblyPath, null);
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
            Assert.AreEqual(StandardTestRunnerFactoryNames.IsolatedAppDomain, launcher.TestRunnerFactoryName);

            Assert.IsNull(launcher.RuntimeSetup);

            Assert.AreElementsEqual(new string[] { assemblyFile }, launcher.TestPackageConfig.AssemblyFiles);
            Assert.AreElementsEqual(new string[] { }, launcher.TestPackageConfig.HintDirectories);

            Assert.AreEqual(Path.GetDirectoryName(assemblyFile), launcher.TestPackageConfig.HostSetup.ApplicationBaseDirectory);
            Assert.IsFalse(launcher.TestPackageConfig.HostSetup.ShadowCopy);
            Assert.AreEqual(Path.GetDirectoryName(assemblyFile), launcher.TestPackageConfig.HostSetup.WorkingDirectory);
        }
    }
}
