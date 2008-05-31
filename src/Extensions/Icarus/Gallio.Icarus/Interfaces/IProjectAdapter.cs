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
using System.Collections.Generic;
using System.IO;

using Gallio.Icarus.Core.CustomEventArgs;
using Gallio.Model.Serialization;
using Gallio.Runner.Projects;
using Gallio.Runner.Reports;

namespace Gallio.Icarus.Interfaces
{
    public interface IProjectAdapter
    {
        event EventHandler<GetTestTreeEventArgs> GetTestTree;
        event EventHandler<EventArgs> RunTests;
        event EventHandler<EventArgs> GenerateReport;
        event EventHandler<EventArgs> CancelOperation;
        event EventHandler<SetFilterEventArgs> SetFilter;
        event EventHandler<EventArgs> GetReportTypes;
        event EventHandler<EventArgs> GetTestFrameworks;
        event EventHandler<SaveReportAsEventArgs> SaveReportAs;
        event EventHandler<SingleEventArgs<string>> GetExecutionLog;
        event EventHandler<EventArgs> UnloadTestPackage;

        TestModelData TestModelData { get; set; }
        Project Project { get; set; }
        string TaskName { set; }
        string SubTaskName { set; }
        string ReportPath { set; }
        IList<string> ReportTypes { set; }
        IList<string> TestFrameworks { set; }
        Stream ExecutionLog { set; }
        double CompletedWorkUnits { set; }
        double TotalWorkUnits { set; }
        
        void Update(TestData testData, TestStepRun testStepRun);
        void DataBind();
    }
}
