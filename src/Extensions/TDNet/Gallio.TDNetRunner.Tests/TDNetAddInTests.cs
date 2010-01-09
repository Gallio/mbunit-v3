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
using System.IO;
using System.Linq;
using System.Reflection;
using Gallio.Runner.Reports.Schema;
using Gallio.Runtime.Logging;
using Gallio.Runtime.ProgressMonitoring;
using Gallio.Model;
using Gallio.Model.Filters;
using Gallio.Common.Reflection;
using Gallio.Runner;
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
                AssertTestLauncherOptions(launcher, assemblyPath, "*");

                return new TestLauncherResult(new Report());
            });

            FacadeOptions facadeOptions = new FacadeOptions();
            tr.Run(MockRepository.GenerateStub<IFacadeTestListener>(), assemblyPath, null, facadeOptions);
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
                    string.Format("Type: {0}", type.FullName));

                return new TestLauncherResult(new Report());
            });

            FacadeOptions facadeOptions = new FacadeOptions();
            tr.Run(MockRepository.GenerateStub<IFacadeTestListener>(), assemblyPath, "T:" + type.FullName, facadeOptions);
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
                    string.Format("(Type: {0} and Member: {1})", method.DeclaringType.FullName, method.Name));

                return new TestLauncherResult(new Report());
            });

            FacadeOptions facadeOptions = new FacadeOptions();
            tr.Run(MockRepository.GenerateStub<IFacadeTestListener>(), assemblyPath,
                "M:" + method.DeclaringType.FullName + "." + method.Name, facadeOptions);
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
                    string.Format("Namespace: {0}", @namespace));

                return new TestLauncherResult(new Report());
            });

            FacadeOptions facadeOptions = new FacadeOptions();
            tr.Run(MockRepository.GenerateStub<IFacadeTestListener>(), assemblyPath, "N:" + @namespace, facadeOptions);
        }

        [Test]
        public void RunWithInclusionCategoryFilterPassesCorrectOptionsToTheLauncher()
        {
            StubbedLocalTestRunner tr = new StubbedLocalTestRunner();

            Assembly assembly = typeof(TDNetAddInTests).Assembly;
            string assemblyPath = AssemblyUtils.GetAssemblyLocalPath(assembly);
            string @namespace = "Foo";

            tr.SetRunLauncherAction(delegate(TestLauncher launcher)
            {
                AssertTestLauncherOptions(launcher, AssemblyUtils.GetAssemblyLocalPath(assembly),
                    string.Format("(Namespace: {0} and Category: {1}, {2})", @namespace, "abc", "def"));

                return new TestLauncherResult(new Report());
            });

            FacadeOptions facadeOptions = new FacadeOptions()
            {
                FilterCategoryMode = FacadeFilterCategoryMode.Include,
                FilterCategoryNames = new string[] { "abc", "def" }
            };
            tr.Run(MockRepository.GenerateStub<IFacadeTestListener>(), assemblyPath, "N:" + @namespace, facadeOptions);
        }

        [Test]
        public void RunWithExclusionCategoryFilterPassesCorrectOptionsToTheLauncher()
        {
            StubbedLocalTestRunner tr = new StubbedLocalTestRunner();

            Assembly assembly = typeof(TDNetAddInTests).Assembly;
            string assemblyPath = AssemblyUtils.GetAssemblyLocalPath(assembly);
            string @namespace = "Foo";

            tr.SetRunLauncherAction(delegate(TestLauncher launcher)
            {
                AssertTestLauncherOptions(launcher, AssemblyUtils.GetAssemblyLocalPath(assembly),
                    string.Format("exclude Category: {0}, {1} include Namespace: {2}", "abc", "def", @namespace));

                return new TestLauncherResult(new Report());
            });

            FacadeOptions facadeOptions = new FacadeOptions()
            {
                FilterCategoryMode = FacadeFilterCategoryMode.Exclude,
                FilterCategoryNames = new string[] { "abc", "def" }
            };
            tr.Run(MockRepository.GenerateStub<IFacadeTestListener>(), assemblyPath, "N:" + @namespace, facadeOptions);
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

            FacadeOptions facadeOptions = new FacadeOptions();
            FacadeTestRunState runResult = tr.Run(MockRepository.GenerateStub<IFacadeTestListener>(), assemblyPath, null, facadeOptions);
            Assert.AreEqual(expectedRunState, runResult);
        }

        private static void AssertTestLauncherOptions(TestLauncher launcher, string assemblyFile, string filterSetExpr)
        {
            Assert.IsFalse(launcher.DoNotRun);
            Assert.IsFalse(launcher.EchoResults);
            Assert.AreEqual(filterSetExpr, launcher.TestExecutionOptions.FilterSet.ToFilterSetExpr());
            Assert.IsInstanceOfType(typeof(FilteredLogger), launcher.Logger);
            Assert.IsInstanceOfType(typeof(LogProgressMonitorProvider), launcher.ProgressMonitorProvider);
            Assert.IsFalse(launcher.ShowReports);
            Assert.AreEqual(StandardTestRunnerFactoryNames.IsolatedAppDomain, launcher.TestProject.TestRunnerFactoryName);

            Assert.IsNull(launcher.RuntimeSetup);

            Assert.AreElementsEqual(new string[] { assemblyFile }, from x in launcher.TestProject.TestPackage.Files select x.ToString());
            Assert.AreElementsEqual(new string[] { }, from x in launcher.TestProject.TestPackage.HintDirectories select x.ToString());

            Assert.AreEqual(Path.GetDirectoryName(assemblyFile), launcher.TestProject.TestPackage.ApplicationBaseDirectory.ToString());
            Assert.IsFalse(launcher.TestProject.TestPackage.ShadowCopy);
            Assert.AreEqual(Path.GetDirectoryName(assemblyFile), launcher.TestProject.TestPackage.WorkingDirectory.ToString());
        }
    }
}
