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

using System.Drawing;
using Gallio.Common.IO;
using Gallio.Common.Xml;
using Gallio.Icarus.Controllers;
using Gallio.Icarus.Controls;
using Gallio.Icarus.Tests.Utilities;
using Gallio.Model;
using Gallio.Runner;
using Gallio.Runtime.Logging;
using Gallio.UI.Common.Policies;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.Icarus.Tests.Controllers
{
    [Category("Controllers"), TestsOn(typeof(OptionsController))]
    internal class OptionsControllerTest
    {
        private static OptionsController SetUpOptionsController(Settings settings)
        {
            var fileSystem = MockRepository.GenerateStub<IFileSystem>();
            fileSystem.Stub(fs => fs.FileExists(Arg<string>.Is.Anything)).Return(true);
            var xmlSerializer = MockRepository.GenerateStub<IXmlSerializer>();
            xmlSerializer.Stub(xs => xs.LoadFromXml<Settings>(Arg<string>.Is.Anything)).Return(settings);
            var unhandledExceptionPolicy = MockRepository.GenerateStub<IUnhandledExceptionPolicy>();
            var optionsController = new OptionsController(fileSystem, xmlSerializer, unhandledExceptionPolicy);
            optionsController.Load();
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

        [SyncTest]
        public void TestRunnerFactory_Test()
        {
            var optionsController = SetUpOptionsController(new Settings());
            bool propChangedFlag = false;
            optionsController.TestRunnerFactory.PropertyChanged += (s, e) => 
            {
                propChangedFlag = true;
            };

            Assert.AreEqual(StandardTestRunnerFactoryNames.IsolatedProcess, optionsController.TestRunnerFactory.Value);
            optionsController.TestRunnerFactory.Value = StandardTestRunnerFactoryNames.Local;
            Assert.IsTrue(propChangedFlag);
            Assert.AreEqual(StandardTestRunnerFactoryNames.Local, optionsController.TestRunnerFactory.Value);
        }

        [Test]
        public void AlwaysReloadFiles_Test()
        {
            var optionsController = SetUpOptionsController(new Settings());

            Assert.IsFalse(optionsController.AlwaysReloadFiles);
            optionsController.AlwaysReloadFiles = true;
            Assert.IsTrue(optionsController.AlwaysReloadFiles);
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

            Assert.AreEqual(5, optionsController.SelectedTreeViewCategories.Value.Count);
        }

        [Test]
        public void UnselectedTreeViewCategories_Test()
        {
            var optionsController = SetUpOptionsController(new Settings());

            Assert.AreEqual(typeof(MetadataKeys).GetFields().Length - 4, 
                optionsController.UnselectedTreeViewCategories.Value.Count);
        }

        [Test]
        public void Cancel_should_reload_options()
        {
            var fileSystem = MockRepository.GenerateStub<IFileSystem>();
            fileSystem.Stub(fs => fs.FileExists(Arg<string>.Is.Anything)).Return(true);
            var xmlSerializer = MockRepository.GenerateStub<IXmlSerializer>();
            xmlSerializer.Stub(xs => xs.LoadFromXml<Settings>(Arg<string>.Is.Anything)).Return(new Settings());
            var unhandledExceptionPolicy = MockRepository.GenerateStub<IUnhandledExceptionPolicy>();
            var optionsController = new OptionsController(fileSystem, xmlSerializer, unhandledExceptionPolicy);

            optionsController.Cancel();

            xmlSerializer.AssertWasCalled(xs => xs.LoadFromXml<Settings>(Arg<string>.Is.Anything));
        }

        [Test]
        public void Save_should_save_settings_as_xml()
        {
            var fileSystem = MockRepository.GenerateStub<IFileSystem>();
            fileSystem.Stub(fs => fs.DirectoryExists(Arg<string>.Is.Anything)).Return(true);
            var xmlSerializer = MockRepository.GenerateStub<IXmlSerializer>();
            var unhandledExceptionPolicy = MockRepository.GenerateStub<IUnhandledExceptionPolicy>();
            var optionsController = new OptionsController(fileSystem, xmlSerializer, unhandledExceptionPolicy);

            optionsController.Save();

            xmlSerializer.AssertWasCalled(xs => xs.SaveToXml(Arg<Settings>.Is.Anything, Arg<string>.Is.Anything));
        }

        [Test]
        public void Save_should_create_directory_if_it_does_not_exist()
        {
            var fileSystem = MockRepository.GenerateStub<IFileSystem>();
            fileSystem.Stub(fs => fs.DirectoryExists(Arg<string>.Is.Anything)).Return(false);
            var xmlSerializer = MockRepository.GenerateStub<IXmlSerializer>();
            var unhandledExceptionPolicy = MockRepository.GenerateStub<IUnhandledExceptionPolicy>();
            var optionsController = new OptionsController(fileSystem, xmlSerializer, unhandledExceptionPolicy);

            optionsController.Save();

            fileSystem.AssertWasCalled(fs => fs.CreateDirectory(Arg<string>.Is.Anything));
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
