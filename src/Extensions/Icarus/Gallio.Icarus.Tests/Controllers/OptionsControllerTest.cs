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

using System.ComponentModel;
using System.Drawing;
using Gallio.Common.IO;
using Gallio.Common.Xml;
using Gallio.Icarus.Controllers;
using Gallio.Icarus.Controls;
using Gallio.Icarus.Tests.Utilities;
using Gallio.Runner;
using Gallio.Runtime.Logging;
using MbUnit.Framework;
using System;
using Gallio.Icarus.Utilities;
using Rhino.Mocks;
using Gallio.Icarus.Options;

namespace Gallio.Icarus.Tests.Controllers
{
    [MbUnit.Framework.Category("Controllers"), TestsOn(typeof(OptionsController))]
    internal class OptionsControllerTest
    {
        private static OptionsController SetUpOptionsController(Settings settings)
        {
            var optionsManager = MockRepository.GenerateStub<IOptionsManager>();
            optionsManager.Stub(om => om.Settings).Return(settings);

            var optionsController = new OptionsController();
            optionsController.SetOptionsManager(optionsManager);

            return optionsController;
        }

        [Test]
        public void RestorePreviousSettings_Test()
        {
            var optionsController = SetUpOptionsController(new Settings());

            Assert.IsTrue(optionsController.RestorePreviousSettings);
            optionsController.RestorePreviousSettings = false;
            Assert.IsFalse(optionsController.RestorePreviousSettings);
        }

        [Test]
        public void PluginDirectories_Test()
        {
            var optionsController = SetUpOptionsController(new Settings());

            Assert.AreEqual(0, optionsController.PluginDirectories.Count);
            optionsController.PluginDirectories.Add("test");
            Assert.AreEqual(1, optionsController.PluginDirectories.Count);
            Assert.AreEqual("test", optionsController.PluginDirectories[0]);
        }

        [SyncTest]
        public void TestRunnerFactory_Test()
        {
            var optionsController = SetUpOptionsController(new Settings());

            bool propChangedFlag = false;
            optionsController.PropertyChanged += delegate(object sender, PropertyChangedEventArgs e)
                                                     {
                                                         propChangedFlag = true;
                                                         Assert.AreEqual("TestRunnerFactory", e.PropertyName);
                                                     };

            Assert.AreEqual(StandardTestRunnerFactoryNames.IsolatedProcess, optionsController.TestRunnerFactory);
            optionsController.TestRunnerFactory = StandardTestRunnerFactoryNames.Local;
            Assert.IsTrue(propChangedFlag);
            Assert.AreEqual(StandardTestRunnerFactoryNames.Local, optionsController.TestRunnerFactory);
        }

        [Test]
        public void AlwaysReloadAssemblies_Test()
        {
            var optionsController = SetUpOptionsController(new Settings());

            Assert.IsFalse(optionsController.AlwaysReloadAssemblies);
            optionsController.AlwaysReloadAssemblies = true;
            Assert.IsTrue(optionsController.AlwaysReloadAssemblies);
        }

        [Test]
        public void ShowProgressDialogs_Test()
        {
            var optionsController = SetUpOptionsController(new Settings());

            Assert.IsTrue(optionsController.ShowProgressDialogs);
            optionsController.ShowProgressDialogs = false;
            Assert.IsFalse(optionsController.ShowProgressDialogs);
        }

        [Test]
        public void TestStatusBarStyle_Test()
        {
            var optionsController = SetUpOptionsController(new Settings());

            Assert.AreEqual(TestStatusBarStyles.Integration, optionsController.TestStatusBarStyle);
            optionsController.TestStatusBarStyle = TestStatusBarStyles.UnitTest;
            Assert.AreEqual(TestStatusBarStyles.UnitTest, optionsController.TestStatusBarStyle);
        }

        [Test]
        public void FailedColor_Test()
        {
            var optionsController = SetUpOptionsController(new Settings());

            Assert.AreEqual(Color.Red.ToArgb(), optionsController.FailedColor.ToArgb());
            optionsController.FailedColor = Color.Black;
            Assert.AreEqual(Color.Black.ToArgb(), optionsController.FailedColor.ToArgb());
        }

        [Test]
        public void PassedColor_Test()
        {
            var optionsController = SetUpOptionsController(new Settings());

            Assert.AreEqual(Color.Green.ToArgb(), optionsController.PassedColor.ToArgb());
            optionsController.PassedColor = Color.Black;
            Assert.AreEqual(Color.Black.ToArgb(), optionsController.PassedColor.ToArgb());
        }

        [Test]
        public void InconclusiveColor_Test()
        {
            var optionsController = SetUpOptionsController(new Settings());

            Assert.AreEqual(Color.Gold.ToArgb(), optionsController.InconclusiveColor.ToArgb());
            optionsController.InconclusiveColor = Color.Black;
            Assert.AreEqual(Color.Black.ToArgb(), optionsController.InconclusiveColor.ToArgb());
        }

        [Test]
        public void SkippedColor_Test()
        {
            var optionsController = SetUpOptionsController(new Settings());

            Assert.AreEqual(Color.SlateGray.ToArgb(), optionsController.SkippedColor.ToArgb());
            optionsController.SkippedColor = Color.Black;
            Assert.AreEqual(Color.Black.ToArgb(), optionsController.SkippedColor.ToArgb());
        }

        [Test]
        public void SelectedTreeViewCategories_Test()
        {
            var optionsController = SetUpOptionsController(new Settings());

            Assert.AreEqual(5, optionsController.SelectedTreeViewCategories.Count);
        }

        [Test]
        public void UnselectedTreeViewCategories_Test()
        {
            var optionsController = SetUpOptionsController(new Settings());

            Assert.AreEqual(20, optionsController.UnselectedTreeViewCategories.Count);
        }

        [Test]
        public void Cancel_should_reload_options()
        {
            var optionsManager = MockRepository.GenerateStub<IOptionsManager>();
            optionsManager.Stub(om => om.Settings).Return(new Settings());
            var optionsController = new OptionsController();
            optionsController.SetOptionsManager(optionsManager);

            optionsController.Cancel();

            optionsManager.AssertWasCalled(om => om.Load());
        }

        [Test]
        public void Save_should_call_save_on_OptionsManager()
        {
            var optionsManager = MockRepository.GenerateStub<IOptionsManager>();
            optionsManager.Stub(om => om.Settings).Return(new Settings());
            var optionsController = new OptionsController();
            optionsController.SetOptionsManager(optionsManager);

            optionsController.Save();

            optionsManager.AssertWasCalled(om => om.Save());
        }

        [Test]
        public void GenerateReportAfterTestRun_Test()
        {
            var optionsController = SetUpOptionsController(new Settings());

            Assert.IsTrue(optionsController.GenerateReportAfterTestRun);
            optionsController.GenerateReportAfterTestRun = false;
            Assert.IsFalse(optionsController.GenerateReportAfterTestRun);
        }

        [Test]
        public void Location_Test()
        {
            var optionsController = SetUpOptionsController(new Settings());

            Point p = new Point(0, 0);
            optionsController.Location = p;
            Assert.AreEqual(p, optionsController.Location);
        }

        [Test]
        public void RecentProjects_Test()
        {
            var settings = new Settings();
            settings.RecentProjects.AddRange(new[] { "one", "two" });

            var optionsController = SetUpOptionsController(settings);
 
            Assert.AreEqual(settings.RecentProjects.Count, optionsController.RecentProjects.Count);
            Assert.AreElementsEqual(settings.RecentProjects, optionsController.RecentProjects.Items);
        }

        [Test]
        public void RunTestsAfterReload_Test()
        {
            var optionsController = SetUpOptionsController(new Settings());

            Assert.IsFalse(optionsController.RunTestsAfterReload);
            optionsController.RunTestsAfterReload = true;
            Assert.IsTrue(optionsController.RunTestsAfterReload);
        }

        [Test]
        public void Size_Test()
        {
            var optionsController = SetUpOptionsController(new Settings());
            Size size = new Size(0, 0);

            optionsController.Size = size;
            Assert.AreEqual(size, optionsController.Size);
        }

        [Test]
        public void UpdateDelay_Test()
        {
            var optionsController = SetUpOptionsController(new Settings());
            Assert.AreEqual(1000, optionsController.UpdateDelay);
        }

        [Test]
        public void AnnotationShowErrors_Test()
        {
            var optionsController = SetUpOptionsController(new Settings());
            
            Assert.AreEqual(false, optionsController.AnnotationsShowErrors);
            optionsController.AnnotationsShowErrors = true;
            Assert.AreEqual(true, optionsController.AnnotationsShowErrors);
        }

        [Test]
        public void AnnotationShowInfos_Test()
        {
            var optionsController = SetUpOptionsController(new Settings());

            Assert.AreEqual(false, optionsController.AnnotationsShowInfos);
            optionsController.AnnotationsShowInfos = true;
            Assert.AreEqual(true, optionsController.AnnotationsShowInfos);
        }

        [Test]
        public void AnnotationShowWarnings_Test()
        {
            var optionsController = SetUpOptionsController(new Settings());

            Assert.AreEqual(false, optionsController.AnnotationsShowWarnings);
            optionsController.AnnotationsShowWarnings = true;
            Assert.AreEqual(true, optionsController.AnnotationsShowWarnings);
        }

        [Test]
        public void MinLogSeverity_should_return_value_from_settings()
        {
            var settings = new Settings { MinLogSeverity = LogSeverity.Error };
            var optionsController = SetUpOptionsController(settings);

            Assert.AreEqual(LogSeverity.Error, optionsController.MinLogSeverity);
            optionsController.MinLogSeverity = LogSeverity.Debug;
            Assert.AreEqual(LogSeverity.Debug, optionsController.MinLogSeverity);
        }
    }
}
