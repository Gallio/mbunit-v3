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

using MbUnit.Runner;

using MbUnit.Icarus.Core.CustomEventArgs;

namespace MbUnit.Icarus.Core.Interfaces
{
    public interface IProjectPresenter
    {
        string StatusText { set; }
        int CompletedWorkUnits { set; }
        int TotalWorkUnits { set; }
        ITestRunner TestRunner { get; }
        void GetTestTree(object sender, ProjectEventArgs e);
        void RunTests(object sender, EventArgs e);
        void Passed(string testId);
        void Failed(string testId);
        void Ignored(string testId);
        void Skipped(string testId);
    }
}