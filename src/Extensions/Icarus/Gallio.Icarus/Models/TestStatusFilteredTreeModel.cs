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

using System.Collections;
using System.Collections.Generic;
using Aga.Controls.Tree;
using Gallio.Icarus.Events;
using Gallio.Model;

namespace Gallio.Icarus.Models
{
    public class TestStatusFilteredTreeModel : TreeModelDecorator, ITestStatusFilteredTreeModel,
        Handles<FilterTestStatusEvent>
    {
        private readonly List<TestStatus> filteredStatuses = new List<TestStatus>();

        public TestStatusFilteredTreeModel(IFilteredTreeModel innerTreeModel) 
            : base(innerTreeModel)
        { }

        public override IEnumerable GetChildren(TreePath treePath)
        {
            foreach (var child in GetChildrenFromBase(treePath))
            {
                var node = child as TestTreeNode;
                
                if (node == null)
                    continue;

                if (NodeShouldNotBeFiltered(node))
                    yield return child;
            }
            yield break;
        }

        private bool NodeShouldNotBeFiltered(TestTreeNode node)
        {
            if (node.TestKind != TestKinds.Test)
                return true;

            if (node.TestStatus.HasValue == false)
                return true;

            return !filteredStatuses.Contains(node.TestStatus.Value);
        }

        private IEnumerable GetChildrenFromBase(TreePath treePath)
        {
            return base.GetChildren(treePath);
        }

        public void Handle(FilterTestStatusEvent @event)
        {
            var testStatus = @event.TestStatus;

            if (filteredStatuses.Contains(testStatus))
            {
                filteredStatuses.Remove(testStatus);
            }
            else
            {
                filteredStatuses.Add(testStatus);
            }

            OnStructureChanged();
        }
    }
}
