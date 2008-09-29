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

using System.Drawing;
using System.IO;
using Gallio.Icarus.Controllers;
using Gallio.Icarus.Controls;
using Gallio.Runner;
using MbUnit.Framework;

namespace Gallio.Icarus.Tests.Controllers
{
    class OptionsControllerTest
    {
        private readonly string settingsBackup = Paths.SettingsFile + ".bak";

        [FixtureSetUp]
        public void FixtureSetUp()
        {
            if (!File.Exists(Paths.SettingsFile))
                return;
            File.Copy(Paths.SettingsFile, settingsBackup, true);
            File.Delete(Paths.SettingsFile);
        }

        [FixtureTearDown]
        public void FixtureTearDown()
        {
            if (!File.Exists(settingsBackup))    
                return;
            if (File.Exists(Paths.SettingsFile))
                File.Delete(Paths.SettingsFile);
            File.Copy(settingsBackup, Paths.SettingsFile);
            File.Delete(settingsBackup);
        }

        [Test]
        public void RestorePreviousSettings_Test()
        {
            Assert.IsTrue(OptionsController.Instance.RestorePreviousSettings);
            OptionsController.Instance.RestorePreviousSettings = false;
            Assert.IsFalse(OptionsController.Instance.RestorePreviousSettings);
        }

        [Test]
        public void PluginDirectories_Test()
        {
            Assert.AreEqual(0, OptionsController.Instance.PluginDirectories.Count);
            OptionsController.Instance.PluginDirectories.Add("test");
            Assert.AreEqual(1, OptionsController.Instance.PluginDirectories.Count);
            Assert.AreEqual("test", OptionsController.Instance.PluginDirectories[0]);
        }

        [Test]
        public void TestRunnerFactory_Test()
        {
            Assert.AreEqual(StandardTestRunnerFactoryNames.IsolatedProcess, OptionsController.Instance.TestRunnerFactory);
            OptionsController.Instance.TestRunnerFactory = StandardTestRunnerFactoryNames.Local;
            Assert.AreEqual(StandardTestRunnerFactoryNames.Local, OptionsController.Instance.TestRunnerFactory);
        }

        [Test]
        public void AlwaysReloadAssemblies_Test()
        {
            Assert.IsFalse(OptionsController.Instance.AlwaysReloadAssemblies);
            OptionsController.Instance.AlwaysReloadAssemblies = true;
            Assert.IsTrue(OptionsController.Instance.AlwaysReloadAssemblies);
        }

        [Test]
        public void ShowProgressDialogs_Test()
        {
            Assert.IsTrue(OptionsController.Instance.ShowProgressDialogs);
            OptionsController.Instance.ShowProgressDialogs = false;
            Assert.IsFalse(OptionsController.Instance.ShowProgressDialogs);
        }

        [Test]
        public void TestStatusBarStyle_Test()
        {
            Assert.AreEqual(TestStatusBarStyles.Integration, OptionsController.Instance.TestStatusBarStyle);
            OptionsController.Instance.TestStatusBarStyle = TestStatusBarStyles.UnitTest;
            Assert.AreEqual(TestStatusBarStyles.UnitTest, OptionsController.Instance.TestStatusBarStyle);
        }

        [Test]
        public void FailedColor_Test()
        {
            Assert.AreEqual(Color.Red.ToArgb(), OptionsController.Instance.FailedColor.ToArgb());
            OptionsController.Instance.FailedColor = Color.Black;
            Assert.AreEqual(Color.Black.ToArgb(), OptionsController.Instance.FailedColor.ToArgb());
        }

        [Test]
        public void PassedColor_Test()
        {
            Assert.AreEqual(Color.Green.ToArgb(), OptionsController.Instance.PassedColor.ToArgb());
            OptionsController.Instance.PassedColor = Color.Black;
            Assert.AreEqual(Color.Black.ToArgb(), OptionsController.Instance.PassedColor.ToArgb());
        }

        [Test]
        public void InconclusiveColor_Test()
        {
            Assert.AreEqual(Color.Gold.ToArgb(), OptionsController.Instance.InconclusiveColor.ToArgb());
            OptionsController.Instance.InconclusiveColor = Color.Black;
            Assert.AreEqual(Color.Black.ToArgb(), OptionsController.Instance.InconclusiveColor.ToArgb());
        }

        [Test]
        public void SkippedColor_Test()
        {
            Assert.AreEqual(Color.SlateGray.ToArgb(), OptionsController.Instance.SkippedColor.ToArgb());
            OptionsController.Instance.SkippedColor = Color.Black;
            Assert.AreEqual(Color.Black.ToArgb(), OptionsController.Instance.SkippedColor.ToArgb());
        }

        [Test]
        public void SelectedTreeViewCategories_Test()
        {
            Assert.AreEqual(5, OptionsController.Instance.SelectedTreeViewCategories.Count);
        }

        [Test]
        public void Cancel_Test()
        {
            OptionsController.Instance.Cancel();
        }

        [Test]
        public void Save_Test()
        {
            Assert.IsFalse(File.Exists(Paths.SettingsFile));
            OptionsController.Instance.Save();
            Assert.IsTrue(File.Exists(Paths.SettingsFile));
            File.Delete(Paths.SettingsFile);
        }

        [Test]
        public void TestRunnerFactories_Test()
        {
            Assert.AreEqual(3, OptionsController.Instance.TestRunnerFactories.Length);
        }

        [Test]
        public void UnselectedTreeViewCategories_Test()
        {
            Assert.AreEqual(20, OptionsController.Instance.UnselectedTreeViewCategories.Count);
        }
    }
}
