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

using Gallio.Icarus.Commands;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Icarus.Tests.Utilities;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.Icarus.Tests.Commands
{
    [Category("Commands"), TestsOn(typeof(DeleteReportCommand))]
    internal class DeleteReportCommandTest
    {
        [Test]
        public void Execute_should_call_DeleteReport_on_ReportController()
        {
            var reportController = MockRepository.GenerateStub<IReportController>();
            const string fileName = "fileName";
            var deleteReportCommand = new DeleteReportCommand(reportController) { FileName = fileName };
            var progressMonitor = MockProgressMonitor.Instance;
            
            deleteReportCommand.Execute(progressMonitor);

            reportController.AssertWasCalled(rc => rc.DeleteReport(fileName, progressMonitor));
        }

        [Test]
        public void FileName_should_return_correct_value()
        {
            var reportController = MockRepository.GenerateStub<IReportController>();
            var deleteReportCommand = new DeleteReportCommand(reportController);
            const string fileName = "fileName";

            deleteReportCommand.FileName = fileName;

            Assert.AreEqual(fileName, deleteReportCommand.FileName);
        }
    }
}
