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
using System.Configuration;
using System.IO;
using Gallio.Framework;
using Gallio.Model;
using Gallio.Common.Reflection;
using Gallio.Runner.Reports.Schema;
using Gallio.Tests;
using MbUnit.Framework;

namespace MbUnit.Tests.Integration
{
    /// <summary>
    /// Tests the test assembly configuration file integration.
    /// </summary>
    [TestFixture]
    [RunSample(typeof(ConfigurationFileSample))]
    public class ConfigurationFileTest : BaseTestWithSampleRunner
    {
        [Test]
        public void TestCanAccessItsAppSettings()
        {
            TestStepRun run = Runner.GetPrimaryTestStepRun(
                CodeReference.CreateFromMember(typeof(ConfigurationFileSample).GetMethod("AppSettingsAreAccessible")));

            Assert.AreEqual(TestOutcome.Passed, run.Result.Outcome);
            AssertLogContains(run, "TestConfigurationValue");
        }

        [TestFixture, Explicit("Sample")]
        internal class ConfigurationFileSample
        {
            [Test]
            public void AppSettingsAreAccessible()
            {
                string value = ConfigurationManager.AppSettings["TestConfigurationSetting"];
                Assert.AreEqual("TestConfigurationValue", value);
                TestLog.WriteLine(value);
            }

            [Test]
            public void ConfigFileIsInAppBase()
            {
                Assert.AreEqual(AppDomain.CurrentDomain.BaseDirectory,
                    Path.GetDirectoryName(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile));
            }
        }
    }
}
