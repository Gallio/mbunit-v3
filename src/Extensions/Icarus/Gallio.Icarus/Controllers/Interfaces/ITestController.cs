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
using System.ComponentModel;
using Gallio.Concurrency;
using Gallio.Icarus.Controllers.EventArgs;
using Gallio.Icarus.Models;
using Gallio.Icarus.Models.Interfaces;
using Gallio.Model;
using Gallio.Model.Filters;
using Gallio.Runner.Events;
using Gallio.Runner.Reports;
using Gallio.Runtime.ProgressMonitoring;

namespace Gallio.Icarus.Controllers.Interfaces
{
    public interface ITestController
    {
        BindingList<TestTreeNode> SelectedTests { get; }
        ITestTreeModel Model { get; }
        string TreeViewCategory { get; set; }
        LockBox<Report> Report { get; }
        IList<string> TestFrameworks { get; }
        int TestCount { get; }

        event EventHandler<TestStepFinishedEventArgs> TestStepFinished;
        event EventHandler<ShowSourceCodeEventArgs> ShowSourceCode;

        event EventHandler RunStarted;
        event EventHandler RunFinished;
        event EventHandler LoadStarted;
        event EventHandler LoadFinished;
        event EventHandler UnloadStarted;
        event EventHandler UnloadFinished;

        void ApplyFilter(string filter, IProgressMonitor progressMonitor);
        Filter<ITest> GetCurrentFilter(IProgressMonitor progressMonitor);
        void RefreshTestTree(IProgressMonitor progressMonitor);
        void Reload(IProgressMonitor progressMonitor);
        void Reload(TestPackageConfig config, IProgressMonitor progressMonitor);
        void ResetTests(IProgressMonitor progressMonitor);
        void RunTests(IProgressMonitor progressMonitor);
        void UnloadTestPackage(IProgressMonitor progressMonitor);
        void ViewSourceCode(string testId, IProgressMonitor progressMonitor);
    }
}
