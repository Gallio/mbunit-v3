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

using System;
using System.Reflection;
using Gallio.NUnitAdapter.Model;
using Gallio.Reflection;
using MbUnit.Framework;
using Gallio.Model;
using Gallio.NUnitAdapter.TestResources;
using Gallio.NUnitAdapter.TestResources.Metadata;
using Gallio.Tests.Model;

namespace Gallio.NUnitAdapter.Tests.Model
{
    [TestFixture]
    [TestsOn(typeof(NUnitTestFramework))]
    [Author("Jeff", "jeff@ingenio.com")]
    public class NUnitTestFrameworkTest : BaseTestFrameworkTest
    {
        protected override Assembly GetSampleAssembly()
        {
            return typeof(SimpleTest).Assembly;
        }

        protected override ITestFramework CreateFramework()
        {
            return new NUnitTestFramework();
        }

        [Test]
        public void NameIsNUnit()
        {
            Assert.AreEqual("NUnit", framework.Name);
        }

        [Test]
        public void PopulateTestTree_WhenAssemblyDoesNotReferenceFramework_IsEmpty()
        {
            sampleAssembly = typeof(Int32).Assembly;
            PopulateTestTree();

            Assert.AreEqual(0, testModel.RootTest.Children.Count);
        }

        [Test]
        public void PopulateTestTree_CapturesTestStructureAndBasicMetadata()
        {
            Version expectedVersion = typeof(NUnit.Framework.Assert).Assembly.GetName().Version;
            PopulateTestTree();

            RootTest rootTest = testModel.RootTest;
            Assert.IsNull(rootTest.Parent);
            Assert.AreEqual(TestKinds.Root, rootTest.Kind);
            Assert.IsNull(rootTest.CodeElement);
            Assert.IsFalse(rootTest.IsTestCase);
            Assert.AreEqual(1, rootTest.Children.Count);

            BaseTest frameworkTest = (BaseTest)rootTest.Children[0];
            Assert.AreSame(testModel.RootTest, frameworkTest.Parent);
            Assert.AreEqual(TestKinds.Framework, frameworkTest.Kind);
            Assert.IsNull(frameworkTest.CodeElement);
            Assert.AreEqual("NUnit v" + expectedVersion, frameworkTest.Name);
            Assert.IsFalse(frameworkTest.IsTestCase);
            Assert.AreEqual(1, frameworkTest.Children.Count);

            NUnitTest assemblyTest = (NUnitTest)frameworkTest.Children[0];
            Assert.AreSame(frameworkTest, assemblyTest.Parent);
            Assert.AreEqual(TestKinds.Assembly, assemblyTest.Kind);
            Assert.AreEqual(CodeReference.CreateFromAssembly(sampleAssembly), assemblyTest.CodeElement.CodeReference);
            Assert.AreEqual(sampleAssembly.GetName().Name, assemblyTest.Name);
            Assert.IsFalse(assemblyTest.IsTestCase);
            Assert.GreaterEqualThan(assemblyTest.Children.Count, 1);

            NUnitTest fixtureTest = (NUnitTest)GetDescendantByName(assemblyTest, "SimpleTest");
            Assert.AreSame(assemblyTest, fixtureTest.Parent);
            Assert.AreEqual(TestKinds.Fixture, fixtureTest.Kind);
            Assert.AreEqual(new CodeReference(sampleAssembly.FullName, "Gallio.NUnitAdapter.TestResources", "Gallio.NUnitAdapter.TestResources.SimpleTest", null, null),
                fixtureTest.CodeElement.CodeReference);
            Assert.AreEqual("SimpleTest", fixtureTest.Name);
            Assert.IsFalse(fixtureTest.IsTestCase);
            Assert.AreEqual(2, fixtureTest.Children.Count);

            NUnitTest passTest = (NUnitTest)GetDescendantByName(fixtureTest, "Pass");
            NUnitTest failTest = (NUnitTest)GetDescendantByName(fixtureTest, "Fail");
 
            Assert.AreSame(fixtureTest, passTest.Parent);
            Assert.AreEqual(TestKinds.Test, passTest.Kind);
            Assert.AreEqual(new CodeReference(sampleAssembly.FullName, "Gallio.NUnitAdapter.TestResources", "Gallio.NUnitAdapter.TestResources.SimpleTest", "Pass", null),
                passTest.CodeElement.CodeReference);
            Assert.AreEqual("Pass", passTest.Name);
            Assert.IsTrue(passTest.IsTestCase);
            Assert.AreEqual(0, passTest.Children.Count);

            Assert.AreSame(fixtureTest, failTest.Parent);
            Assert.AreEqual(TestKinds.Test, failTest.Kind);
            Assert.AreEqual(new CodeReference(sampleAssembly.FullName, "Gallio.NUnitAdapter.TestResources", "Gallio.NUnitAdapter.TestResources.SimpleTest", "Fail", null),
                failTest.CodeElement.CodeReference);
            Assert.AreEqual("Fail", failTest.Name);
            Assert.IsTrue(failTest.IsTestCase);
            Assert.AreEqual(0, failTest.Children.Count);
        }

        [Test]
        public void MetadataImport_XmlDocumentation()
        {
            PopulateTestTree();

            NUnitTest test = (NUnitTest)GetDescendantByName(testModel.RootTest, typeof(SimpleTest).Name);
            NUnitTest passTest = (NUnitTest)GetDescendantByName(test, "Pass");
            NUnitTest failTest = (NUnitTest)GetDescendantByName(test, "Fail");

            Assert.AreEqual("<summary>\nA simple test fixture.\n</summary>", test.Metadata.GetValue(MetadataKeys.XmlDocumentation));
            Assert.AreEqual("<summary>\nA passing test.\n</summary>", passTest.Metadata.GetValue(MetadataKeys.XmlDocumentation));
            Assert.AreEqual("<summary>\nA failing test.\n</summary>", failTest.Metadata.GetValue(MetadataKeys.XmlDocumentation));
        }

        [Test]
        public void MetadataImport_Description()
        {
            PopulateTestTree();

            NUnitTest test = (NUnitTest)GetDescendantByName(testModel.RootTest, typeof(DescriptionSample).Name);
            Assert.AreEqual("A sample description.", test.Metadata.GetValue(MetadataKeys.Description));
        }

        [Test]
        public void MetadataImport_Category()
        {
            PopulateTestTree();

            NUnitTest test = (NUnitTest)GetDescendantByName(testModel.RootTest, typeof(CategorySample).Name);
            Assert.AreEqual("samples", test.Metadata.GetValue(MetadataKeys.CategoryName));
        }

        [Test]
        public void MetadataImport_IgnoreReason()
        {
            PopulateTestTree();

            NUnitTest fixture = (NUnitTest)GetDescendantByName(testModel.RootTest, typeof(IgnoreReasonSample).Name);
            NUnitTest test = (NUnitTest)fixture.Children[0];
            Assert.AreEqual("For testing purposes.", test.Metadata.GetValue(MetadataKeys.IgnoreReason));
        }

        [Test]
        public void MetadataImport_Property()
        {
            PopulateTestTree();

            NUnitTest test = (NUnitTest)GetDescendantByName(testModel.RootTest, typeof(PropertySample).Name);
            Assert.AreEqual("customvalue-1", test.Metadata.GetValue("customkey-1"));
            Assert.AreEqual("customvalue-2", test.Metadata.GetValue("customkey-2"));
        }

        [Test]
        public void MetadataImport_AssemblyAttributes()
        {
            PopulateTestTree();

            BaseTest frameworkTest = (BaseTest)testModel.RootTest.Children[0];
            NUnitTest assemblyTest = (NUnitTest)frameworkTest.Children[0];

            Assert.AreEqual("MbUnit Project", assemblyTest.Metadata.GetValue(MetadataKeys.Company));
            Assert.AreEqual("Test", assemblyTest.Metadata.GetValue(MetadataKeys.Configuration));
            StringAssert.Contains(assemblyTest.Metadata.GetValue(MetadataKeys.Copyright), "Gallio Project");
            Assert.AreEqual("A sample test assembly for NUnit.", assemblyTest.Metadata.GetValue(MetadataKeys.Description));
            Assert.AreEqual("Gallio", assemblyTest.Metadata.GetValue(MetadataKeys.Product));
            Assert.AreEqual("Gallio.NUnitAdapter.TestResources", assemblyTest.Metadata.GetValue(MetadataKeys.Title));
            Assert.AreEqual("Gallio", assemblyTest.Metadata.GetValue(MetadataKeys.Trademark));

            Assert.AreEqual("1.2.3.4", assemblyTest.Metadata.GetValue(MetadataKeys.InformationalVersion));
            StringAssert.IsNonEmpty(assemblyTest.Metadata.GetValue(MetadataKeys.FileVersion));
            StringAssert.IsNonEmpty(assemblyTest.Metadata.GetValue(MetadataKeys.Version));
        }

        [Test]
        public void DoesNotChokeOnAmbiguousMatch()
        {
            PopulateTestTree();

            NUnitTest fixture = (NUnitTest)GetDescendantByName(testModel.RootTest, typeof(AmbiguousMatchSample).Name);
            Assert.AreEqual(1, fixture.Children.Count);

            NUnitTest test = (NUnitTest) fixture.Children[0];
            Assert.AreEqual("Test", test.Name);
            Assert.IsNull(test.CodeElement);
        }
    }
}
