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

using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;
using Gallio.Icarus.Controls;
using Gallio.Icarus.Tests;
using Gallio.Model;
using Gallio.Model.Filters;
using Gallio.Model.Serialization;
using Gallio.Reflection;
using MbUnit.Framework;
using Gallio.Icarus.Controls.Interfaces;
using Gallio.Runner.Reports;
using Gallio.Icarus.Core.CustomEventArgs;
using Rhino.Mocks;

namespace Gallio.Icarus.AdapterModel.Tests
{
    [TestFixture]
    public class ProjectAdapterModelTest : MockTest
    {
        private ProjectAdapterModel projectAdapterModel;

        [SetUp]
        public void SetUp()
        {
            projectAdapterModel = new ProjectAdapterModel();
        }

        [Test]
        public void BuildNamespaceTestTree_Test()
        {
            projectAdapterModel.BuildTestTree(CreateTestModel(), "Namespaces");
            ITestTreeModel testTreeModel = projectAdapterModel.TreeModel;
            Assert.AreEqual(1, testTreeModel.Nodes.Count);

            // check Root node
            TestTreeNode rootNode = testTreeModel.Root;
            Assert.AreEqual("Root", rootNode.Name);
            Assert.AreEqual(1, rootNode.Nodes.Count);

            // check Framework node
            TestTreeNode frameworkNode = (TestTreeNode)rootNode.Nodes[0];
            Assert.AreEqual("Framework", frameworkNode.Name);
            Assert.AreEqual(1, frameworkNode.Nodes.Count);

            // check Assembly node
            TestTreeNode assemblyNode = (TestTreeNode)frameworkNode.Nodes[0];
            Assert.AreEqual("Assembly", assemblyNode.Name);
            Assert.AreEqual(1, assemblyNode.Nodes.Count);

            // check Namespace node
            TestTreeNode namespaceNode = (TestTreeNode)assemblyNode.Nodes[0];
            Assert.AreEqual("Namespace", namespaceNode.Name);
            Assert.AreEqual(2, namespaceNode.Nodes.Count);

            // check Fixture node
            TestTreeNode fixtureNode = (TestTreeNode)namespaceNode.Nodes[0];
            Assert.AreEqual("Fixture", fixtureNode.Name);
            Assert.AreEqual(2, fixtureNode.Nodes.Count);

            // check Test nodes
            TestTreeNode testNode;
            for (int i = 0; i < 2; i++)
            {
                testNode = (TestTreeNode)fixtureNode.Nodes[i];
                Assert.AreEqual("Test" + (i + 1), testNode.Name);
                Assert.AreEqual(0, testNode.Nodes.Count);
            }

            // check Fixture2 node
            fixtureNode = (TestTreeNode)namespaceNode.Nodes[1];
            Assert.AreEqual("Fixture2", fixtureNode.Name);
            Assert.AreEqual(2, fixtureNode.Nodes.Count);

            // check Test nodes
            testNode = (TestTreeNode)fixtureNode.Nodes[0];
            Assert.AreEqual("Test3", testNode.Name);
            Assert.AreEqual(0, testNode.Nodes.Count);
            Assert.IsTrue(testNode.SourceCodeAvailable);

            testNode = (TestTreeNode)fixtureNode.Nodes[1];
            Assert.AreEqual("Test4", testNode.Name);
            Assert.AreEqual(0, testNode.Nodes.Count);
        }

        [Test]
        public void BuildMetadataTestTree_Test()
        {
            string[] names = new string[] { "Authors", "Categories", "Importance", "TestsOn" };
            foreach (string name in names)
            {
                projectAdapterModel.BuildTestTree(CreateTestModel(), name);
                ITestTreeModel testTreeModel = projectAdapterModel.TreeModel;
                Assert.AreEqual(1, testTreeModel.Nodes.Count);

                // check Root node
                TestTreeNode node = testTreeModel.Root;
                Assert.AreEqual("Root", node.Name);
                Assert.AreEqual(2, node.Nodes.Count);
                Assert.IsTrue(node.IsChecked);

                // check None node
                node = (TestTreeNode)node.Nodes[0];
                Assert.AreEqual("None", node.Name);
                Assert.AreEqual(2, node.Nodes.Count);

                // check Fixture node
                node = (TestTreeNode)node.Nodes[0];
                Assert.AreEqual("Fixture", node.Name);
                Assert.AreEqual(1, node.Nodes.Count);

                // check Test node
                node = (TestTreeNode)node.Nodes[0];
                Assert.AreEqual("Test2", node.Name);
                Assert.AreEqual(0, node.Nodes.Count);

                // check Fixture node
                node = (TestTreeNode)node.Parent.Parent;
                node = (TestTreeNode)node.Nodes[1];
                Assert.AreEqual("Fixture2", node.Name);
                Assert.AreEqual(1, node.Nodes.Count);

                // check Test node
                node = (TestTreeNode)node.Nodes[0];
                Assert.AreEqual("Test3", node.Name);
                Assert.AreEqual(0, node.Nodes.Count);

                // check Author node
                node = (TestTreeNode)node.Parent.Parent.Parent.Nodes[1];
                Assert.AreEqual(name, node.Name);
                Assert.AreEqual(1, node.Nodes.Count);

                // check Test node
                node = (TestTreeNode)node.Nodes[0];
                Assert.AreEqual("Test1", node.Name);
                Assert.AreEqual(0, node.Nodes.Count);
            }
        }

        [Test]
        public void BuildAssemblyList_Test()
        {
            ListViewItem[] listViewItems = projectAdapterModel.BuildAssemblyList(new List<string>(new string[] { 
                Assembly.GetExecutingAssembly().Location }));
            Assert.AreEqual(1, listViewItems.Length);
            Assert.AreEqual(3, listViewItems[0].SubItems.Count);
        }

        private TestModelData CreateTestModel()
        {
            TestData rootTest = new TestData("Root", "Root", "Root");
            TestData framework = new TestData("Framework", "Framework", "Framework");
            framework.Metadata.SetValue(MetadataKeys.TestKind, "Framework");
            rootTest.Children.Add(framework);
            TestData assembly = new TestData("Assembly", "Assembly", "Assembly");
            assembly.Metadata.SetValue(MetadataKeys.TestKind, "Assembly");
            assembly.Metadata.SetValue(MetadataKeys.CodeBase, "CodeBase");
            framework.Children.Add(assembly);
            TestData fixture = new TestData("Fixture", "Fixture", "Fixture");
            fixture.CodeReference = CodeReference.CreateFromNamespace("Namespace");
            fixture.Metadata.SetValue(MetadataKeys.TestKind, "Fixture");
            assembly.Children.Add(fixture);
            TestData test1 = new TestData("Test1", "Test1", "Test1");
            test1.Metadata.SetValue(MetadataKeys.TestKind, "Test");
            test1.Metadata.SetValue(MetadataKeys.AuthorName, "Authors");
            test1.Metadata.SetValue(MetadataKeys.CategoryName, "Categories");
            test1.Metadata.SetValue(MetadataKeys.Importance, "Importance");
            test1.Metadata.SetValue(MetadataKeys.TestsOn, "TestsOn");
            test1.IsTestCase = true;
            fixture.Children.Add(test1);
            TestData test2 = new TestData("Test2", "Test2", "Test2");
            test2.Metadata.SetValue(MetadataKeys.TestKind, "Test");
            test2.IsTestCase = true;
            fixture.Children.Add(test2);
            TestData fixture2 = new TestData("Fixture2", "Fixture2", "Fixture2");
            fixture2.CodeReference = CodeReference.CreateFromNamespace("Namespace");
            fixture2.Metadata.SetValue(MetadataKeys.TestKind, "Fixture");
            assembly.Children.Add(fixture2);
            TestData test3 = new TestData("Test3", "Test3", "Test3");
            test3.Metadata.SetValue(MetadataKeys.TestKind, "Test");
            test3.IsTestCase = true;
            test3.CodeLocation = new CodeLocation("path", 1, 1);
            fixture2.Children.Add(test3);
            TestData test4 = new TestData("Test4", "Test4", "Test4");
            test4.Metadata.SetValue(MetadataKeys.TestKind, "Other");
            test4.IsTestCase = true;
            fixture2.Children.Add(test4);
            return new TestModelData(rootTest);
        }

        [Test]
        public void ApplyNoneFilter_Test()
        {
            projectAdapterModel.BuildTestTree(CreateTestModel(), "Namespaces");
            ITestTreeModel testTreeModel = projectAdapterModel.TreeModel;
            Filter<ITest> filter = new NoneFilter<ITest>();
            projectAdapterModel.ApplyFilter(filter);
            Assert.IsFalse(testTreeModel.Root.IsChecked);
        }

        [Test]
        public void ApplyFilter_Test()
        {
            projectAdapterModel.BuildTestTree(CreateTestModel(), "Namespaces");
            ITestTreeModel testTreeModel = projectAdapterModel.TreeModel;
            Filter<ITest> filter = new OrFilter<ITest>(new Filter<ITest>[] { new IdFilter<ITest>(new EqualityFilter<string>("Test2")), 
                new IdFilter<ITest>(new EqualityFilter<string>("Fixture2")) });
            projectAdapterModel.ApplyFilter(filter);
            // root node
            Assert.AreEqual(CheckState.Indeterminate, testTreeModel.Root.CheckState);
            // assembly node
            TestTreeNode node = (TestTreeNode)testTreeModel.Root.Nodes[0];
            Assert.AreEqual(CheckState.Indeterminate, node.CheckState);
            // namespace node
            node = (TestTreeNode)node.Nodes[0];
            Assert.AreEqual(CheckState.Indeterminate, node.CheckState);
            // fixture node
            node = (TestTreeNode)node.Nodes[0];
            Assert.AreEqual(CheckState.Indeterminate, node.CheckState);
            // fixture node
            node = (TestTreeNode)node.Nodes[0];
            Assert.AreEqual(CheckState.Indeterminate, node.CheckState);
            // fixture2 node
            node = (TestTreeNode)node.Parent.Nodes[1];
            Assert.AreEqual(CheckState.Checked, node.CheckState);
        }

        [Test]
        public void CreateFilter_Test_AnyFilter()
        {
            ITestTreeModel testTreeModel = projectAdapterModel.TreeModel;
            testTreeModel.Nodes.Add(new TestTreeNode("test", "test", "test"));
            Filter<ITest> filter = projectAdapterModel.CreateFilter();
            Assert.IsTrue(filter is AnyFilter<ITest>);
        }

        [Test]
        public void CreateFilter_Test_NoneFilter()
        {
            ITestTreeModel testTreeModel = projectAdapterModel.TreeModel;
            TestTreeNode test = new TestTreeNode("test", "test", "test");
            test.CheckState = CheckState.Unchecked;
            testTreeModel.Nodes.Add(test);
            Filter<ITest> filter = projectAdapterModel.CreateFilter();
            Assert.IsTrue(filter is NoneFilter<ITest>);
        }

        [Test]
        public void CreateFilter_Test()
        {
            ITestTreeModel testTreeModel = projectAdapterModel.TreeModel;
            TestTreeNode root = new TestTreeNode("root", "root", "root");
            root.CheckState = CheckState.Indeterminate;
            TestTreeNode ns = new TestTreeNode("ns", "ns", TestKinds.Namespace);
            root.Nodes.Add(ns);
            TestTreeNode child = new TestTreeNode("child", "child", "child");
            ns.Nodes.Add(child);
            testTreeModel.Nodes.Add(root);
            Filter<ITest> filter = projectAdapterModel.CreateFilter();
            Assert.AreEqual(1, ((OrFilter<ITest>)filter).Filters.Length);
            filter = ((OrFilter<ITest>)filter).Filters[0];
            Assert.AreEqual(1, ((OrFilter<ITest>)filter).Filters.Length);
            filter = ((OrFilter<ITest>)filter).Filters[0];
            Assert.AreEqual(1, ((OrFilter<ITest>)filter).Filters.Length);
            filter = ((OrFilter<ITest>)filter).Filters[0];
            Assert.AreEqual("child", ((EqualityFilter<string>)((IdFilter<ITest>)filter).ValueFilter).Comparand);
        }

        [Test]
        public void ResetTestStatus_Test()
        {
            ITestTreeModel testTreeModel = mocks.CreateMock<ITestTreeModel>();
            testTreeModel.ResetTestStatus();
            mocks.ReplayAll();
            projectAdapterModel.TreeModel = testTreeModel;
            projectAdapterModel.ResetTestStatus();
        }

        [Test]
        public void Update_Test()
        {
            TestData testData = new TestData("test", "test", "test");
            TestStepRun testStepRun = new TestStepRun(new TestStepData("test", "test", "test", "test"));
            testStepRun.Step.IsTestCase = true;
            ITestTreeModel testTreeModel = mocks.CreateMock<ITestTreeModel>();
            testTreeModel.UpdateTestStatus(testData, testStepRun);
            LastCall.IgnoreArguments();
            mocks.ReplayAll();
            projectAdapterModel.TreeModel = testTreeModel;
            projectAdapterModel.Update(testData, testStepRun);
        }
    }
}