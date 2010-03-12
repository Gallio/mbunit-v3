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

using System.Windows.Forms;
using Aga.Controls.Tree;
using Gallio.Icarus.Models;
using Gallio.Icarus.Models.TestTreeNodes;
using Gallio.Icarus.Services;
using Gallio.Model;
using Gallio.Model.Filters;
using Gallio.Model.Schema;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.Icarus.Tests.Services
{
    [TestsOn(typeof(FilterService))]
    public class FilterServiceTest
    {
        private ITestTreeModel testTreeModel;
        private FilterService filterService;

        [SetUp]
        public void SetUp()
        {
            testTreeModel = MockRepository.GenerateStub<ITestTreeModel>();
            filterService = new FilterService(testTreeModel);
        }

        [Test]
        public void Apply_filter_set_should_do_nothing_if_tree_model_root_is_null()
        {
            filterService.ApplyFilterSet(new FilterSet<ITestDescriptor>(new AnyFilter<ITestDescriptor>()));
        }

        [Test]
        public void If_applied_filter_set_is_empty_then_the_root_should_be_checked()
        {
            var testTreeNode = new TestTreeNode("root", "root")
            {
                CheckState = CheckState.Unchecked
            };
            testTreeModel.Stub(ttm => ttm.Root).Return(testTreeNode);

            filterService.ApplyFilterSet(FilterSet<ITestDescriptor>.Empty);

            Assert.AreEqual(CheckState.Checked, testTreeNode.CheckState);
        }

        [Test]
        public void If_applied_filter_set_is_any_filter_then_the_root_should_be_checked()
        {
            var testTreeNode = new TestDataNode(new TestData("root", "root", "root"))
            {
                CheckState = CheckState.Unchecked
            };
            testTreeModel.Stub(ttm => ttm.Root).Return(testTreeNode);           
            var filterSet = new FilterSet<ITestDescriptor>(new AnyFilter<ITestDescriptor>());

            filterService.ApplyFilterSet(filterSet);

            Assert.AreEqual(CheckState.Checked, testTreeNode.CheckState);
        }

        [Test]
        public void If_applied_filter_set_is_none_filter_then_the_root_should_be_unchecked()
        {
            var testTreeNode = new TestTreeNode("root", "root");
            testTreeModel.Stub(ttm => ttm.Root).Return(testTreeNode);
            var filterSet = new FilterSet<ITestDescriptor>(new NoneFilter<ITestDescriptor>());

            filterService.ApplyFilterSet(filterSet);

            Assert.AreEqual(CheckState.Unchecked, testTreeNode.CheckState);
        }

        [Test]
        public void Filter_sets_should_be_applied_appropriately()
        {
            var root = new TestDataNode(new TestData("root", "root", "root"));
            var test1 = new TestDataNode(new TestData("test1", "test1", "test1"));
            var test2 = new TestDataNode(new TestData("test2", "test2", "test2"));
            root.Nodes.Add(test1);
            root.Nodes.Add(test2);
            testTreeModel.Stub(ttm => ttm.Root).Return(root);
            var filterSet = new FilterSet<ITestDescriptor>(new OrFilter<ITestDescriptor>(new[]
            {
                new IdFilter<ITestDescriptor>(new EqualityFilter<string>("test2"))
            }));

            filterService.ApplyFilterSet(filterSet);

            Assert.AreEqual(CheckState.Indeterminate, root.CheckState);
            Assert.AreEqual(CheckState.Unchecked, test1.CheckState);
            Assert.AreEqual(CheckState.Checked, test2.CheckState);
        }

        [Test]
        public void Generated_filter_should_be_empty_if_tree_model_root_is_null()
        {
            var filterSet = filterService.GenerateFilterSetFromSelectedTests();

            Assert.IsTrue(filterSet.IsEmpty);
        }

        [Test]
        public void Generated_filter_should_be_empty_if_tree_model_root_is_checked()
        {
            var testTreeNode = new TestTreeNode("root", "root")
            {
                CheckState = CheckState.Checked
            };
            testTreeModel.Stub(ttm => ttm.Root).Return(testTreeNode);

            var filterSet = filterService.GenerateFilterSetFromSelectedTests();

            Assert.IsTrue(filterSet.IsEmpty);
        }

        [Test]
        public void Generated_filter_should_be_none_if_tree_model_root_is_unchecked()
        {
            var root = new TestTreeNode("root", "root")
            {
                CheckState = CheckState.Unchecked
            };
            testTreeModel.Stub(ttm => ttm.Root).Return(root);

            var filterSet = filterService.GenerateFilterSetFromSelectedTests();

            Assert.IsInstanceOfType(typeof(NoneFilter<ITestDescriptor>), 
                filterSet.Rules[0].Filter);
        }

        [Test]
        public void Generated_filter_should_be_correct_if_root_is_indeterminate_and_namespace_is_checked()
        {
            var root = new TestTreeNode("root", "root")
            {
                CheckState = CheckState.Indeterminate
            };
            var child = new TestTreeNode("child", "child")
            {
                CheckState = CheckState.Checked
            };
            root.Nodes.Add(child);
            const string @namespace = "Gallio.Icarus.Tests.Services";
            child.Nodes.Add(new NamespaceNode(@namespace)
            {
                CheckState = CheckState.Checked
            });
            testTreeModel.Stub(ttm => ttm.Root).Return(root);

            var filterSet = filterService.GenerateFilterSetFromSelectedTests();

            var filter = (PropertyFilter<ITestDescriptor>)filterSet.Rules[0].Filter;
            Assert.IsInstanceOfType(typeof(NamespaceFilter<ITestDescriptor>),
                filter);
            Assert.AreEqual(true, filter.ValueFilter.IsMatch(@namespace));
        }

        [Test]
        public void Generated_filter_should_be_correct_if_root_is_indeterminate_and_test_data_is_checked()
        {
            var root = new TestTreeNode("root", "root")
            {
                CheckState = CheckState.Indeterminate
            };
            var child = new TestTreeNode("child", "child")
            {
                CheckState = CheckState.Indeterminate
            };
            root.Nodes.Add(child);
            const string id = "id";
            child.Nodes.Add(new TestDataNode(new TestData(id, "name", "fullName"))
            {
                CheckState = CheckState.Checked
            });
            child.Nodes.Add(new Node());
            testTreeModel.Stub(ttm => ttm.Root).Return(root);

            var filterSet = filterService.GenerateFilterSetFromSelectedTests();

            var filter = (PropertyFilter<ITestDescriptor>)filterSet.Rules[0].Filter;
            Assert.IsInstanceOfType(typeof(IdFilter<ITestDescriptor>),
                filter);
            Assert.AreEqual(true, filter.ValueFilter.IsMatch(id));
        }

        [Test]
        public void Generated_filter_should_be_correct_if_root_is_indeterminate_and_metadata_is_checked()
        {
            var root = new TestTreeNode("root", "root")
            {
                CheckState = CheckState.Indeterminate
            };
            const string category = "blahblah";
            const string metadataType = MetadataKeys.Category;
            root.Nodes.Add(new MetadataNode(category, metadataType)
            {
                CheckState = CheckState.Checked
            });
            testTreeModel.Stub(ttm => ttm.Root).Return(root);

            var filterSet = filterService.GenerateFilterSetFromSelectedTests();

            var filter = (PropertyFilter<ITestDescriptor>)filterSet.Rules[0].Filter;
            Assert.IsInstanceOfType(typeof(MetadataFilter<ITestDescriptor>),
                filter);
            Assert.AreEqual(metadataType, filter.Key);
            Assert.AreEqual(true, filter.ValueFilter.IsMatch(category));
        }
    }
}
