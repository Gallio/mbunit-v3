// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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
using System.IO;
using Gallio.Hosting;
using Gallio.Model;
using Gallio.Model.Filters;
using Gallio.Runner;
using Gallio.Runner.Reports;
using MbUnit.Framework;
using Gallio.PowerShellCommands;

namespace Gallio.PowerShellCommands.Tests
{
    [TestFixture]
    [TestsOn(typeof(RunGallioCommand))]
    [Category("UnitTests")]
    public class RunGallioCommandUnitTest
    {
        [Test]
        public void TaskPassesDefaultArgumentsToLauncher()
        {
            StubbedRunGallioCommand task = new StubbedRunGallioCommand();

            task.SetRunLauncherAction(delegate(TestLauncher launcher)
            {
                Assert.IsFalse(launcher.DoNotRun);
                Assert.IsTrue(launcher.EchoResults);
                Assert.IsInstanceOfType(typeof(AnyFilter<ITest>), launcher.Filter);
                Assert.IsInstanceOfType(typeof(CommandLogger), launcher.Logger);
                Assert.IsInstanceOfType(typeof(CommandProgressMonitorProvider), launcher.ProgressMonitorProvider);
                Assert.AreEqual("", launcher.ReportDirectory);
                Assert.AreEqual(0, launcher.ReportFormatOptions.Count);
                CollectionAssert.AreElementsEqual(new string[] { }, launcher.ReportFormats);
                Assert.AreEqual("test-report-{0}-{1}", launcher.ReportNameFormat);
                Assert.IsFalse(launcher.ShowReports);
                Assert.AreEqual(StandardTestRunnerFactoryNames.IsolatedProcess, launcher.TestRunnerFactoryName);
                Assert.AreEqual(0, launcher.TestRunnerOptions.Count);

                Assert.AreEqual(0, launcher.CustomMonitors.Count);

                Assert.IsNull(launcher.RuntimeSetup.ConfigurationFilePath);
                Assert.AreEqual(Path.GetDirectoryName(Loader.GetAssemblyLocalPath(typeof(RunGallioCommand).Assembly)), launcher.RuntimeSetup.InstallationPath);
                CollectionAssert.AreElementsEqual(new string[] { }, launcher.RuntimeSetup.PluginDirectories);
                Assert.IsNull(launcher.RuntimeSetup.RuntimeFactoryType);

                CollectionAssert.AreElementsEqual(new string[] { }, launcher.TestPackageConfig.AssemblyFiles);
                CollectionAssert.AreElementsEqual(new string[] { }, launcher.TestPackageConfig.HintDirectories);

                Assert.AreEqual("", launcher.TestPackageConfig.HostSetup.ApplicationBaseDirectory);
                Assert.IsFalse(launcher.TestPackageConfig.HostSetup.ShadowCopy);
                Assert.AreEqual("", launcher.TestPackageConfig.HostSetup.WorkingDirectory);

                return new TestLauncherResult(new Report());
            });

            task.ExecuteWithMessagePump();
        }

        [Test]
        public void TaskPassesSpecifiedArgumentsToLauncher()
        {
            StubbedRunGallioCommand task = new StubbedRunGallioCommand();
            task.DoNotRun = true;
            task.NoEchoResults = true;
            task.Filter = "Type: SimpleTest";
            task.ReportDirectory = "dir";
            task.ReportTypes = new string[] { "XML", "Html" };
            task.ReportNameFormat = "report";
            task.ShowReports = true;
            task.RunnerType = StandardTestRunnerFactoryNames.LocalAppDomain;

            task.PluginDirectories = new string[] { "plugin" };
            task.Assemblies = new string[] { "assembly1", "assembly2" };
            task.HintDirectories = new string[] { "hint1", "hint2" };

            task.ApplicationBaseDirectory = "baseDir";
            task.ShadowCopy = true;
            task.WorkingDirectory = "workingDir";

            task.SetRunLauncherAction(delegate(TestLauncher launcher)
            {
                Assert.IsTrue(launcher.DoNotRun);
                Assert.IsFalse(launcher.EchoResults);
                Assert.AreEqual("Type: SimpleTest", launcher.Filter.ToFilterExpr());
                Assert.IsInstanceOfType(typeof(CommandLogger), launcher.Logger);
                Assert.IsInstanceOfType(typeof(CommandProgressMonitorProvider), launcher.ProgressMonitorProvider);
                Assert.AreEqual("dir", launcher.ReportDirectory);
                Assert.AreEqual(0, launcher.ReportFormatOptions.Count);
                CollectionAssert.AreElementsEqual(new string[] { "XML", "Html" }, launcher.ReportFormats);
                Assert.AreEqual("report", launcher.ReportNameFormat);
                Assert.IsTrue(launcher.ShowReports);
                Assert.AreEqual(StandardTestRunnerFactoryNames.LocalAppDomain, launcher.TestRunnerFactoryName);
                Assert.AreEqual(0, launcher.TestRunnerOptions.Count);

                Assert.AreEqual(0, launcher.CustomMonitors.Count);

                Assert.IsNull(launcher.RuntimeSetup.ConfigurationFilePath);
                Assert.AreEqual(Path.GetDirectoryName(Loader.GetAssemblyLocalPath(typeof(RunGallioCommand).Assembly)), launcher.RuntimeSetup.InstallationPath);
                CollectionAssert.AreElementsEqual(new string[] { "plugin" }, launcher.RuntimeSetup.PluginDirectories);
                Assert.IsNull(launcher.RuntimeSetup.RuntimeFactoryType);

                CollectionAssert.AreElementsEqual(new string[] { "assembly1", "assembly2" }, launcher.TestPackageConfig.AssemblyFiles);
                CollectionAssert.AreElementsEqual(new string[] { "hint1", "hint2" }, launcher.TestPackageConfig.HintDirectories);

                Assert.AreEqual("baseDir", launcher.TestPackageConfig.HostSetup.ApplicationBaseDirectory);
                Assert.IsTrue(launcher.TestPackageConfig.HostSetup.ShadowCopy);
                Assert.AreEqual("workingDir", launcher.TestPackageConfig.HostSetup.WorkingDirectory);

                return new TestLauncherResult(new Report());
            });

            task.ExecuteWithMessagePump();
        }

        [Test]
        public void TaskExposesResultsReturnedByLauncher()
        {
            StubbedRunGallioCommand task = new StubbedRunGallioCommand();
            TestLauncherResult expectedResult = new TestLauncherResult(new Report());

            task.SetRunLauncherAction(delegate
            {
                return expectedResult;
            });

            Assert.AreSame(expectedResult, task.ExecuteWithMessagePump());
        }
    }
}
