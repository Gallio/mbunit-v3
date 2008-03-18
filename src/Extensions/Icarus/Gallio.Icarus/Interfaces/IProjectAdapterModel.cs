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

using System.Collections.Generic;
using System.Windows.Forms;

using Aga.Controls.Tree;

using Gallio.Model;
using Gallio.Model.Filters;
using Gallio.Model.Serialization;
using Gallio.Runner.Reports;

namespace Gallio.Icarus.Interfaces
{
    public interface IProjectAdapterModel
    {
        ITreeModel TreeModel { get; }
        void BuildTestTree(TestModelData testModelData, string mode);
        ListViewItem[] BuildAssemblyList(List<string> assemblyList);
        void Update(TestData testData, TestStepRun testStepRun);
        Filter<ITest> CreateFilter();
        void ApplyFilter(Filter<ITest> filter);
        void ResetTestStatus();
    }
}
