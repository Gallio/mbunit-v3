// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
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

using Aga.Controls.Tree;
using Gallio.Icarus.TreeBuilders;
using Gallio.Model;
using Gallio.Model.Filters;
using Gallio.Model.Schema;
using Gallio.Runner.Reports.Schema;
using Gallio.Runtime.ProgressMonitoring;
using Gallio.UI.DataBinding;

namespace Gallio.Icarus.Models
{
    public interface ITestTreeModel : ITreeModel
    {
        Observable<int> TestCount { get; }
        TestTreeNode Root { get; }

        void ApplyFilterSet(FilterSet<ITestDescriptor> filterSet);
        void BuildTestTree(IProgressMonitor progressMonitor, TestModelData testModelData, 
            TreeBuilderOptions options);
        FilterSet<ITestDescriptor> GenerateFilterSetFromSelectedTests();
        void RemoveFilter(TestStatus testStatus);
        void ResetTestStatus(IProgressMonitor progressMonitor);
        void SetFilter(TestStatus testStatus);
        void UpdateTestCount();
        void TestStepFinished(TestData testData, TestStepRun testStepRun);
    }
}