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
using System.Reflection;
using System;
using Gallio.Icarus.Utilities;
using Rhino.Mocks;
using Gallio.Runtime;

namespace Gallio.Icarus.Tests.Controllers
{
    class OptionsControllerTest
    {
        private OptionsController optionsController = null;
        private IFileSystem fileSystem = null;
        private IXmlSerialization xmlSerialization = null;

        [SetUp]
        public void SetUp()
        {
            ConstructorInfo ci = typeof(OptionsController).GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, 
                null, new Type[] { typeof(IFileSystem), typeof(IXmlSerialization) }, null);
            Assert.IsNotNull(ci);
            fileSystem = MockRepository.GenerateStub<IFileSystem>();
            xmlSerialization = MockRepository.GenerateStub<IXmlSerialization>();
            fileSystem.Stub(x => x.FileExists(Paths.SettingsFile)).Return(true);
            xmlSerialization.Stub(x => x.LoadFromXml<Settings>(Paths.SettingsFile)).Return(new Settings());
            optionsController = (OptionsController)ci.Invoke(new object[] { fileSystem, xmlSerialization });
        }

        [Test]
        public void RestorePreviousSettings_Test()
        {
            Assert.IsTrue(optionsController.RestorePreviousSettings);
            optionsController.RestorePreviousSettings = false;
            Assert.IsFalse(optionsController.RestorePreviousSettings);
        }

        [Test]
        public void PluginDirectories_Test()
        {
            Assert.AreEqual(0, optionsController.PluginDirectories.Count);
            optionsController.PluginDirectories.Add("test");
            Assert.AreEqual(1, optionsController.PluginDirectories.Count);
            Assert.AreEqual("test", optionsController.PluginDirectories[0]);
        }

        [Test]
        public void TestRunnerFactory_Test()
        {
            Assert.AreEqual(StandardTestRunnerFactoryNames.IsolatedProcess, optionsController.TestRunnerFactory);
            optionsController.TestRunnerFactory = StandardTestRunnerFactoryNames.Local;
            Assert.AreEqual(StandardTestRunnerFactoryNames.Local, optionsController.TestRunnerFactory);
        }

        [Test]
        public void AlwaysReloadAssemblies_Test()
        {
            Assert.IsFalse(optionsController.AlwaysReloadAssemblies);
            optionsController.AlwaysReloadAssemblies = true;
            Assert.IsTrue(optionsController.AlwaysReloadAssemblies);
        }

        [Test]
        public void ShowProgressDialogs_Test()
        {
            Assert.IsTrue(optionsController.ShowProgressDialogs);
            optionsController.ShowProgressDialogs = false;
            Assert.IsFalse(optionsController.ShowProgressDialogs);
        }

        [Test]
        public void TestStatusBarStyle_Test()
        {
            Assert.AreEqual(TestStatusBarStyles.Integration, optionsController.TestStatusBarStyle);
            optionsController.TestStatusBarStyle = TestStatusBarStyles.UnitTest;
            Assert.AreEqual(TestStatusBarStyles.UnitTest, optionsController.TestStatusBarStyle);
        }

        [Test]
        public void FailedColor_Test()
        {
            Assert.AreEqual(Color.Red.ToArgb(), optionsController.FailedColor.ToArgb());
            optionsController.FailedColor = Color.Black;
            Assert.AreEqual(Color.Black.ToArgb(), optionsController.FailedColor.ToArgb());
        }

        [Test]
        public void PassedColor_Test()
        {
            Assert.AreEqual(Color.Green.ToArgb(), optionsController.PassedColor.ToArgb());
            optionsController.PassedColor = Color.Black;
            Assert.AreEqual(Color.Black.ToArgb(), optionsController.PassedColor.ToArgb());
        }

        [Test]
        public void InconclusiveColor_Test()
        {
            Assert.AreEqual(Color.Gold.ToArgb(), optionsController.InconclusiveColor.ToArgb());
            optionsController.InconclusiveColor = Color.Black;
            Assert.AreEqual(Color.Black.ToArgb(), optionsController.InconclusiveColor.ToArgb());
        }

        [Test]
        public void SkippedColor_Test()
        {
            Assert.AreEqual(Color.SlateGray.ToArgb(), optionsController.SkippedColor.ToArgb());
            optionsController.SkippedColor = Color.Black;
            Assert.AreEqual(Color.Black.ToArgb(), optionsController.SkippedColor.ToArgb());
        }

        [Test]
        public void SelectedTreeViewCategories_Test()
        {
            Assert.AreEqual(5, optionsController.SelectedTreeViewCategories.Count);
        }

        [Test]
        public void TestRunnerFactories_Test()
        {
            Assert.AreEqual(3, optionsController.TestRunnerFactories.Length);
        }

        [Test]
        public void UnselectedTreeViewCategories_Test()
        {
            Assert.AreEqual(20, optionsController.UnselectedTreeViewCategories.Count);
        }

        [Test]
        public void Cancel_Test()
        {
            string fileName = Paths.SettingsFile;
            fileSystem.Stub(x => x.FileExists(fileName)).Return(true);
            xmlSerialization.Expect(x => x.LoadFromXml<Settings>(fileName)).Return(new Settings());
            optionsController.Cancel();
            xmlSerialization.VerifyAllExpectations();
        }

        [Test]
        public void Save_Test()
        {
            optionsController.Save();
            xmlSerialization.AssertWasCalled(x => x.SaveToXml(Arg<Settings>.Is.Anything, Arg.Is(Paths.SettingsFile)));
        }

        [Test]
        public void Save_Exception_Test()
        {
            Exception ex = new Exception("This exception is testing the UnhandledExceptionPolicy framework. PLEASE IGNORE IT!");
            xmlSerialization.Stub(x => x.SaveToXml(Arg<Settings>.Is.Anything, Arg.Is(Paths.SettingsFile))).Throw(ex);
            EventHandler<CorrelatedExceptionEventArgs> eh = delegate(object sender, CorrelatedExceptionEventArgs e)
            {
                Assert.AreEqual(ex, e.Exception);
            };
            UnhandledExceptionPolicy.ReportUnhandledException += eh;
            optionsController.Save();
            UnhandledExceptionPolicy.ReportUnhandledException -= eh;
        }
    }
}
