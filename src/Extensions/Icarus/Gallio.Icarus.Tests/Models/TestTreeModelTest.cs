// Copyright 2005-2009 Gallio Project - http://www.gallio.org/
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

using Gallio.Icarus.Models;
using Gallio.Icarus.TreeBuilders;
using Gallio.Model;
using Gallio.Model.Filters;
using MbUnit.Framework;

namespace Gallio.Icarus.Tests.Models
{
    public class TestTreeModelTest
    {
        private TestTreeModel testTreeModel;

        [SetUp]
        public void Establish_context()
        {
            testTreeModel = new TestTreeModel(new ITreeBuilder[0]);
        }

        [Test]
        public void ApplyFilter_NullRoot_Test()
        {
            var filter = new FilterSet<ITestDescriptor>(new NoneFilter<ITestDescriptor>());
            testTreeModel.ApplyFilterSet(filter);
        }

        //[Test]
        //public void ApplyFilter_AnyFilter_Test()
        //{
        //    TestTreeModel testTreeModel = new TestTreeModel();
        //    TestTreeNode n = new TestTreeNode("root", "root", "root");
        //    testTreeModel.Root.Nodes.Add(n);
        //    Filter<ITestDescriptor> filter = new AnyFilter<ITestDescriptor>();
        //    //testTreeModel.ApplyFilter(filter);
        //    Assert.AreEqual(CheckState.Checked, n.CheckState);
        //}

        //[Test]
        //public void ApplyFilter_NoneFilter_Test()
        //{
        //    IProgressMonitor progressMonitor = MockProgressMonitor.GetMockProgressMonitor();
        //    TestTreeModel testTreeModel = new TestTreeModel();
        //    TestTreeNode n = new TestTreeNode("root", "root", "root");
        //    testTreeModel.Root.Nodes.Add(n);
        //    Filter<ITestDescriptor> filter = new NoneFilter<ITestDescriptor>();
        //    FilterSet<ITestDescriptor> filterSet = new FilterSet<ITestDescriptor>(filter);
        //    testTreeModel.ApplyFilterSet(filterSet);
        //    Assert.AreEqual(CheckState.Unchecked, n.CheckState);
        //}

        //[Test]
        //public void ApplyFilter_OrFilter_Test()
        //{
        //    IProgressMonitor progressMonitor = MockProgressMonitor.GetMockProgressMonitor();
        //    TestTreeModel testTreeModel = new TestTreeModel();
        //    TestTreeNode n = new TestTreeNode("root", "root", "root");
        //    testTreeModel.Root.Nodes.Add(n);
        //    TestTreeNode a = new TestTreeNode("a", "a", "a");
        //    TestTreeNode b = new TestTreeNode("b", "b", "b");
        //    TestTreeNode c = new TestTreeNode("c", "c", "c");
        //    n.Nodes.Add(a);
        //    n.Nodes.Add(b);
        //    n.Nodes.Add(c);
        //    Filter<ITestDescriptor> left = new IdFilter<ITestDescriptor>(new EqualityFilter<string>("a"));
        //    Filter<ITestDescriptor> right = new IdFilter<ITestDescriptor>(new EqualityFilter<string>("b"));
        //    Filter<ITestDescriptor> orFilter = new OrFilter<ITestDescriptor>(new[] { left, right});
        //    FilterSet<ITestDescriptor> filterSet = new FilterSet<ITestDescriptor>(orFilter);
        //    testTreeModel.ApplyFilterSet(filterSet);
        //    Assert.AreEqual(CheckState.Indeterminate, n.CheckState);
        //    Assert.AreEqual(CheckState.Checked, a.CheckState);
        //    Assert.AreEqual(CheckState.Checked, b.CheckState);
        //    Assert.AreEqual(CheckState.Unchecked, c.CheckState);
        //}

        //[Test]
        //[Row(TestStatus.Failed)]
        //[Row(TestStatus.Passed)]
        //[Row(TestStatus.Inconclusive)]
        //public void StatusFilter_Test(TestStatus testStatus)
        //{
        //    TestTreeModel testTreeModel = new TestTreeModel();

        //    TestTreeNode root = new TestTreeNode("root", "root", "root");
        //    testTreeModel.Root.Nodes.Add(root);

        //    TestTreeNode fixture = new TestTreeNode("fixture", "fixture", "fixture");
        //    root.Nodes.Add(fixture);

        //    TestTreeNode test1 = new TestTreeNode("test", "test", "test");
        //    fixture.Nodes.Add(test1);

        //    TestTreeNode statusTest = new TestTreeNode("status", "status", "status") { TestStatus = testStatus };
        //    fixture.Nodes.Add(statusTest);

        //    bool structureChangedFlag = false;
        //    testTreeModel.StructureChanged += delegate { structureChangedFlag = true; };

        //    testTreeModel.SetFilter(testStatus);

        //    TestTreeNode statusNode = null;
        //    foreach (var node in fixture.Nodes)
        //    {
        //        if (node.Text == testStatus.ToString())
        //            statusNode = (TestTreeNode)node;
        //    }

        //    Assert.IsNotNull(statusNode);
        //    Assert.AreEqual(1, statusNode.Nodes.Count);
        //    Assert.AreEqual(statusTest, statusNode.Nodes[0]);

        //    Assert.IsTrue(structureChangedFlag);
        //}

        [Test]
        public void FilterPassed_should_return_true_if_passed_filter_is_set()
        {
            testTreeModel.SetFilter(TestStatus.Passed);

            Assert.IsTrue(testTreeModel.FilterPassed);
        }

        [Test]
        public void FilterFailed_should_return_true_if_passed_filter_is_set()
        {
            testTreeModel.SetFilter(TestStatus.Failed);

            Assert.IsTrue(testTreeModel.FilterFailed);
        }

        [Test]
        public void FilterInconclusive_should_return_true_if_passed_filter_is_set()
        {
            testTreeModel.SetFilter(TestStatus.Inconclusive);

            Assert.IsTrue(testTreeModel.FilterInconclusive);
        }

        public class NestedClass
        {
            [Test]
            public void NestedTest()
            {
                Assert.IsTrue(true);
            }
        }
    }
}
