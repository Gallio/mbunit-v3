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
using Gallio.Common.Collections;
using Gallio.Model;
using Gallio.Model.Filters;
using Gallio.Common.Reflection;
using Gallio.Runner;
using Gallio.Runner.Reports;
using MbUnit.Framework;

namespace Gallio.Echo.Tests
{
    [TestFixture]
    [TestsOn(typeof(EchoProgram))]
    [Category("UnitTests")]
    public class EchoProgramUnitTest
    {
        [Test]
        public void TaskPassesDefaultArgumentsToLauncher()
        {
            TestLauncher launcher = new TestLauncher();
            EchoArguments arguments = new EchoArguments();

            EchoProgram.ConfigureLauncherFromArguments(launcher, arguments);

            Assert.IsFalse(launcher.DoNotRun);
            Assert.IsTrue(launcher.EchoResults);
            Assert.IsTrue(launcher.TestExecutionOptions.FilterSet.IsEmpty);
            Assert.AreEqual("", launcher.ReportDirectory);
            Assert.AreElementsEqual(new string[] { }, launcher.ReportFormats);
            Assert.AreEqual("test-report-{0}-{1}", launcher.ReportNameFormat);
            Assert.IsFalse(launcher.ShowReports);
            Assert.IsNull(launcher.RunTimeLimit);

            Assert.AreEqual(StandardTestRunnerFactoryNames.IsolatedProcess, launcher.TestRunnerFactoryName);
            Assert.AreEqual(0, launcher.TestRunnerExtensions.Count);
            Assert.AreElementsEqual(new string[] { }, launcher.TestRunnerExtensionSpecifications);

            Assert.IsNull(launcher.RuntimeSetup.ConfigurationFilePath);
            Assert.IsNull(launcher.RuntimeSetup.RuntimePath);
            Assert.AreElementsEqual(new string[] { }, launcher.RuntimeSetup.PluginDirectories);

            Assert.AreElementsEqual(new string[] { }, launcher.TestPackageConfig.Files);
            Assert.AreElementsEqual(new string[] { }, launcher.TestPackageConfig.HintDirectories);

            Assert.IsNull(launcher.TestPackageConfig.ApplicationBaseDirectory);
            Assert.IsNull(launcher.TestPackageConfig.WorkingDirectory);
            Assert.IsFalse(launcher.TestPackageConfig.ShadowCopy);
            Assert.IsFalse(launcher.TestPackageConfig.Debug);
            Assert.IsNull(launcher.TestPackageConfig.RuntimeVersion);

            Assert.AreEqual(new PropertySet(), launcher.TestRunnerOptions.Properties);
            Assert.AreEqual(new PropertySet(), launcher.ReportFormatterOptions.Properties);
        }

        [Test]
        public void TaskPassesSpecifiedArgumentsToLauncher()
        {
            TestLauncher launcher = new TestLauncher();
            EchoArguments arguments = new EchoArguments();

            arguments.DoNotRun = true;
            arguments.NoEchoResults = true;
            arguments.Filter = "Type: SimpleTest";
            arguments.ReportDirectory = "dir";
            arguments.ReportTypes = new string[] { "XML", "Html" };
            arguments.ReportNameFormat = "report";
            arguments.ShowReports = true;
            arguments.RunTimeLimitInSeconds = 7200; // seconds = 120 minutes

            arguments.RunnerType = StandardTestRunnerFactoryNames.Local;
            arguments.RunnerExtensions = new string[] { "DebugExtension,Gallio" };

            arguments.PluginDirectories = new string[] { "plugin" };
            arguments.Files = new[] { Assembly.GetExecutingAssembly().CodeBase };
            arguments.HintDirectories = new string[] { "hint1", "hint2" };

            arguments.ApplicationBaseDirectory = "baseDir";
            arguments.WorkingDirectory = "workingDir";
            arguments.ShadowCopy = true;
            arguments.Debug = true;
            arguments.RuntimeVersion = "v4.0.20506";

            arguments.RunnerProperties = new[] { "RunnerOption1=RunnerValue1", "  RunnerOption2  ", "RunnerOption3 = 'RunnerValue3'", "RunnerOption4=\"'RunnerValue4'\"" };
            arguments.ReportFormatterProperties = new[] { "FormatterOption1=FormatterValue1", "  FormatterOption2  ", "FormatterOption3 = 'FormatterValue3'", "FormatterOption4=\"'FormatterValue4'\"" };

            EchoProgram.ConfigureLauncherFromArguments(launcher, arguments);

            Assert.IsTrue(launcher.DoNotRun);
            Assert.IsFalse(launcher.EchoResults);
            Assert.AreEqual("Type: SimpleTest", launcher.TestExecutionOptions.FilterSet.ToFilterSetExpr());
            Assert.AreEqual("dir", launcher.ReportDirectory);
            Assert.AreElementsEqual(new string[] { "XML", "Html" }, launcher.ReportFormats);
            Assert.AreEqual("report", launcher.ReportNameFormat);
            Assert.IsTrue(launcher.ShowReports);
            Assert.AreEqual(TimeSpan.FromMinutes(120), launcher.RunTimeLimit);

            Assert.AreEqual(StandardTestRunnerFactoryNames.Local, launcher.TestRunnerFactoryName);
            Assert.AreEqual(0, launcher.TestRunnerExtensions.Count);
            Assert.AreElementsEqual(new[] { "DebugExtension,Gallio" }, launcher.TestRunnerExtensionSpecifications);

            Assert.IsNull(launcher.RuntimeSetup.ConfigurationFilePath);
            Assert.IsNull(launcher.RuntimeSetup.RuntimePath);
            Assert.AreElementsEqual(new[] { "plugin" }, launcher.RuntimeSetup.PluginDirectories);

            Assert.AreEqual(1, launcher.TestPackageConfig.Files.Count);
            Assert.AreElementsEqual(new[] { "hint1", "hint2" }, launcher.TestPackageConfig.HintDirectories);

            Assert.AreEqual("baseDir", launcher.TestPackageConfig.ApplicationBaseDirectory);
            Assert.AreEqual("workingDir", launcher.TestPackageConfig.WorkingDirectory);
            Assert.IsTrue(launcher.TestPackageConfig.ShadowCopy);
            Assert.IsTrue(launcher.TestPackageConfig.Debug);
            Assert.AreEqual("v4.0.20506", launcher.TestPackageConfig.RuntimeVersion);

            Assert.AreEqual(new PropertySet()
                {
                    { "RunnerOption1", "RunnerValue1" },
                    { "RunnerOption2", "" },
                    { "RunnerOption3", "RunnerValue3" },
                    { "RunnerOption4", "'RunnerValue4'" }
                }, launcher.TestRunnerOptions.Properties);

            Assert.AreEqual(new PropertySet()
                {
                    { "FormatterOption1", "FormatterValue1" },
                    { "FormatterOption2", "" },
                    { "FormatterOption3", "FormatterValue3" },
                    { "FormatterOption4", "'FormatterValue4'" }
                }, launcher.ReportFormatterOptions.Properties);
        }
    }
}
