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
using TestDriven.Framework;

namespace Gallio.TDNetRunner.Tests
{
    // TODO: Verify logged output
    [TestFixture]
    [TestsOn(typeof(StubbedGallioTestRunner))]
    public class TDNetAddInTests
    {
        private ITestListener stubbedTestListener;

        [FixtureSetUp]
        public void FixtureSetUp()
        {
            stubbedTestListener = MockRepository.GenerateStub<ITestListener>();
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RunAssembly_NullTestListener()
        {
            StubbedGallioTestRunner tr = new StubbedGallioTestRunner();
            tr.RunAssembly(null, null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RunAssembly_NullAssembly()
        {
            StubbedGallioTestRunner tr = new StubbedGallioTestRunner();
            tr.RunAssembly(stubbedTestListener, null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RunNamespace_NullTestListener()
        {
            StubbedGallioTestRunner tr = new StubbedGallioTestRunner();
            tr.RunNamespace(null, null, null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RunNamespace_NullAssembly()
        {
            StubbedGallioTestRunner tr = new StubbedGallioTestRunner();
            tr.RunNamespace(stubbedTestListener, null, null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RunNamespace_NullNamespace()
        {
            StubbedGallioTestRunner tr = new StubbedGallioTestRunner();
            tr.RunNamespace(stubbedTestListener, Assembly.GetExecutingAssembly(), null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RunMember_NullTestListener()
        {
            StubbedGallioTestRunner tr = new StubbedGallioTestRunner();
            tr.RunMember(null, null, null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RunMember_NullAssembly()
        {
            StubbedGallioTestRunner tr = new StubbedGallioTestRunner();
            tr.RunMember(stubbedTestListener, null, null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RunMember_NullMember()
        {
            StubbedGallioTestRunner tr = new StubbedGallioTestRunner();
            tr.RunMember(stubbedTestListener, Assembly.GetExecutingAssembly(), null);
        }

        [Test]
        public void RunAssemblyPassesCorrectOptionsToTheLauncher()
        {
            StubbedGallioTestRunner tr = new StubbedGallioTestRunner();

            Assembly assembly = typeof(TDNetAddInTests).Assembly;
            tr.SetRunLauncherAction(delegate(TestLauncher launcher)
            {
                AssertTestLauncherOptions(launcher, AssemblyUtils.GetAssemblyLocalPath(assembly),
                    new AssemblyFilter<ITest>(new EqualityFilter<string>(assembly.FullName)));

                return new TestLauncherResult(new Report());
            });

            tr.RunAssembly(stubbedTestListener, assembly);
        }

        [Test]
        public void RunMemberWithTypePassesCorrectOptionsToTheLauncher()
        {
            StubbedGallioTestRunner tr = new StubbedGallioTestRunner();

            Type type = typeof(TDNetAddInTests);
            Assembly assembly = type.Assembly;

            tr.SetRunLauncherAction(delegate(TestLauncher launcher)
            {
                AssertTestLauncherOptions(launcher, AssemblyUtils.GetAssemblyLocalPath(assembly),
                    new AndFilter<ITest>(new Filter<ITest>[] {
                        new AssemblyFilter<ITest>(new EqualityFilter<string>(assembly.FullName)),
                        new TypeFilter<ITest>(new EqualityFilter<string>(type.FullName), true)
                    }));

                return new TestLauncherResult(new Report());
            });

            tr.RunMember(stubbedTestListener, assembly, type);
        }

        [Test]
        public void RunMemberWithMethodPassesCorrectOptionsToTheLauncher()
        {
            StubbedGallioTestRunner tr = new StubbedGallioTestRunner();

            MethodBase method = Reflector.GetExecutingFunction().Resolve(true);
            Assembly assembly = method.DeclaringType.Assembly;

            tr.SetRunLauncherAction(delegate(TestLauncher launcher)
            {
                AssertTestLauncherOptions(launcher, AssemblyUtils.GetAssemblyLocalPath(assembly),
                    new AndFilter<ITest>(new Filter<ITest>[] {
                        new AssemblyFilter<ITest>(new EqualityFilter<string>(assembly.FullName)),
                        new TypeFilter<ITest>(new EqualityFilter<string>(method.DeclaringType.FullName), true),
                        new MemberFilter<ITest>(new EqualityFilter<string>(method.Name))
                    }));

                return new TestLauncherResult(new Report());
            });

            tr.RunMember(stubbedTestListener, assembly, method);
        }

        [Test]
        public void RunNamespacePassesCorrectOptionsToTheLauncher()
        {
            StubbedGallioTestRunner tr = new StubbedGallioTestRunner();

            Assembly assembly = typeof(TDNetAddInTests).Assembly;
            string @namespace = "Foo";

            tr.SetRunLauncherAction(delegate(TestLauncher launcher)
            {
                AssertTestLauncherOptions(launcher, AssemblyUtils.GetAssemblyLocalPath(assembly),
                    new AndFilter<ITest>(new Filter<ITest>[] {
                        new AssemblyFilter<ITest>(new EqualityFilter<string>(assembly.FullName)),
                        new NamespaceFilter<ITest>(new EqualityFilter<string>(@namespace))
                    }));

                return new TestLauncherResult(new Report());
            });

            tr.RunNamespace(stubbedTestListener, assembly, @namespace);
        }

        [Test]
        [Row(ResultCode.Canceled, TestRunState.Error)]
        [Row(ResultCode.Failure, TestRunState.Failure)]
        [Row(ResultCode.FatalException, TestRunState.Error)]
        [Row(ResultCode.InvalidArguments, TestRunState.Error)]
        [Row(ResultCode.NoTests, TestRunState.NoTests)]
        [Row(ResultCode.Success, TestRunState.Success)]
        public void RunReturnsCorrectResultCode(int resultCode, TestRunState expectedRunState)
        {
            StubbedGallioTestRunner tr = new StubbedGallioTestRunner();

            Assembly assembly = typeof(TDNetAddInTests).Assembly;
            tr.SetRunLauncherAction(delegate
            {
                TestLauncherResult result = new TestLauncherResult(new Report());
                result.SetResultCode(resultCode);
                return result;
            });

            Assert.AreEqual(expectedRunState, tr.RunAssembly(stubbedTestListener, assembly));
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
            OldCollectionAssert.AreElementsEqual(new string[] { "html" }, launcher.ReportFormats);
            Assert.AreEqual(Path.GetFileName(assemblyFile), launcher.ReportNameFormat);
            Assert.IsFalse(launcher.ShowReports);
            Assert.AreEqual(StandardTestRunnerFactoryNames.Local, launcher.TestRunnerFactoryName);

            Assert.IsNull(launcher.RuntimeSetup.ConfigurationFilePath);
            Assert.AreEqual(Path.GetDirectoryName(AssemblyUtils.GetAssemblyLocalPath(typeof(GallioTestRunner).Assembly)), launcher.RuntimeSetup.RuntimePath);
            OldCollectionAssert.AreElementsEqual(new string[] { }, launcher.RuntimeSetup.PluginDirectories);

            OldCollectionAssert.AreElementsEqual(new string[] { assemblyFile }, launcher.TestPackageConfig.AssemblyFiles);
            OldCollectionAssert.AreElementsEqual(new string[] { }, launcher.TestPackageConfig.HintDirectories);

            Assert.AreEqual(Path.GetDirectoryName(assemblyFile), launcher.TestPackageConfig.HostSetup.ApplicationBaseDirectory);
            Assert.IsFalse(launcher.TestPackageConfig.HostSetup.ShadowCopy);
            Assert.AreEqual(Path.GetDirectoryName(assemblyFile), launcher.TestPackageConfig.HostSetup.WorkingDirectory);
        }
    }
}
