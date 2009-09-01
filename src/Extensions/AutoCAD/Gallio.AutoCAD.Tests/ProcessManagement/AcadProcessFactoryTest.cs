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
using Gallio.AutoCAD.Preferences;
using Gallio.AutoCAD.ProcessManagement;
using Gallio.Common.Concurrency;
using Gallio.Common.IO;
using Gallio.Model.Isolation;
using Gallio.Runtime.Debugging;
using Gallio.Runtime.Logging;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.AutoCAD.Tests.ProcessManagement
{
    [TestsOn(typeof(AcadProcessFactory))]
    public class AcadProcessFactoryTest
    {
        private IAcadPreferenceManager preferenceManager;
        private AcadProcessFactory factory;

        [SetUp]
        public void SetUp()
        {
            var logger = MockRepository.GenerateStub<ILogger>();

            var fileSystem = MockRepository.GenerateStub<IFileSystem>();
            fileSystem.Stub(x => x.FileExists(Arg.Is(@"c:\path\to\acad.exe"))).Return(true);
            fileSystem.Stub(x => x.FileExists(Arg.Is(@"c:\most\recently\used\acad.exe"))).Return(true);
            fileSystem.Stub(x => x.DirectoryExists(Arg.Is(@"c:\working\dir\"))).Return(true);

            var process = MockRepository.GenerateStub<IProcess>();
            var processFinder = MockRepository.GenerateStub<IProcessFinder>();
            processFinder.Stub(x => x.GetProcessesByName(Arg.Is("acad"))).Return(new[] { process });

            var debuggerManager = MockRepository.GenerateStub<IDebuggerManager>();
            var processCreator = MockRepository.GenerateStub<IProcessCreator>();

            preferenceManager = MockRepository.GenerateStub<IAcadPreferenceManager>();

            var acadLocator = MockRepository.GenerateStub<IAcadLocator>();
            acadLocator.Stub(x => x.GetMostRecentlyUsed()).Return(@"c:\most\recently\used\acad.exe");

            factory = new AcadProcessFactory(logger, fileSystem, processFinder,
                processCreator, debuggerManager, preferenceManager, acadLocator);
        }

        [Test]
        public void CreateProcess_WhenAttachPropertyProvided_ReturnsExistingProcess()
        {
            var options = new TestIsolationOptions();
            options.AddProperty("AcadAttachToExisting", "true");

            var process = factory.CreateProcess(options);

            Assert.IsInstanceOfType<ExistingAcadProcess>(process);
        }

        [Test]
        public void CreateProcess_WhenAttachAndExePropertiesProvided_ReturnsExistingProcess()
        {
            var options = new TestIsolationOptions();
            options.AddProperty("AcadAttachToExisting", "true");
            options.AddProperty("AcadExePath", @"c:\path\to\acad.exe");

            var process = factory.CreateProcess(options);

            Assert.IsInstanceOfType<ExistingAcadProcess>(process);
        }

        [Test]
        public void CreateProcess_WhenAttachPropertyProvided_OverridesPreferenceManager()
        {
            preferenceManager.StartupAction = StartupAction.StartMostRecentlyUsed;
            var options = new TestIsolationOptions();
            options.AddProperty("AcadAttachToExisting", @"true");

            var process = factory.CreateProcess(options);

            Assert.IsInstanceOfType<ExistingAcadProcess>(process);
        }

        [Test]
        public void CreateProcess_WhenExePropertyProvided_SetsProcessFileName()
        {
            var options = new TestIsolationOptions();
            options.AddProperty("AcadExePath", @"c:\path\to\acad.exe");

            var process = factory.CreateProcess(options);

            Assert.IsInstanceOfType<CreatedAcadProcess>(process);
            Assert.AreEqual(@"c:\path\to\acad.exe", ((CreatedAcadProcess)process).FileName);
        }

        [Test]
        public void CreateProcess_WhenExePropertyProvided_OverridesPreferenceManager()
        {
            preferenceManager.StartupAction = StartupAction.AttachToExisting;
            var options = new TestIsolationOptions();
            options.AddProperty("AcadExePath", @"c:\path\to\acad.exe");

            var process = factory.CreateProcess(options);

            Assert.IsInstanceOfType<CreatedAcadProcess>(process);
            Assert.AreEqual(@"c:\path\to\acad.exe", ((CreatedAcadProcess)process).FileName);
        }

        [Test]
        public void CreateProcess_WhenExePropertyIsInvalid_ThrowsArgumentException()
        {
            var options = new TestIsolationOptions();
            options.AddProperty("AcadExePath", new string(Path.GetInvalidPathChars()));

            Assert.Throws<ArgumentException>(() => factory.CreateProcess(options));
        }

        [Test]
        public void CreateProcess_WhenExePropertyRefersToMissingFile_ThrowsFileNotFoundException()
        {
            var options = new TestIsolationOptions();
            options.AddProperty("AcadExePath", @"c:\path\to\missing\acad.exe");

            Assert.Throws<FileNotFoundException>(() => factory.CreateProcess(options));
        }

        [Test]
        public void CreateProcess_WhenOptionsArgumentIsNull_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => factory.CreateProcess(null));
        }

        [Test]
        public void CreateProcess_WhenCommandLineArgumentsPreferenceProvided_SetsProcessArguments()
        {
            preferenceManager.CommandLineArguments = "arguments";
            var options = new TestIsolationOptions();

            var process = factory.CreateProcess(options);

            Assert.IsInstanceOfType<CreatedAcadProcess>(process);
            Assert.AreEqual("arguments", ((CreatedAcadProcess)process).Arguments);
        }
        
        [Test]
        public void CreateProcess_WhenStartupActionPreferenceIsAttachToExisting_ReturnsExistingProcess()
        {
            preferenceManager.StartupAction = StartupAction.AttachToExisting;
            var options = new TestIsolationOptions();

            var process = factory.CreateProcess(options);

            Assert.IsInstanceOfType<ExistingAcadProcess>(process);
        }

        [Test]
        public void CreateProcess_WhenStartupActionPreferenceIsStartMru_SetsFileName()
        {
            preferenceManager.StartupAction = StartupAction.StartMostRecentlyUsed;
            var options = new TestIsolationOptions();

            var process = factory.CreateProcess(options);

            Assert.IsInstanceOfType<CreatedAcadProcess>(process);
            Assert.AreEqual(@"c:\most\recently\used\acad.exe", ((CreatedAcadProcess)process).FileName);
        }

        [Test]
        public void CreateProcess_WhenStartupActionPreferenceIsStartUserSpecified_SetsFileName()
        {
            preferenceManager.StartupAction = StartupAction.StartUserSpecified;
            preferenceManager.UserSpecifiedExecutable = @"c:\path\to\acad.exe";
            var options = new TestIsolationOptions();

            var process = factory.CreateProcess(options);

            Assert.IsInstanceOfType<CreatedAcadProcess>(process);
            Assert.AreEqual(@"c:\path\to\acad.exe", ((CreatedAcadProcess)process).FileName);
        }

        [Test]
        public void CreateProcess_WhenUserSpecifiedExecutablePreferenceIsNull_ThrowsArgumentException()
        {
            preferenceManager.StartupAction = StartupAction.StartUserSpecified;
            preferenceManager.UserSpecifiedExecutable = null;
            var options = new TestIsolationOptions();

            Assert.Throws<ArgumentException>(() => factory.CreateProcess(options));
        }

        [Test]
        public void CreateProcess_WhenUserSpecifiedExecutablePreferenceIsInvalid_ThrowsArgumentException()
        {
            preferenceManager.StartupAction = StartupAction.StartUserSpecified;
            preferenceManager.UserSpecifiedExecutable = new string(Path.GetInvalidPathChars());
            var options = new TestIsolationOptions();

            Assert.Throws<ArgumentException>(() => factory.CreateProcess(options));
        }

        [Test]
        public void CreateProcess_WhenUserSpecifiedExecutablePreferenceRefersToMissingFile_ThrowsFileNotFoundException()
        {
            preferenceManager.StartupAction = StartupAction.StartUserSpecified;
            preferenceManager.UserSpecifiedExecutable = @"c:\missing\path\to\acad.exe";
            var options = new TestIsolationOptions();

            Assert.Throws<FileNotFoundException>(() => factory.CreateProcess(options));
        }

        [Test]
        public void CreateProcess_WhenWorkingDirectoryPreferenceProvided_SetsWorkingDirectory()
        {
            preferenceManager.WorkingDirectory = @"c:\working\dir\";
            var options = new TestIsolationOptions();

            var process = factory.CreateProcess(options);

            Assert.IsInstanceOfType<CreatedAcadProcess>(process);
            Assert.AreEqual(@"c:\working\dir\", ((CreatedAcadProcess)process).WorkingDirectory);
        }

        [Test]
        public void CreateProcess_WhenWorkingDirectoryPreferenceIsInvalid_ThrowsArgumentException()
        {
            preferenceManager.WorkingDirectory = new string(Path.GetInvalidPathChars());
            var options = new TestIsolationOptions();

            Assert.Throws<ArgumentException>(() => factory.CreateProcess(options));
        }

        [Test]
        public void CreateProcess_WhenWorkingDirectoryPreferenceRefersToMissingDirectory_ThrowsDirectoryNotFoundException()
        {
            preferenceManager.WorkingDirectory = @"c:\missing\dir\";
            var options = new TestIsolationOptions();

            Assert.Throws<DirectoryNotFoundException>(() => factory.CreateProcess(options));
        }
    }
}
