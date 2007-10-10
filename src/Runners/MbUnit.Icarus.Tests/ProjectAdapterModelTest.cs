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
using MbUnit.Model;
using MbUnit.Model.Serialization;
using MbUnit.Icarus.AdapterModel;

namespace MbUnit.Icarus.Tests
{
    [TestFixture]
    public class ProjectAdapterModelTest
    {
        [Test]
        public void BuildTestTree_Test()
        {
            TestData rootTest = new TestData("Root", "Root");
            TestData framework = new TestData("Framework", "Framework");
            framework.Metadata.SetValue(MetadataKeys.ComponentKind, "Framework");
            rootTest.Children.Add(framework);
            TestData assembly = new TestData("Assembly", "Assembly");
            assembly.Metadata.SetValue(MetadataKeys.ComponentKind, "Assembly");
            framework.Children.Add(assembly);
            TestData fixture = new TestData("Fixture", "Fixture");
            fixture.Metadata.SetValue(MetadataKeys.ComponentKind, "Fixture");
            assembly.Children.Add(fixture);
            TestData test = new TestData("Test", "Test");
            test.Metadata.SetValue(MetadataKeys.ComponentKind, "Test");
            fixture.Children.Add(test);
            TestModel testModel = new TestModel(rootTest);

            ProjectAdapterModel projectAdapterModel = new ProjectAdapterModel();
            TreeNode[] treeNodes = projectAdapterModel.BuildTestTree(testModel);
            Assert.AreEqual(1, treeNodes.Length);
            TreeNode rootNode = treeNodes[0];
            Assert.AreEqual("Root", rootNode.Name);
            Assert.AreEqual(0, rootNode.ImageIndex);
            Assert.AreEqual(1, rootNode.Nodes.Count);
            TreeNode frameworkNode = rootNode.Nodes[0];
            Assert.AreEqual("Framework", frameworkNode.Name);
            Assert.AreEqual(1, frameworkNode.ImageIndex);
            Assert.AreEqual(1, frameworkNode.Nodes.Count);
            TreeNode assemblyNode = frameworkNode.Nodes[0];
            Assert.AreEqual("Assembly", assemblyNode.Name);
            Assert.AreEqual(2, assemblyNode.ImageIndex);
            Assert.AreEqual(1, assemblyNode.Nodes.Count);
            TreeNode fixtureNode = assemblyNode.Nodes[0];
            Assert.AreEqual("Fixture", fixtureNode.Name);
            Assert.AreEqual(3, fixtureNode.ImageIndex);
            Assert.AreEqual(1, fixtureNode.Nodes.Count);
            TreeNode testNode = fixtureNode.Nodes[0];
            Assert.AreEqual("Test", testNode.Name);
            Assert.AreEqual(4, testNode.ImageIndex);
            Assert.AreEqual(0, testNode.Nodes.Count);
        }

        [Test]
        public void BuildAssemblyList_Test()
        {
            ProjectAdapterModel projectAdapterModel = new ProjectAdapterModel();
            ListViewItem[] listViewItems = projectAdapterModel.BuildAssemblyList(new List<string>(new string[] { 
                Assembly.GetExecutingAssembly().Location }));
            Assert.AreEqual(1, listViewItems.Length);
            Assert.AreEqual(3, listViewItems[0].SubItems.Count);
        }
    }
}