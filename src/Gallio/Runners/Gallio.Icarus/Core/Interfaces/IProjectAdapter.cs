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

using System;
using System.Collections.Generic;
using Gallio.Icarus.Core.CustomEventArgs;
using Gallio.Model.Serialization;
using Gallio.Runner;

namespace Gallio.Icarus.Core.Interfaces
{
    public interface IProjectAdapter
    {
        event EventHandler<ProjectEventArgs> GetTestTree;
        event EventHandler<EventArgs> RunTests;
        event EventHandler<EventArgs> StopTests;
        event EventHandler<SetFilterEventArgs> SetFilter;
        event EventHandler<SingleStringEventArgs> GetLogStream;
        event EventHandler<EventArgs> GetReportTypes;
        event EventHandler<SaveReportAsEventArgs> SaveReportAs;
        TestModelData TestModelData { set; }
        string StatusText { set; }
        string LogBody { set; }
        string ReportPath { set; }
        IList<string> ReportTypes { set; }
        Exception Exception { set; }
        int CompletedWorkUnits { set; }
        int TotalWorkUnits { set; }
        void DataBind();
        void Passed(string testId);
        void Failed(string testId);
        void Ignored(string testId);
        void Skipped(string testId);
    }
}
