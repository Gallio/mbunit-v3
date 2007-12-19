// Copyright 2007 MbUnit Project - http://www.mbunit.com/
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
using Gallio.Icarus.AdapterModel;
using Gallio.Icarus.Controls;
using Gallio.Icarus.Controls.Enums;
using Gallio.Icarus.Interfaces;
using Gallio.Model;
using Gallio.Model.Filters;
using Gallio.Model.Serialization;
using MbUnit.Framework;

namespace Gallio.Icarus.Tests
{
    [TestFixture, Category("ProjectAdapterModel"), Category("Category2")]
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
            TreeNode[] treeNodes = projectAdapterModel.BuildTestTree(CreateTestModel(), "Namespaces");
            Assert.AreEqual(1, treeNodes.Length);

            // check Root node
            TreeNode rootNode = treeNodes[0];
            Assert.AreEqual("Root", rootNode.Name);
            Assert.AreEqual(0, rootNode.ImageIndex);
            Assert.AreEqual(1, rootNode.Nodes.Count);

            // check Framework node
            TreeNode frameworkNode = rootNode.Nodes[0];
            Assert.AreEqual("Framework", frameworkNode.Name);
            Assert.AreEqual(0, frameworkNode.ImageIndex);
            Assert.AreEqual(1, frameworkNode.Nodes.Count);

            // check Assembly node
            TreeNode assemblyNode = frameworkNode.Nodes[0];
            Assert.AreEqual("Assembly", assemblyNode.Name);
            Assert.AreEqual(1, assemblyNode.ImageIndex);
            Assert.AreEqual(1, assemblyNode.Nodes.Count);

            // check Namespace node
            TreeNode namespaceNode = assemblyNode.Nodes[0];
            Assert.AreEqual("Namespace", namespaceNode.Name);
            Assert.AreEqual(2, namespaceNode.ImageIndex);
            Assert.AreEqual(2, namespaceNode.Nodes.Count);

            // check Fixture node
            TreeNode fixtureNode = namespaceNode.Nodes[0];
            Assert.AreEqual("Fixture", fixtureNode.Name);
            Assert.AreEqual(3, fixtureNode.ImageIndex);
            Assert.AreEqual(2, fixtureNode.Nodes.Count);

            // check Test nodes
            TreeNode testNode;
            for (int i = 0; i < 2; i++)
            {
                testNode = fixtureNode.Nodes[i];
                Assert.AreEqual("Test" + (i + 1), testNode.Name);
                Assert.AreEqual(4, testNode.ImageIndex);
                Assert.AreEqual(0, testNode.Nodes.Count);
            }

            // check Fixture2 node
            fixtureNode = namespaceNode.Nodes[1];
            Assert.AreEqual("Fixture2", fixtureNode.Name);
            Assert.AreEqual(3, fixtureNode.ImageIndex);
            Assert.AreEqual(2, fixtureNode.Nodes.Count);

            // check Test nodes
            testNode = fixtureNode.Nodes[0];
            Assert.AreEqual("Test3", testNode.Name);
            Assert.AreEqual(4, testNode.ImageIndex);
            Assert.AreEqual(0, testNode.Nodes.Count);
            testNode = fixtureNode.Nodes[1];
            Assert.AreEqual("Test4", testNode.Name);
            Assert.AreEqual(0, testNode.ImageIndex);
            Assert.AreEqual(0, testNode.Nodes.Count);
        }

        //[RowTest, Row("Authors"), Row("Categories"), Row("Importance"), Row("TestsOn")]
        //public void BuildMetadataTestTree_Test(string name)
        [Test]
        public void BuildMetadataTestTree_Test()
        {
            string[] names = new string[] { "Authors", "Categories", "Importance", "TestsOn" };
            foreach (string name in names)
            {
                TreeNode[] treeNodes = projectAdapterModel.BuildTestTree(CreateTestModel(), name);
                Assert.AreEqual(1, treeNodes.Length);

                // check Root node
                TreeNode node = treeNodes[0];
                Assert.AreEqual("Root", node.Name);
                Assert.AreEqual(0, node.ImageIndex);
                Assert.AreEqual(2, node.Nodes.Count);

                // check None node
                node = node.Nodes[0];
                Assert.AreEqual("None", node.Name);
                Assert.AreEqual(0, node.ImageIndex);
                Assert.AreEqual(2, node.Nodes.Count);

                // check Fixture node
                node = node.Nodes[0];
                Assert.AreEqual("Fixture", node.Name);
                Assert.AreEqual(3, node.ImageIndex);
                Assert.AreEqual(1, node.Nodes.Count);

                // check Test node
                node = node.Nodes[0];
                Assert.AreEqual("Test2", node.Name);
                Assert.AreEqual(4, node.ImageIndex);
                Assert.AreEqual(0, node.Nodes.Count);

                // check Fixture node
                node = node.Parent.Parent;
                node = node.Nodes[1];
                Assert.AreEqual("Fixture2", node.Name);
                Assert.AreEqual(3, node.ImageIndex);
                Assert.AreEqual(1, node.Nodes.Count);

                // check Test node
                node = node.Nodes[0];
                Assert.AreEqual("Test3", node.Name);
                Assert.AreEqual(4, node.ImageIndex);
                Assert.AreEqual(0, node.Nodes.Count);

                // check Author node
                node = node.Parent.Parent.Parent.Nodes[1];
                Assert.AreEqual(name, node.Name);
                Assert.AreEqual(0, node.ImageIndex);
                Assert.AreEqual(1, node.Nodes.Count);

                // check Test node
                node = node.Nodes[0];
                Assert.AreEqual("Test1", node.Name);
                Assert.AreEqual(4, node.ImageIndex);
                Assert.AreEqual(0, node.Nodes.Count);
            }
        }

        [Test]
        public void CountTests_Test()
        {
            Assert.AreEqual(4, projectAdapterModel.CountTests(CreateTestModel()));
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
            TestData rootTest = new TestData("Root", "Root");
            TestData framework = new TestData("Framework", "Framework");
            framework.Metadata.SetValue(MetadataKeys.TestKind, "Framework");
            rootTest.Children.Add(framework);
            TestData assembly = new TestData("Assembly", "Assembly");
            assembly.Metadata.SetValue(MetadataKeys.TestKind, "Assembly");
            assembly.Metadata.SetValue(MetadataKeys.CodeBase, "CodeBase");
            framework.Children.Add(assembly);
            TestData fixture = new TestData("Fixture", "Fixture");
            fixture.CodeReference.NamespaceName = "Namespace";
            fixture.Metadata.SetValue(MetadataKeys.TestKind, "Fixture");
            assembly.Children.Add(fixture);
            TestData test1 = new TestData("Test1", "Test1");
            test1.Metadata.SetValue(MetadataKeys.TestKind, "Test");
            test1.Metadata.SetValue(MetadataKeys.AuthorName, "Authors");
            test1.Metadata.SetValue(MetadataKeys.CategoryName, "Categories");
            test1.Metadata.SetValue(MetadataKeys.Importance, "Importance");
            test1.Metadata.SetValue(MetadataKeys.TestsOn, "TestsOn");
            test1.IsTestCase = true;
            fixture.Children.Add(test1);
            TestData test2 = new TestData("Test2", "Test2");
            test2.Metadata.SetValue(MetadataKeys.TestKind, "Test");
            test2.IsTestCase = true;
            fixture.Children.Add(test2);
            TestData fixture2 = new TestData("Fixture2", "Fixture2");
            fixture2.CodeReference.NamespaceName = "Namespace";
            fixture2.Metadata.SetValue(MetadataKeys.TestKind, "Fixture");
            assembly.Children.Add(fixture2);
            TestData test3 = new TestData("Test3", "Test3");
            test3.Metadata.SetValue(MetadataKeys.TestKind, "Test");
            test3.IsTestCase = true;
            fixture2.Children.Add(test3);
            TestData test4 = new TestData("Test4", "Test4");
            test4.Metadata.SetValue(MetadataKeys.TestKind, "Other");
            test4.IsTestCase = true;
            fixture2.Children.Add(test4);
            return new TestModelData(rootTest);
        }

        [Test]
        public void GetEmptyFilter_Test()
        {
            TreeView treeView = new TreeView();
            Assert.IsNull(projectAdapterModel.GetFilter(treeView.Nodes));
        }

        [Test]
        public void GetFilter_Test()
        {
            TreeView treeView = new TreeView();
            TestTreeNode root = new TestTreeNode("Root", "Root", 0);
            root.CheckState = CheckBoxStates.Indeterminate;
            TestTreeNode child = new TestTreeNode("Child", "Child", 2);
            child.CheckState = CheckBoxStates.Checked;
            root.Nodes.Add(child);
            TestTreeNode child2 = new TestTreeNode("Child2", "Child2", 3);
            child2.CheckState = CheckBoxStates.Checked;
            child.Nodes.Add(child2);
            treeView.Nodes.Add(root);
            Assert.IsNotNull(projectAdapterModel.GetFilter(treeView.Nodes));
        }
    }
}