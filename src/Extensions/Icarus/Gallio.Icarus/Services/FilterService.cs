using System.Collections.Generic;
using System.Windows.Forms;
using Aga.Controls.Tree;
using Gallio.Icarus.Models;
using Gallio.Icarus.Models.TestTreeNodes;
using Gallio.Model.Filters;

namespace Gallio.Icarus.Services
{
    public class FilterService : IFilterService
    {
        private readonly ITestTreeModel testTreeModel;

        public FilterService(ITestTreeModel testTreeModel)
        {
            this.testTreeModel = testTreeModel;
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

        private static Filter<ITestDescriptor> CreateFilter(IEnumerable<Node> nodes)
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

        private static Filter<ITestDescriptor> CreateFilterForNode(TestTreeNode node)
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

        private static Filter<ITestDescriptor> GenerateFilter(TestTreeNode node)
        {
            var equalityFilter = new EqualityFilter<string>(node.Id);

            if (node is NamespaceNode)
            {
                return new NamespaceFilter<ITestDescriptor>(equalityFilter);
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
    }
}
