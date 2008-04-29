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
using System.Configuration;
using Gallio.Framework;
using Gallio.Model;
using Gallio.Reflection;
using Gallio.Runner.Reports;
using Gallio.Tests.Integration;
using MbUnit.Framework;

namespace MbUnit.Tests.Framework
{
    /// <summary>
    /// Tests the test assembly configuration file integration.
    /// </summary>
    [TestFixture]
    public class ConfigurationFileTest : BaseSampleTest
    {
        [FixtureSetUp]
        public void RunSample()
        {
            RunFixtures(typeof(ConfigurationFileSample));
        }

        [Test]
        public void TestCanAccessItsAppSettings()
        {
            TestStepRun run = Runner.GetPrimaryTestStepRun(
                CodeReference.CreateFromMember(typeof(ConfigurationFileSample).GetMethod("AppSettingsAreAccessible")));

            Assert.AreEqual(TestOutcome.Passed, run.Result.Outcome);
            AssertLogOutputContains(run, "TestConfigurationValue");
        }
    }

    [TestFixture, Explicit("Sample")]
    internal class ConfigurationFileSample
    {
        [Test]
        public void AppSettingsAreAccessible()
        {
            string value = ConfigurationManager.AppSettings["TestConfigurationSetting"];
            Assert.AreEqual("TestConfigurationValue", value);
            Log.WriteLine(value);
        }
    }
}
