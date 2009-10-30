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
using Gallio.Common.Concurrency;
using Gallio.Icarus.Commands;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Icarus.Reports;
using Gallio.Icarus.Tests.Utilities;
using Gallio.Runner.Reports.Schema;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;
using Rhino.Mocks;

namespace Gallio.Icarus.Tests.Commands
{
    [Category("Commands"), TestsOn(typeof(GenerateReportCommand))]
    internal class GenerateReportCommandTest
    {
        [Test]
        public void Execute_should_call_GenerateReport_on_ReportController()
        {
            var testController = MockRepository.GenerateStub<ITestController>();
            var report = new Report();
            testController.Stub(tc => tc.ReadReport(null)).IgnoreArguments().Do(
                (Action<ReadAction<Report>>)(action => action(report)));
            var reportController = MockRepository.GenerateStub<IReportController>();
            var reportOptions = new ReportOptions("", "");
            var generateReportCommand = new GenerateReportCommand(testController, reportController)
                                            { ReportOptions = reportOptions };
            var progressMonitor = MockProgressMonitor.Instance;
            
            generateReportCommand.Execute(progressMonitor);

            reportController.AssertWasCalled(rc => rc.GenerateReport(report, reportOptions, 
                progressMonitor));
        }

        [Test]
        public void ReportOptions_should_return_set_value()
        {
            var testController = MockRepository.GenerateStub<ITestController>();
            var reportController = MockRepository.GenerateStub<IReportController>();
            var reportOptions = new ReportOptions("", "");
            var generateReportCommand = new GenerateReportCommand(testController, reportController) 
                { ReportOptions = reportOptions };

            Assert.AreEqual(reportOptions, generateReportCommand.ReportOptions);
        }
    }
}
