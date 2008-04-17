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
using Gallio.Icarus.Core.CustomEventArgs;
using Gallio.Model;
using Aga.Controls.Tree;
using System.Collections.ObjectModel;
using System.Collections;

namespace Gallio.Icarus.Controls.Interfaces
{
    public interface ITestTreeModel : ITreeModel
    {
        Collection<Node> Nodes { get; }
        bool FilterPassed { set; }
        bool FilterFailed { set; }
        bool FilterSkipped { set; }
        int TestCount { get; }
        TestTreeNode Root { get; }

        Node FindNode(TreePath path);
        TreePath GetPath(Node node);
        void OnStructureChanged(TreePathEventArgs args);
        void ResetTestStatus();
        void UpdateTestStatus(string testId, TestStatus testStatus);
        void FilterTree();
        void OnTestCountChanged(EventArgs e);
        void OnTestResult(TestResultEventArgs e);

        event EventHandler<EventArgs> TestCountChanged;
        event EventHandler<TestResultEventArgs> TestResult;
    }
}
