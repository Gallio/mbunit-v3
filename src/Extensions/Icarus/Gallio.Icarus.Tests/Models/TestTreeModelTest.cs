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

using System.Windows.Forms;
using Gallio.Icarus.Models;
using Gallio.Model;
using Gallio.Model.Filters;
using Gallio.Model.Serialization;
using Gallio.Reflection;
using Gallio.Runtime.ProgressMonitoring;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.Icarus.Tests.Models
{
    class TestTreeModelTest
    {
        [Test]
        public void ApplyFilter_NullRoot_Test()
        {
            IProgressMonitor progressMonitor = MockProgressMonitor();
            TestTreeModel testTreeModel = new TestTreeModel();
            Filter<ITest> filter = new NoneFilter<ITest>();
            testTreeModel.ApplyFilter(filter);
        }

        [Test]
        public void ApplyFilter_AnyFilter_Test()
        {
            IProgressMonitor progressMonitor = MockProgressMonitor();
            TestTreeModel testTreeModel = new TestTreeModel();
            TestTreeNode n = new TestTreeNode("root", "root", "root");
            testTreeModel.Nodes.Add(n);
            Filter<ITest> filter = new AnyFilter<ITest>();
            testTreeModel.ApplyFilter(filter);
            Assert.AreEqual(CheckState.Checked, n.CheckState);
        }

        [Test]
        public void ApplyFilter_NoneFilter_Test()
        {
            IProgressMonitor progressMonitor = MockProgressMonitor();
            TestTreeModel testTreeModel = new TestTreeModel();
            TestTreeNode n = new TestTreeNode("root", "root", "root");
            testTreeModel.Nodes.Add(n);
            Filter<ITest> filter = new NoneFilter<ITest>();
            testTreeModel.ApplyFilter(filter);
            Assert.AreEqual(CheckState.Unchecked, n.CheckState);
        }

        [Test]
        public void ApplyFilter_OrFilter_Test()
        {
            IProgressMonitor progressMonitor = MockProgressMonitor();
            TestTreeModel testTreeModel = new TestTreeModel();
            TestTreeNode n = new TestTreeNode("root", "root", "root");
            testTreeModel.Nodes.Add(n);
            TestTreeNode a = new TestTreeNode("a", "a", "a");
            TestTreeNode b = new TestTreeNode("b", "b", "b");
            TestTreeNode c = new TestTreeNode("c", "c", "c");
            n.Nodes.Add(a);
            n.Nodes.Add(b);
            n.Nodes.Add(c);
            Filter<ITest> left = new IdFilter<ITest>(new EqualityFilter<string>("a"));
            Filter<ITest> right = new IdFilter<ITest>(new EqualityFilter<string>("b"));
            Filter<ITest> orFilter = new OrFilter<ITest>(new[] { left, right});
            testTreeModel.ApplyFilter(orFilter);
            Assert.AreEqual(CheckState.Indeterminate, n.CheckState);
            Assert.AreEqual(CheckState.Checked, a.CheckState);
            Assert.AreEqual(CheckState.Checked, b.CheckState);
            Assert.AreEqual(CheckState.Unchecked, c.CheckState);
        }

        private static IProgressMonitor MockProgressMonitor()
        {
            var progressMonitor = MockRepository.GenerateStub<IProgressMonitor>();
            progressMonitor.Stub(x => x.BeginTask(Arg<string>.Is.Anything, 
                Arg<double>.Is.Anything)).Return(new ProgressMonitorTaskCookie(progressMonitor));
            progressMonitor.Stub(x => x.CreateSubProgressMonitor(
                Arg<double>.Is.Anything)).Return(progressMonitor).Repeat.Any();
            return progressMonitor;
        }

        [Test]
        public void BuildTestTree_Namespace_Test()
        {
            IProgressMonitor progressMonitor = MockProgressMonitor();
            
            TestTreeModel testTreeModel = new TestTreeModel();
            bool structureChangedFlag = false;
            testTreeModel.StructureChanged += delegate { structureChangedFlag = true; };
            
            TestData root = new TestData("root", "root", "root");
            
            TestData fixture = new TestData("fixture", "fixture", "fixture");
            fixture.Metadata.Add(MetadataKeys.TestKind, TestKinds.Fixture);
            fixture.CodeReference = new CodeReference(null, "namespaceName", null, null, null);
            root.Children.Add(fixture);
            
            TestData fixture2 = new TestData("fixture2", "fixture2", "fixture2");
            fixture2.Metadata.Add(MetadataKeys.TestKind, TestKinds.Fixture);
            fixture2.CodeReference = new CodeReference(null, "namespaceName", null, null, null);
            root.Children.Add(fixture2);
            
            TestData ignore = new TestData("ignore", "ignore", "ignore");
            fixture.Children.Add(ignore);
            
            TestData test = new TestData("test", "test", "test");
            test.Metadata.Add(MetadataKeys.TestKind, TestKinds.Test);
            fixture.Children.Add(test);
            
            TestModelData testModelData = new TestModelData(root);
            
            testTreeModel.BuildTestTree(testModelData, "Namespace");
            
            Assert.IsNotNull(testTreeModel.Root);
            Assert.AreEqual(1, testTreeModel.Root.Nodes.Count);
            
            TestTreeNode namespaceNode = (TestTreeNode)testTreeModel.Root.Nodes[0];
            Assert.AreEqual("namespaceName", namespaceNode.Name);
            Assert.AreEqual(2, namespaceNode.Nodes.Count);
            
            TestTreeNode fixtureNode = (TestTreeNode)namespaceNode.Nodes[0];
            Assert.AreEqual("fixture", fixtureNode.Name);
            Assert.AreEqual(1, fixtureNode.Nodes.Count);

            TestTreeNode testNode = (TestTreeNode)fixtureNode.Nodes[0];
            Assert.AreEqual("test", testNode.Name);
            Assert.AreEqual(0, testNode.Nodes.Count);

            Assert.IsTrue(structureChangedFlag);
        }
    }
}
