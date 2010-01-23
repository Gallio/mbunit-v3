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

namespace Gallio.Icarus.Models
{
    internal class SortedTreeModel : TreeModelDecorator, ISortedTreeModel, Handles<SortTreeEvent>
    {
        private readonly TestTreeNodeComparer comparer = new TestTreeNodeComparer();

        public SortedTreeModel(ITestStatusFilteredTreeModel treeModel) 
            : base(treeModel)
        { }

        public override IEnumerable GetChildren(TreePath treePath)
        {
            var list = new List<TestTreeNode>();
            var res = innerTreeModel.GetChildren(treePath);

            if (res == null)
                return null;

            foreach (object obj in res)
                list.Add((TestTreeNode)obj);

            list.Sort(comparer);
            return list;
        }

        public void Handle(SortTreeEvent @event)
        {
            SetSortOrder(@event.SortOrder);
        }
        
        private void SetSortOrder(SortOrder sortOrder)
        {
            comparer.SetSortOrder(sortOrder);
            OnStructureChanged();
        }

        private class TestTreeNodeComparer : IComparer<TestTreeNode>
        {
            private SortOrder sortOrder;

            public void SetSortOrder(SortOrder value)
            {
                sortOrder = value;
            }

            public int Compare(TestTreeNode x, TestTreeNode y)
            {
                int compareResult = x.CompareTo(y);
                return sortOrder == SortOrder.Ascending ? compareResult 
                    : -compareResult;
            }
        }
    }
}
