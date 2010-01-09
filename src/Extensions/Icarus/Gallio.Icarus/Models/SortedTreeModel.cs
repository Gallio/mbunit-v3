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
using Aga.Controls.Tree;
using Gallio.Icarus.Events;

namespace Gallio.Icarus.Models
{
    internal class SortedTreeModel : TreeModelDecorator, ISortedTreeModel, Handles<SortTreeEvent>
    {
        private readonly TreeSorter treeSorter = new TreeSorter();

        public SortedTreeModel(IFilteredTreeModel treeModel) 
            : base(treeModel)
        { }

        public override IEnumerable GetChildren(TreePath treePath)
        {
            var list = new ArrayList();
            var res = innerTreeModel.GetChildren(treePath);

            if (res == null)
                return null;

            foreach (object obj in res)
                list.Add(obj);

            list.Sort(treeSorter);
            return list;
        }

        public void Handle(SortTreeEvent message)
        {
            SetSortOrder(message.SortOrder);
        }
        
        private void SetSortOrder(SortOrder sortOrder)
        {
            treeSorter.SetSortOrder(sortOrder);

            // notify treeview
            var treePath = new TreePath(GetRoot());
            OnStructureChanged(new TreePathEventArgs(treePath));
        }

        private class TreeSorter : IComparer
        {
            private readonly CaseInsensitiveComparer comparer = new CaseInsensitiveComparer();

            private SortOrder sortOrder;

            public void SetSortOrder(SortOrder value)
            {
                sortOrder = value;
            }

            public int Compare(object x, object y)
            {
                var left = (Node)x;
                var right = (Node)y;

                int compareResult = comparer.Compare(left.Text, right.Text);

                return sortOrder == SortOrder.Ascending ? compareResult 
                    : -compareResult;
            }
        }
    }
}
