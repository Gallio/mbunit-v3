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
using System.Collections.ObjectModel;
using Aga.Controls.Tree;
using Gallio.Icarus.Models;
using Gallio.Model;
using Gallio.Model.Filters;
using Gallio.Model.Serialization;
using Gallio.Runner.Reports;

namespace Gallio.Icarus.Models.Interfaces
{
    public interface ITestTreeModel : ITreeModel
    {
        bool FilterPassed { get; set; }
        bool FilterFailed { get; set; }
        bool FilterSkipped { get; set; }
        int TestCount { get; }
        TestTreeNode Root { get; }
        bool SortAsc { get; set; }
        bool SortDesc { get; set; }
        int Passed { get; }
        int Failed { get; }
        int Skipped { get; }
        int Inconclusive { get; }

        void ApplyFilter(Filter<ITest> filter);
        void BuildTestTree(TestModelData testModelData, string treeViewCategory);
        Node FindNode(TreePath path);
        Filter<ITest> GetCurrentFilter();
        TreePath GetPath(Node node);
        void ResetTestStatus();
        void UpdateTestStatus(TestData testData, TestStepRun testStepRun);
    }
}
