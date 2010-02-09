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
using System.IO;
using Gallio.Common.IO;
using Gallio.Runner.Reports;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.Tests.Runner.Reports
{
    [TestFixture]
    [TestsOn(typeof(ReportContainerFactory))]
    public class ReportContainerFactoryTest
    {
        [Test]
        [ExpectedArgumentNullException]
        public void Constructs_with_null_fileSystem_should_throw_exception()
        {
            new ReportContainerFactory(null, @"C:\Directory", "name");
        }

        [Test]
        [ExpectedArgumentNullException]
        public void Constructs_with_null_reportDirectory_should_throw_exception()
        {
            var mockFileSystem = MockRepository.GenerateStub<IFileSystem>();
            new ReportContainerFactory(mockFileSystem, null, "name");
        }
        [Test]
        [ExpectedArgumentNullException]
        public void Constructs_with_null_reportName_should_throw_exception()
        {
            var mockFileSystem = MockRepository.GenerateStub<IFileSystem>();
            new ReportContainerFactory(mockFileSystem, @"C:\Directory", null);
        }

        [Test]
        public void MakeForSaving_with_flat_file()
        {
            var mockFileSystem = MockRepository.GenerateStub<IFileSystem>();
            var factory = new ReportContainerFactory(mockFileSystem, @"C:\Directory", "name");
            var container = (AbstractReportContainer)factory.MakeForSaving(ReportArchive.Flat);
            Assert.IsInstanceOfType<FileSystemReportContainer>(container);
            Assert.AreEqual("name", container.ReportName);
            Assert.AreEqual(@"C:\Directory\", container.ReportDirectory);
        }

        [Test]
        public void MakeForSaving_with_archive()
        {
            var mockFileSystem = MockRepository.GenerateStub<IFileSystem>();
            var factory = new ReportContainerFactory(mockFileSystem, @"C:\Directory", "name");
            var container = (AbstractReportContainer)factory.MakeForSaving(ReportArchive.Zip);
            Assert.IsInstanceOfType<ArchiveReportContainer>(container);
            Assert.AreEqual("name", container.ReportName);
            Assert.AreEqual(@"C:\Directory\", container.ReportDirectory);
        }

        [Test]
        public void MakeForReading_from_flat_file()
        {
            var mockFileSystem = MockRepository.GenerateMock<IFileSystem>();
            mockFileSystem.Expect(x => x.FileExists(@"C:\directory\name.zip")).Return(false);
            var factory = new ReportContainerFactory(mockFileSystem, @"C:\directory", "name");
            var container = factory.MakeForReading();
            Assert.IsInstanceOfType<FileSystemReportContainer>(container);
            mockFileSystem.VerifyAllExpectations();
        }

        [Test]
        public void MakeForReading_from_archive_file()
        {
            var mockFileSystem = MockRepository.GenerateMock<IFileSystem>();
            mockFileSystem.Expect(x => x.FileExists(@"C:\directory\name.zip")).Return(true);
            var factory = new ReportContainerFactory(mockFileSystem, @"C:\directory", "name");
            var container = factory.MakeForReading();
            Assert.IsInstanceOfType<ArchiveReportContainer>(container);
            mockFileSystem.VerifyAllExpectations();
        }
    }
}
