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
using Gallio.Concurrency;
using Gallio.Icarus.ProgressMonitoring.EventArgs;
using Gallio.Model;
using Gallio.Model.Filters;
using Gallio.Model.Serialization;
using Gallio.Runner.Events;
using Gallio.Runner.Reports;

namespace Gallio.Icarus.Services.Interfaces
{
    public interface ITestRunnerService : IDisposable
    {
        event EventHandler<ProgressUpdateEventArgs> ProgressUpdate;
        event EventHandler<TestStepFinishedEventArgs> TestStepFinished;

        LockBox<Report> Report { get; }
        IList<string> TestFrameworks { get; }

        void Initialize();
        void Load(TestPackageConfig testpackage);
        TestModelData Explore();
        void Run();
        void Unload();

        void SetFilter(Filter<ITest> filter);
        void Cancel();
    }
}
