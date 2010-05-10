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

using System.Collections.Generic;
using System.Windows.Forms;
using Aga.Controls.Tree;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Icarus.Models;
using Gallio.Icarus.Models.TestTreeNodes;
using Gallio.Icarus.TreeBuilders;
using Gallio.Model.Filters;

namespace Gallio.Icarus.Services
{
    public class FilterService : IFilterService
    {
        private readonly ITestTreeModel testTreeModel;
        private readonly IOptionsController optionsController;

        public FilterService(ITestTreeModel testTreeModel, IOptionsController optionsController)
        {
            this.testTreeModel = testTreeModel;
            this.optionsController = optionsController;
        }

        public void ApplyFilterSet(FilterSet<ITestDescriptor> filterSet)
        {
            if (testTreeModel.Root == null)
                return;

            var root = testTreeModel.Root;

            if (filterSet.IsEmpty)
            {
                root.CheckState = CheckState.Checked;
            }
            else
            {
                ApplyFilters(root, filterSet);
            }
        }

        private static void ApplyFilters(Node node, FilterSet<ITestDescriptor> filterSet)
        {
            if (node is ITestDescriptor)
            {
                var match = EvaluateNode(node, filterSet);
                if (match) return;
            }
         
            foreach (var child in node.Nodes)
            {
                ApplyFilters(child, filterSet);
            }
        }

        private static bool EvaluateNode(Node node, FilterSet<ITestDescriptor> filterSet)
        {
            var result = filterSet.Evaluate((ITestDescriptor)node);

            if (result == FilterSetResult.Include)
            {
                node.CheckState = CheckState.Checked;
                return true;
            }
            
            node.CheckState = CheckState.Unchecked;
            return false;
        }

        public FilterSet<ITestDescriptor> GenerateFilterSetFromSelectedTests()
        {
            if (testTreeModel.Root == null || testTreeModel.Root.CheckState == CheckState.Checked)
                return FilterSet<ITestDescriptor>.Empty;

            var filter = testTreeModel.Root.CheckState == CheckState.Unchecked ? new NoneFilter<ITestDescriptor>()
                : CreateFilter(testTreeModel.Root.Nodes);

            return new FilterSet<ITestDescriptor>(filter);
        }

        private Filter<ITestDescriptor> CreateFilter(IEnumerable<Node> nodes)
        {
            var filters = new List<Filter<ITestDescriptor>>();
            foreach (var n in nodes)
            {
                var node = n as TestTreeNode;

                if (node == null)
                    continue;

                var filter = CreateFilterForNode(node);

                if (filter != null)
                {
                    filters.Add(filter);
                }
            }

            return filters.Count > 1 ? new OrFilter<ITestDescriptor>(filters)
                : filters[0];
        }

        private Filter<ITestDescriptor> CreateFilterForNode(TestTreeNode node)
        {
            Filter<ITestDescriptor> filter = null;
            switch (node.CheckState)
            {
                case CheckState.Checked:
                    {
                        filter = GenerateFilter(node);
                        break;
                    }
                case CheckState.Indeterminate:
                    {
                        filter = CreateFilter(node.Nodes);
                        break;
                    }
            }
            return filter;
        }

        private Filter<ITestDescriptor> GenerateFilter(TestTreeNode node)
        {
            var equalityFilter = new EqualityFilter<string>(node.Id);

            if (node is NamespaceNode)
            {
                return GetNamespaceFilter(node);
            }

            if (node is TestDataNode)
            {
                return new IdFilter<ITestDescriptor>(equalityFilter);
            }

            if (node is MetadataNode && node.Id != "None")
            {
                return new MetadataFilter<ITestDescriptor>(node.TestKind, equalityFilter);
            }

            return CreateFilter(node.Nodes);
        }

        private Filter<ITestDescriptor> GetNamespaceFilter(TestTreeNode namespaceNode)
        {
            var equalityFilter = new EqualityFilter<string>(namespaceNode.Id);
            var namespaceFilter = new NamespaceFilter<ITestDescriptor>(equalityFilter);
            
            if (optionsController.NamespaceHierarchy == NamespaceHierarchy.Flat)
                return namespaceFilter;

            var filters = new List<Filter<ITestDescriptor>> { namespaceFilter };

            foreach (var n in namespaceNode.Nodes)
            {
                var node = n as NamespaceNode;

                if (node == null)
                    continue;

                var filter = GetNamespaceFilter(node);

                if (filter != null)
                {
                    filters.Add(filter);
                }
            }

            return filters.Count > 1 ? new OrFilter<ITestDescriptor>(filters)
                : filters[0];
        }
    }
}
