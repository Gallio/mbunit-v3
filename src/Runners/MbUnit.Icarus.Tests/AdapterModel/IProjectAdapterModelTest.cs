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

using MbUnit.Framework;
using MbUnit.Icarus.AdapterModel;
using MbUnit.Icarus.Interfaces;
using MbUnit.Model;
using MbUnit.Model.Serialization;

namespace MbUnit.Icarus.Tests
{
    [TestFixture, Category("IProjectAdapterModel")]
    public class IProjectAdapterModelTest
    {
        [Test]
        public void BuildNamespaceTestTree_Test()
        {
            IProjectAdapterModel projectAdapterModel = new ProjectAdapterModel();
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
            Assert.AreEqual(1, namespaceNode.Nodes.Count);

            // check Fixture node
            TreeNode fixtureNode = namespaceNode.Nodes[0];
            Assert.AreEqual("Fixture", fixtureNode.Name);
            Assert.AreEqual(3, fixtureNode.ImageIndex);
            Assert.AreEqual(3, fixtureNode.Nodes.Count);
                        
            // check Test nodes
            for (int i = 0; i < 3; i++)
            {
                TreeNode testNode = fixtureNode.Nodes[i];
                Assert.AreEqual("Test" + (i + 1), testNode.Name);
                Assert.AreEqual(4, testNode.ImageIndex);
                Assert.AreEqual(0, testNode.Nodes.Count);
            }
        }

        [Test]
        public void CountTests_Test()
        {
            IProjectAdapterModel projectAdapterModel = new ProjectAdapterModel();
            Assert.AreEqual(3, projectAdapterModel.CountTests(CreateTestModel()));
        }

        [Test]
        public void BuildAssemblyList_Test()
        {
            IProjectAdapterModel projectAdapterModel = new ProjectAdapterModel();
            ListViewItem[] listViewItems = projectAdapterModel.BuildAssemblyList(new List<string>(new string[] { 
                Assembly.GetExecutingAssembly().Location }));
            Assert.AreEqual(1, listViewItems.Length);
            Assert.AreEqual(3, listViewItems[0].SubItems.Count);
        }

        private TestModel CreateTestModel()
        {
            TestData rootTest = new TestData("Root", "Root");
            TestData framework = new TestData("Framework", "Framework");
            framework.Metadata.SetValue(MetadataKeys.ComponentKind, "Framework");
            rootTest.Children.Add(framework);
            TestData assembly = new TestData("Assembly", "Assembly");
            assembly.Metadata.SetValue(MetadataKeys.ComponentKind, "Assembly");
            framework.Children.Add(assembly);
            TestData fixture = new TestData("Fixture", "Fixture");
            fixture.CodeReference.NamespaceName = "Namespace";
            fixture.Metadata.SetValue(MetadataKeys.ComponentKind, "Fixture");
            assembly.Children.Add(fixture);
            TestData test1 = new TestData("Test1", "Test1");
            test1.Metadata.SetValue(MetadataKeys.ComponentKind, "Test");
            test1.IsTestCase = true;
            fixture.Children.Add(test1);
            TestData test2 = new TestData("Test2", "Test2");
            test2.Metadata.SetValue(MetadataKeys.ComponentKind, "Test");
            test2.IsTestCase = true;
            fixture.Children.Add(test2);
            TestData test3 = new TestData("Test3", "Test3");
            test3.Metadata.SetValue(MetadataKeys.ComponentKind, "Test");
            test3.IsTestCase = true;
            fixture.Children.Add(test3);
            return new TestModel(rootTest);
        }
    }
}