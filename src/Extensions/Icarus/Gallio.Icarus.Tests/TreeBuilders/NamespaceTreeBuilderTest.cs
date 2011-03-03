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

using Gallio.Icarus.Tests.Utilities;
using Gallio.Icarus.TreeBuilders;
using Gallio.Model.Schema;
using MbUnit.Framework;
using Rhino.Mocks;
using Test = Gallio.Model.Tree.Test;
using Gallio.Common.Reflection;

namespace Gallio.Icarus.Tests.TreeBuilders
{
    public class NamespaceTreeBuilderTest
    {
        private TestModelData testModelData;
        private NamespaceTreeBuilder namespaceTreeBuilder;
        private MockRepository mocks;

        [SetUp]
        public void Establish_context()
        {
            mocks = new MockRepository();
            namespaceTreeBuilder = new NamespaceTreeBuilder();

            testModelData = new TestModelData();
            var assembly = new TestData("assembly", "assembly", "assembly");
            testModelData.RootTest.Children.Add(assembly);

            var nsRootCodeElement = mocks.StrictMock<ICodeElementInfo>();
            var nsRootCodeReference = new CodeReference("assembly", null, null, null, null);
            var nsRoot = new Test("nsRoot", nsRootCodeElement);

            using (mocks.Record())
            {
                Expect.Call(nsRootCodeElement.CodeReference).Return(nsRootCodeReference);
                Expect.Call(nsRootCodeElement.GetCodeLocation()).Return(new CodeLocation("about:blank", 1, 1));
            }
            nsRoot.Kind = Gallio.Model.TestKinds.Namespace;
            nsRoot.IsTestCase = false;

            var nsRootTest = new TestData(nsRoot, true);
            assembly.Children.Add(nsRootTest);

            var nsLevel1CodeElement = mocks.StrictMock<ICodeElementInfo>();
            var nsLevel1CodeReference = new CodeReference("assembly", null, null, null, null);
            var nsLevel1 = new Test("nsLevel1", nsLevel1CodeElement);

            using (mocks.Record())
            {
                Expect.Call(nsLevel1CodeElement.CodeReference).Return(nsLevel1CodeReference);
                Expect.Call(nsLevel1CodeElement.GetCodeLocation()).Return(new CodeLocation("about:blank", 1, 1));
            }
            nsLevel1.Kind = Gallio.Model.TestKinds.Namespace;
            nsLevel1.IsTestCase = false;

            var nsLevel1Test = new TestData(nsLevel1, true);
            nsRootTest.Children.Add(nsLevel1Test);

            var nsLevel2CodeElement = mocks.StrictMock<ICodeElementInfo>();
            var nsLevel2CodeReference = new CodeReference("assembly", null, null, null, null);
            var nsLevel2 = new Test("nsLevel2", nsLevel2CodeElement);

            using (mocks.Record())
            {
                Expect.Call(nsLevel2CodeElement.CodeReference).Return(nsLevel2CodeReference);
                Expect.Call(nsLevel2CodeElement.GetCodeLocation()).Return(new CodeLocation("about:blank", 1, 1));
            }
            nsLevel2.Kind = Gallio.Model.TestKinds.Namespace;
            nsLevel2.IsTestCase = false;

            var nsLevel2Test = new TestData(nsLevel2, true);
            nsLevel1Test.Children.Add(nsLevel2Test);

            var nsLevel2aCodeElement = mocks.StrictMock<ICodeElementInfo>();
            var nsLevel2aCodeReference = new CodeReference("assembly", null, null, null, null);
            var nsLevel2a = new Test("nsLevel2a", nsLevel2aCodeElement);

            using (mocks.Record())
            {
                Expect.Call(nsLevel2aCodeElement.CodeReference).Return(nsLevel2aCodeReference);
                Expect.Call(nsLevel2aCodeElement.GetCodeLocation()).Return(new CodeLocation("about:blank", 1, 1));
            }
            nsLevel2a.Kind = Gallio.Model.TestKinds.Namespace;
            nsLevel2a.IsTestCase = false;

            var nsLevel2aTest = new TestData(nsLevel2a, true);
            nsLevel1Test.Children.Add(nsLevel2aTest);

            var fixture1CodeElement = mocks.StrictMock<ICodeElementInfo>();
            var fixture1CodeReference = new CodeReference("assembly", "nsRoot.nsLevel1.nsLevel2", "fixture1", null, null);
            var fixture1 = new Test("fixture1", fixture1CodeElement);

            using (mocks.Record())
            {
                Expect.Call(fixture1CodeElement.CodeReference).Return(fixture1CodeReference);
                Expect.Call(fixture1CodeElement.GetCodeLocation()).Return(new CodeLocation("about:blank", 1, 1));
            }
            fixture1.Kind = Gallio.Model.TestKinds.Fixture;
            fixture1.IsTestCase = false;

            var fixture1Test = new TestData(fixture1, true);
            nsLevel2Test.Children.Add(fixture1Test);

            var fixture2CodeElement = mocks.StrictMock<ICodeElementInfo>();
            var fixture2CodeReference = new CodeReference("assembly", "nsRoot.nsLevel1.nsLevel2a", "fixture2", null, null);
            var fixture2 = new Test("fixture2", fixture2CodeElement);

            using (mocks.Record())
            {
                Expect.Call(fixture2CodeElement.CodeReference).Return(fixture2CodeReference);
                Expect.Call(fixture2CodeElement.GetCodeLocation()).Return(new CodeLocation("about:blank", 1, 1));
            }
            fixture2.Kind = Gallio.Model.TestKinds.Fixture;
            fixture2.IsTestCase = false;

            var fixture2Test = new TestData(fixture2, true);
            nsLevel2aTest.Children.Add(fixture2Test);
        }

        [Test]
        public void tree_mode_namespace()
        {
            var node = namespaceTreeBuilder.BuildTree(MockProgressMonitor.Instance, testModelData,
                new TreeBuilderOptions { TreeViewCategory = "Namespace", NamespaceHierarchy = NamespaceHierarchy.Tree });

            mocks.VerifyAll();

            Assert.AreEqual(1, node.Nodes.Count);
            var assembly = node.Nodes[0];
            Assert.AreEqual("assembly", assembly.Text);

            Assert.AreEqual(1, assembly.Nodes.Count);
            var nsRoot = assembly.Nodes[0];
            Assert.AreEqual("nsRoot", nsRoot.Text);

            Assert.AreEqual(1, nsRoot.Nodes.Count);
            var nsLevel1 = nsRoot.Nodes[0];
            Assert.AreEqual("nsLevel1", nsLevel1.Text);

            Assert.AreEqual(2, nsLevel1.Nodes.Count);
            var nsLevel2 = nsLevel1.Nodes[0];
            Assert.AreEqual("nsLevel2", nsLevel2.Text);

            Assert.AreEqual(1, nsLevel2.Nodes.Count);
            var fixture = nsLevel2.Nodes[0];
            Assert.AreEqual("fixture1", fixture.Text);
        }

        [Test]
        public void flat_mode_namespace()
        {
            var node = namespaceTreeBuilder.BuildTree(MockProgressMonitor.Instance, testModelData,
                new TreeBuilderOptions { TreeViewCategory = "Namespace", NamespaceHierarchy = NamespaceHierarchy.Flat });

            mocks.VerifyAll();

            Assert.AreEqual(1, node.Nodes.Count);
            var assembly = node.Nodes[0];
            Assert.AreEqual("assembly", assembly.Text);

            Assert.AreEqual(2, assembly.Nodes.Count);
            Assert.AreEqual("nsRoot.nsLevel1.nsLevel2", assembly.Nodes[0].Text);
            Assert.AreEqual("nsRoot.nsLevel1.nsLevel2a", assembly.Nodes[1].Text);

            Assert.AreEqual(1, assembly.Nodes[0].Nodes.Count);
            Assert.AreEqual("fixture1", assembly.Nodes[0].Nodes[0].Text);

            Assert.AreEqual(1, assembly.Nodes[1].Nodes.Count);
            Assert.AreEqual("fixture2", assembly.Nodes[1].Nodes[0].Text);
        }
    }
}
