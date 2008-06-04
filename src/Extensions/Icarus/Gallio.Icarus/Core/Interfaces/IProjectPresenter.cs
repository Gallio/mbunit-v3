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

using System;
using Gallio.Icarus.Core.CustomEventArgs;
using Gallio.Model;
using Gallio.Model.Serialization;
using Gallio.Runner;
using Gallio.Runner.Reports;
using Gallio.Runner.Events;

namespace Gallio.Icarus.Core.Interfaces
{
    public interface IProjectPresenter
    {
        void projectAdapter_GetTestTree(object sender, GetTestTreeEventArgs e);
        void projectAdapter_RunTests(object sender, EventArgs e);
        void projectAdapter_GenerateReport(object sender, EventArgs e);
        void projectAdapter_CancelOperation(object sender, EventArgs e);
        void projectAdapter_SetFilter(object sender, SetFilterEventArgs e);
        void projectAdapter_GetReportTypes(object sender, EventArgs e);
        void projectAdapter_SaveReportAs(object sender, SaveReportAsEventArgs e);
        void projectAdapter_GetTestFrameworks(object sender, EventArgs e);
        void projectAdapter_GetExecutionLog(object sender, SingleEventArgs<string> e);
        void projectAdapter_UnloadTestPackage(object sender, EventArgs e);

        void testRunnerModel_TestStepFinished(object sender, TestStepFinishedEventArgs e);
        void testRunnerModel_ProgressUpdate(object sender, ProgressUpdateEventArgs e);
    }
}
