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
using System.Drawing;
using Gallio.Icarus.Controls;
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
            Assert.AreEqual(0, settings.PluginDirectories.Count);
            settings.PluginDirectories.Add("test");
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

        [Test]
        public void AlwaysReloadAssemblies_Test()
        {
            Assert.IsFalse(settings.AlwaysReloadAssemblies);
            settings.AlwaysReloadAssemblies = true;
            Assert.IsTrue(settings.AlwaysReloadAssemblies);
        }

        [Test]
        public void ShowProgressDialogs_Test()
        {
            Assert.IsTrue(settings.ShowProgressDialogs);
            settings.ShowProgressDialogs = false;
            Assert.IsFalse(settings.ShowProgressDialogs);
        }

        [Test]
        public void TestStatusBarStyle_Test()
        {
            Assert.AreEqual(TestStatusBarStyles.Integration, settings.TestStatusBarStyle);
            settings.TestStatusBarStyle = TestStatusBarStyles.UnitTest;
            Assert.AreEqual(TestStatusBarStyles.UnitTest, settings.TestStatusBarStyle);
        }

        [Test]
        public void FailedColor_Test()
        {
            Assert.AreEqual(Color.Red.ToArgb(), settings.FailedColor);
            settings.FailedColor = Color.Black.ToArgb();
            Assert.AreEqual(Color.Black.ToArgb(), settings.FailedColor);
        }

        [Test]
        public void PassedColor_Test()
        {
            Assert.AreEqual(Color.Green.ToArgb(), settings.PassedColor);
            settings.PassedColor = Color.Black.ToArgb();
            Assert.AreEqual(Color.Black.ToArgb(), settings.PassedColor);
        }

        [Test]
        public void InconclusiveColor_Test()
        {
            Assert.AreEqual(Color.Gold.ToArgb(), settings.InconclusiveColor);
            settings.InconclusiveColor = Color.Black.ToArgb();
            Assert.AreEqual(Color.Black.ToArgb(), settings.InconclusiveColor);
        }

        [Test]
        public void SkippedColor_Test()
        {
            Assert.AreEqual(Color.SlateGray.ToArgb(), settings.SkippedColor);
            settings.SkippedColor = Color.Black.ToArgb();
            Assert.AreEqual(Color.Black.ToArgb(), settings.SkippedColor);
        }

        [Test]
        public void TreeViewCategories_Test()
        {
            Assert.AreEqual(0, settings.TreeViewCategories.Count);
            List<string> list = new List<string> {"test"};
            settings.TreeViewCategories = list;
            Assert.AreEqual(1, settings.TreeViewCategories.Count);
            Assert.AreEqual("test", settings.TreeViewCategories[0]);
        }
    }
}
