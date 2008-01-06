// Copyright 2008 MbUnit Project - http://www.mbunit.com/
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

using System;
using Gallio.Icarus.Core.CustomEventArgs;
using Gallio.Model;
using Gallio.Model.Serialization;
using Gallio.Runner;
using Gallio.Runner.Reports;

namespace Gallio.Icarus.Core.Interfaces
{
    public interface IProjectPresenter
    {
        string StatusText { set; }
        int CompletedWorkUnits { set; }
        int TotalWorkUnits { set; }
        string ReportPath { set; }
        ITestRunner TestRunner { get; }
        void GetTestTree(object sender, GetTestTreeEventArgs e);
        void RunTests(object sender, EventArgs e);
        void StopTests(object sender, EventArgs e);
        void SetFilter(object sender, SetFilterEventArgs e);
        void GetReportTypes(object sender, EventArgs e);
        void SaveReportAs(object sender, SaveReportAsEventArgs e);
        void GetAvailableLogStreams(object sender, SingleStringEventArgs e);
        void Update(TestData testData, TestStepRun testStepRun);
    }
}
