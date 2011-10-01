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

using Gallio.Icarus.Models;
using Gallio.Icarus.Tests.Utilities;
using Gallio.Icarus.TreeBuilders;
using Gallio.Model.Schema;
using MbUnit.Framework;
using NHamcrest;
using NHamcrest.Core;

namespace Gallio.Icarus.Tests.TreeBuilders
{
    public class MetadataTreeBuilderTest
    {
        private TestModelData testModelData;
        private MetadataTreeBuilder metadataTreeBuilder;
        private const string Metadata = "Metadata";

        [SetUp]
        public void Establish_context()
        {
            metadataTreeBuilder = new MetadataTreeBuilder();

            testModelData = new TestModelData();
            var assembly = CreateTestData("assembly");
            testModelData.RootTest.Children.Add(assembly);
            var fixture1 = CreateTestData("fixture1");
            var fixture2 = CreateTestData("fixture2");
            assembly.Children.AddRange(new[] { fixture1, fixture2 });
            var test1 = CreateTestData("test1");
            var test2 = CreateTestData("test2");
            fixture1.Children.AddRange(new[] { test1, test2 });
            var test3 = CreateTestData("test3");
            test3.Metadata.Add(Metadata, "metadata");
            test3.Metadata.Add(Metadata, "metadata2");
            fixture2.Children.Add(test3);
            var fixture3 = CreateTestData("fixture3");
            fixture3.Metadata.Add(Metadata, "metadata");
            assembly.Children.Add(fixture3);
            var test4 = CreateTestData("test4");
            test4.Metadata.Add(Metadata, "metadata");
            fixture3.Children.Add(test4);
        }

        private static TestData CreateTestData(string name)
        {
            return new TestData(name, name, name);
        }

        [Test, Author("Graham")]
        public void Test1_should_be_under_Fixture1()
        {
            var node = metadataTreeBuilder.BuildTree(MockProgressMonitor.Instance, testModelData,
                new TreeBuilderOptions { TreeViewCategory = Metadata });

            var test1 = node.Find("test1", true)[0];
            var parent = (TestTreeNode)test1.Parent;
            Assert.AreEqual("fixture1", parent.Id);
        }

        [Test]
        public void Test2_should_be_under_Fixture1()
        {
            var node = metadataTreeBuilder.BuildTree(MockProgressMonitor.Instance, testModelData,
                new TreeBuilderOptions { TreeViewCategory = Metadata });

            var test2 = node.Find("test2", true)[0];
            var parent = (TestTreeNode)test2.Parent;
            Assert.AreEqual("fixture1", parent.Id);
        }

        [Test]
        public void Test3_should_be_under_two_metadata_nodes()
        {
            var node = metadataTreeBuilder.BuildTree(MockProgressMonitor.Instance, testModelData, 
                new TreeBuilderOptions { TreeViewCategory = Metadata });

            var test3s = node.Find("test3", true);

            Assert.That(test3s.Count, Is.EqualTo(2));
            Assert.That(test3s, new IsCollectionContaining<TestTreeNode>(new CustomMatcher<TestTreeNode>("metadata node", ttn => ttn.Parent.Text == "metadata")));
            Assert.That(test3s, new IsCollectionContaining<TestTreeNode>(new CustomMatcher<TestTreeNode>("metadata node", ttn => ttn.Parent.Text == "metadata2")));
        }

        [Test]
        public void Fixture3_should_be_under_the_metadata_node()
        {
            var node = metadataTreeBuilder.BuildTree(MockProgressMonitor.Instance, testModelData,
                new TreeBuilderOptions { TreeViewCategory = Metadata });

            var fixture3 = node.Find("fixture3", true)[0];
            var metadataNode = (TestTreeNode)fixture3.Parent;
            Assert.AreEqual(Metadata, metadataNode.TestKind);
        }

        [Test]
        public void Test4_should_be_under_Fixture3()
        {
            var node = metadataTreeBuilder.BuildTree(MockProgressMonitor.Instance, testModelData,
                new TreeBuilderOptions { TreeViewCategory = Metadata });

            var test4 = node.Find("test4", true)[0];
            var metadataNode = (TestTreeNode)test4.Parent;
            Assert.AreEqual(Metadata, metadataNode.TestKind);
        }
    }
}
