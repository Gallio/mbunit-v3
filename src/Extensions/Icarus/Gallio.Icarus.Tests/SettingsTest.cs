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

using System.Collections.Generic;

using MbUnit.Framework;
using Gallio.Runner;

namespace Gallio.Icarus.Tests
{
    [TestFixture, Category("Settings")]
    public class SettingsTest
    {
        private Settings settings;

        [SetUp]
        public void SetUp()
        {
            settings = new Settings();
        }

        [Test]
        public void RestorePreviousSettings_Test()
        {
            Assert.IsTrue(settings.RestorePreviousSettings);
            settings.RestorePreviousSettings = false;
            Assert.IsFalse(settings.RestorePreviousSettings);
        }

        [Test]
        public void PluginDirectories_Test()
        {
            List<string> pluginDirectories = new List<string>();
            pluginDirectories.Add("test");
            Assert.AreEqual(0, settings.PluginDirectories.Count);
            settings.PluginDirectories = pluginDirectories;
            Assert.AreEqual(1, settings.PluginDirectories.Count);
            Assert.AreEqual("test", settings.PluginDirectories[0]);
        }

        [Test]
        public void TestRunnerFactory_Test()
        {
            Assert.AreEqual(StandardTestRunnerFactoryNames.IsolatedProcess, settings.TestRunnerFactory);
            settings.TestRunnerFactory = StandardTestRunnerFactoryNames.Local;
            Assert.AreEqual(StandardTestRunnerFactoryNames.Local, settings.TestRunnerFactory);
        }
    }
}
