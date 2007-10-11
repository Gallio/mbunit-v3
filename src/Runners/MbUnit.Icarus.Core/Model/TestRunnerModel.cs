// Copyright 2007 MbUnit Project - http://www.mbunit.com/
// Portions Copyright 2000-2004 Jonathan De Halleux, Jamie Cansdale
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


using MbUnit.Core.Harness;
using MbUnit.Core.Runner;
using MbUnit.Core.Runner.Monitors;
using MbUnit.Icarus.Core.Interfaces;
using MbUnit.Icarus.Core.ProgressMonitoring;
using MbUnit.Model.Serialization;

namespace MbUnit.Icarus.Core.Model
{
    public class TestRunnerModel : ITestRunnerModel
    {
        private ReportMonitor reportMonitor;

        public TestModel LoadUpAssembly(IProjectPresenter presenter, TestPackage testpackage)
        {
            // set up report monitor
            reportMonitor = new ReportMonitor();
            reportMonitor.Attach(presenter.Runner);

            presenter.Runner.LoadPackage(testpackage, new StatusStripProgressMonitor(presenter));
            presenter.Runner.BuildTemplates(new StatusStripProgressMonitor(presenter));
            presenter.Runner.BuildTests(new StatusStripProgressMonitor(presenter));

            return presenter.Runner.TestModel;
        }

        public void RunTests(IProjectPresenter presenter)
        {
            TestRunnerMonitor testRunnerMonitor = new TestRunnerMonitor(presenter, reportMonitor);
            testRunnerMonitor.Attach(presenter.Runner);
            presenter.Runner.Run(new StatusStripProgressMonitor(presenter));
        }
    }
}