// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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
using System.IO;

using Gallio.Model;
using Gallio.Model.Filters;
using Gallio.Model.Serialization;

namespace Gallio.Icarus.Core.Interfaces
{
    public interface ITestRunnerModel : IDisposable
    {
        IProjectPresenter ProjectPresenter { set; }

        void Initialize();
        void Load(TestPackageConfig testpackage);
        TestModelData Explore();
        void Run();
        void Unload();

        Stream GetExecutionLog(string testId, TestModelData testModelData);
        IList<string> GetReportTypes();
        IList<string> GetTestFrameworks();
        string GenerateReport();
        void SaveReportAs(string fileName, string format);
        void SetFilter(Filter<ITest> filter);
        void StopTests();
    }
}
