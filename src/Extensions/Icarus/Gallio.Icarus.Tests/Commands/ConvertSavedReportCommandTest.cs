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

using Gallio.Common.IO;
using Gallio.Icarus.Commands;
using Gallio.Icarus.Controllers.Interfaces;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.Icarus.Tests.Commands
{
    [Category("Commands"), TestsOn(typeof(ConvertSavedReportCommand))]
    internal class ConvertSavedReportCommandTest
    {
        [Test]
        public void Execute_should_call_Convert_on_ReportController()
        {
            var reportController = MockRepository.GenerateStub<IReportController>();
            const string fileName = "fileName";
            const string format = "format";
            var progressMonitor = MockProgressMonitor.GetMockProgressMonitor();
            const string generatedFile = "generatedFile";
            reportController.Stub(rc => rc.ConvertSavedReport(fileName, format, progressMonitor)).Return(generatedFile);
            var fileSystem = MockRepository.GenerateStub<IFileSystem>();
            fileSystem.Stub(fs => fs.FileExists(generatedFile)).Return(true);
            var command = new ConvertSavedReportCommand(reportController, fileName, format, fileSystem);

            command.Execute(progressMonitor);

            fileSystem.AssertWasCalled(fs => fs.OpenFile(generatedFile));
        }
    }
}
