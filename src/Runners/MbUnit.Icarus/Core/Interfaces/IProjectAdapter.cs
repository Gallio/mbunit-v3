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

using MbUnit.Icarus.Core.CustomEventArgs;
using MbUnit.Model.Serialization;
using MbUnit.Runner;

namespace MbUnit.Icarus.Core.Interfaces
{
    public interface IProjectAdapter
    {
        event EventHandler<ProjectEventArgs> GetTestTree;
        event EventHandler<EventArgs> RunTests;
        TestModel TestModel { get; set; }
        TestPackage TestPackage { get; set; }
        string StatusText { get; set; }
        int CompletedWorkUnits { get; set; }
        int TotalWorkUnits { get; set; }
        void DataBind();
        void Passed(string testId);
        void Failed(string testId);
        void Ignored(string testId);
        void Skipped(string testId);
    }
}
