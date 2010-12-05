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
using Gallio.Common.IO;
using Gallio.Icarus.Commands;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Icarus.Tests.Utilities;
using Gallio.Runtime.ProgressMonitoring;
using MbUnit.Framework;
using NHamcrest.Core;
using Rhino.Mocks;

namespace Gallio.Icarus.Tests.Commands
{
    [Category("Commands"), TestsOn(typeof(ConvertSavedReportCommand))]
    internal class ConvertSavedReportCommandTest
    {
        private IReportController reportController;
        private IFileSystem fileSystem;
        private ConvertSavedReportCommand command;

        [SetUp]
        public void SetUp()
        {
            reportController = MockRepository.GenerateStub<IReportController>();
            fileSystem = MockRepository.GenerateStub<IFileSystem>();
            command = new ConvertSavedReportCommand(reportController, fileSystem);
        }

        [Test]
        public void Execute_should_throw_if_filename_is_not_set()
        {
            const string fileName = null;
            const string format = "format";
            command.FileName = fileName;
            command.Format = format;

            var exception = Assert.Throws<ArgumentException>(() => command.Execute(MockProgressMonitor.Instance));

            Assert.That(exception.Message, Is.EqualTo("FileName cannot be null or empty"));
        }

        [Test]
        public void Execute_should_throw_if_format_is_not_set()
        {
            const string fileName = "file";
            const string format = null;
            command.FileName = fileName;
            command.Format = format;

            var exception = Assert.Throws<ArgumentException>(() => command.Execute(MockProgressMonitor.Instance));

            Assert.That(exception.Message, Is.EqualTo("Format cannot be null or empty"));
        }

        [Test]
        public void Execute_should_convert_saved_report()
        {
            const string fileName = "fileName";
            const string format = "format";
            command.FileName = fileName;
            command.Format = format;

            command.Execute(MockProgressMonitor.Instance);

            reportController.AssertWasCalled(rc => rc.ConvertSavedReport(Arg.Is(fileName), 
                Arg.Is(format), Arg<IProgressMonitor>.Is.Anything));
        }

        [Test]
        public void Execute_should_open_report_if_it_is_valid()
        {
            command.FileName = "fileName";
            command.Format = "format";
            const string path = "path/to/generated/report";
            StubGeneratedPath(path);

            command.Execute(MockProgressMonitor.Instance);

            fileSystem.AssertWasCalled(fs => fs.OpenFile(path));
        }

        private void StubGeneratedPath(string path)
        {
            reportController.Stub(rc => rc.ConvertSavedReport(Arg<string>.Is.Anything, Arg<string>.Is.Anything, 
                Arg<IProgressMonitor>.Is.Anything)).Return(path);
            fileSystem.Stub(fs => fs.FileExists(path)).Return(true);
        }
    }
}
