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

extern alias MbUnit2;
using System;
using System.Reflection;
using Gallio.Plugin.MbUnit2Adapter.Model;
using MbUnit.Framework;
using Gallio.Model;
using Gallio.TestResources.MbUnit2;
using Gallio.TestResources.MbUnit2.Metadata;
using Gallio.Tests.Model;

namespace Gallio.Plugin.MbUnit2Adapter.Tests.Model
{
    [TestFixture]
    [TestsOn(typeof(MbUnit2TestFramework))]
    [Author("Jeff", "jeff@ingenio.com")]
    public class MbUnit2TestFrameworkTest : BaseTestFrameworkTest
    {
        protected override Assembly GetSampleAssembly()
        {
            return typeof(SimpleTest).Assembly;
        }

        protected override ITestFramework CreateFramework()
        {
            return new MbUnit2TestFramework();
        }

        [Test]
        public void NameIsMbUnitV2()
        {
            Assert.AreEqual("MbUnit v2", framework.Name);
        }

        [Test]
        public void PopulateTemplateTree_WhenAssemblyDoesNotReferenceFramework_IsEmpty()
        {
            sampleAssembly = typeof(Int32).Assembly;
            PopulateTemplateTree();

            Assert.AreEqual(0, rootTemplate.Children.Count);
        }

        [Test]
        public void PopulateTemplateTree_WhenAssemblyReferencesMbUnitV2_ContainsJustTheFrameworkTemplate()
        {
            Version expectedVersion = typeof(MbUnit2::MbUnit.Framework.Assert).Assembly.GetName().Version;
            PopulateTemplateTree();

            Assert.IsNull(rootTemplate.Parent);
            Assert.AreEqual(ComponentKind.Root, rootTemplate.Kind);
            Assert.AreEqual(CodeReference.Unknown, rootTemplate.CodeReference);
            Assert.IsTrue(rootTemplate.IsGenerator);
            Assert.AreEqual(1, rootTemplate.Children.Count);

            BaseTemplate frameworkTemplate = (BaseTemplate)rootTemplate.Children[0];
            Assert.AreSame(rootTemplate, frameworkTemplate.Parent);
            Assert.AreEqual(ComponentKind.Framework, frameworkTemplate.Kind);
            Assert.AreEqual(CodeReference.Unknown, frameworkTemplate.CodeReference);
            Assert.AreEqual("MbUnit v" + expectedVersion, frameworkTemplate.Name);
            Assert.IsTrue(frameworkTemplate.IsGenerator);
            Assert.AreEqual(1, frameworkTemplate.Children.Count);

            MbUnit2AssemblyTemplate assemblyTemplate = (MbUnit2AssemblyTemplate)frameworkTemplate.Children[0];
            Assert.AreSame(frameworkTemplate, assemblyTemplate.Parent);
            Assert.AreEqual(ComponentKind.Assembly, assemblyTemplate.Kind);
            Assert.AreEqual(CodeReference.CreateFromAssembly(sampleAssembly), assemblyTemplate.CodeReference);
            Assert.IsTrue(assemblyTemplate.IsGenerator);
            Assert.AreEqual(0, assemblyTemplate.Children.Count);
        }

        [Test]
        public void PopulateTestTree_CapturesTestStructureAndBasicMetadata()
        {
            Version expectedVersion = typeof(MbUnit2::MbUnit.Framework.Assert).Assembly.GetName().Version;
            PopulateTestTree();

            Assert.IsNull(rootTest.Parent);
            Assert.AreEqual(ComponentKind.Root, rootTest.Kind);
            Assert.AreEqual(CodeReference.Unknown, rootTest.CodeReference);
            Assert.IsFalse(rootTest.IsTestCase);
            Assert.AreEqual(1, rootTest.Children.Count);

            BaseTest frameworkTest = (BaseTest)rootTest.Children[0];
            Assert.AreSame(rootTest, frameworkTest.Parent);
            Assert.AreEqual(ComponentKind.Framework, frameworkTest.Kind);
            Assert.AreEqual(CodeReference.Unknown, frameworkTest.CodeReference);
            Assert.AreEqual("MbUnit v" + expectedVersion, frameworkTest.Name);
            Assert.IsFalse(frameworkTest.IsTestCase);
            Assert.GreaterEqualThan(frameworkTest.Children.Count, 1);

            MbUnit2Test assemblyTest = (MbUnit2Test)frameworkTest.Children[0];
            Assert.AreSame(frameworkTest, assemblyTest.Parent);
            Assert.AreEqual(ComponentKind.Assembly, assemblyTest.Kind);
            Assert.AreEqual(CodeReference.CreateFromAssembly(sampleAssembly), assemblyTest.CodeReference);
            Assert.AreEqual(sampleAssembly.GetName().Name, assemblyTest.Name);
            Assert.IsFalse(assemblyTest.IsTestCase);
            Assert.GreaterEqualThan(assemblyTest.Children.Count, 1);

            MbUnit2Test fixtureTest = (MbUnit2Test)GetDescendantByName(assemblyTest, typeof(SimpleTest).Name);
            Assert.AreSame(assemblyTest, fixtureTest.Parent);
            Assert.AreEqual(ComponentKind.Fixture, fixtureTest.Kind);
            Assert.AreEqual(new CodeReference(sampleAssembly.FullName, "MbUnit.TestResources.MbUnit2", "MbUnit.TestResources.MbUnit2.SimpleTest", null, null), fixtureTest.CodeReference);
            Assert.AreEqual("SimpleTest", fixtureTest.Name);
            Assert.AreEqual("SimpleTest", fixtureTest.Name);
            Assert.IsFalse(fixtureTest.IsTestCase);
            Assert.AreEqual(2, fixtureTest.Children.Count);

            MbUnit2Test passTest = (MbUnit2Test)GetDescendantByName(fixtureTest, "SimpleTest.Pass");
            MbUnit2Test failTest = (MbUnit2Test)GetDescendantByName(fixtureTest, "SimpleTest.Fail");

            Assert.AreSame(fixtureTest, passTest.Parent);
            Assert.AreEqual(ComponentKind.Test, passTest.Kind);
            Assert.AreEqual(new CodeReference(sampleAssembly.FullName, "MbUnit.TestResources.MbUnit2", "MbUnit.TestResources.MbUnit2.SimpleTest", "Pass", null), passTest.CodeReference);
            Assert.AreEqual("SimpleTest.Pass", passTest.Name);
            Assert.IsTrue(passTest.IsTestCase);
            Assert.AreEqual(0, passTest.Children.Count);

            Assert.AreSame(fixtureTest, failTest.Parent);
            Assert.AreEqual(ComponentKind.Test, failTest.Kind);
            Assert.AreEqual(new CodeReference(sampleAssembly.FullName, "MbUnit.TestResources.MbUnit2", "MbUnit.TestResources.MbUnit2.SimpleTest", "Fail", null), failTest.CodeReference);
            Assert.AreEqual("SimpleTest.Fail", failTest.Name);
            Assert.IsTrue(failTest.IsTestCase);
            Assert.AreEqual(0, failTest.Children.Count);
        }

        [Test]
        public void MetadataImport_XmlDocumentation()
        {
            PopulateTestTree();

            MbUnit2Test test = (MbUnit2Test)GetDescendantByName(rootTest, typeof(SimpleTest).Name);
            MbUnit2Test passTest = (MbUnit2Test)GetDescendantByName(test, "SimpleTest.Pass");
            MbUnit2Test failTest = (MbUnit2Test)GetDescendantByName(test, "SimpleTest.Fail");

            Assert.AreEqual("<summary>\nA simple test fixture.\n</summary>", test.Metadata.GetValue(MetadataKeys.XmlDocumentation));
            Assert.AreEqual("<summary>\nA passing test.\n</summary>", passTest.Metadata.GetValue(MetadataKeys.XmlDocumentation));
            Assert.AreEqual("<summary>\nA failing test.\n</summary>", failTest.Metadata.GetValue(MetadataKeys.XmlDocumentation));
        }

        [Test]
        public void MetadataImport_AuthorName()
        {
            PopulateTestTree();

            MbUnit2Test test = (MbUnit2Test) GetDescendantByName(rootTest, typeof(AuthorNameSample).Name);
            Assert.AreEqual("joe", test.Metadata.GetValue(MetadataKeys.AuthorName));
            Assert.AreEqual(null, test.Metadata.GetValue(MetadataKeys.AuthorEmail));
            Assert.AreEqual(null, test.Metadata.GetValue(MetadataKeys.AuthorHomepage));
        }

        [Test]
        public void MetadataImport_AuthorNameAndEmail()
        {
            PopulateTestTree();

            MbUnit2Test test = (MbUnit2Test)GetDescendantByName(rootTest, typeof(AuthorNameAndEmailSample).Name);
            Assert.AreEqual("joe", test.Metadata.GetValue(MetadataKeys.AuthorName));
            Assert.AreEqual("joe@example.com", test.Metadata.GetValue(MetadataKeys.AuthorEmail));
            Assert.AreEqual(null, test.Metadata.GetValue(MetadataKeys.AuthorHomepage));
        }

        [Test]
        public void MetadataImport_AuthorNameAndEmailAndHomepage()
        {
            PopulateTestTree();

            MbUnit2Test test = (MbUnit2Test)GetDescendantByName(rootTest, typeof(AuthorNameAndEmailAndHomepageSample).Name);
            Assert.AreEqual("joe", test.Metadata.GetValue(MetadataKeys.AuthorName));
            Assert.AreEqual("joe@example.com", test.Metadata.GetValue(MetadataKeys.AuthorEmail));
            Assert.AreEqual("http://www.example.com/", test.Metadata.GetValue(MetadataKeys.AuthorHomepage));
        }

        [Test]
        public void MetadataImport_FixtureCategory()
        {
            PopulateTestTree();

            MbUnit2Test test = (MbUnit2Test)GetDescendantByName(rootTest, typeof(FixtureCategorySample).Name);
            Assert.AreEqual("samples", test.Metadata.GetValue(MetadataKeys.CategoryName));
        }

        [Test]
        public void MetadataImport_Importance()
        {
            PopulateTestTree();

            MbUnit2Test test = (MbUnit2Test)GetDescendantByName(rootTest, typeof(ImportanceSample).Name);
            Assert.AreEqual(MbUnit2::MbUnit.Framework.TestImportance.Critical.ToString(), test.Metadata.GetValue(MetadataKeys.Importance));
        }

        [Test]
        public void MetadataImport_TestsOn()
        {
            PopulateTestTree();

            MbUnit2Test test = (MbUnit2Test)GetDescendantByName(rootTest, typeof(TestsOnSample).Name);
            Assert.AreEqual(typeof(TestsOnSample).AssemblyQualifiedName, test.Metadata.GetValue(MetadataKeys.TestsOn));
        }

        [Test]
        public void MetadataImport_FixtureDescription()
        {
            PopulateTestTree();

            MbUnit2Test test = (MbUnit2Test)GetDescendantByName(rootTest, typeof(FixtureDescriptionSample).Name);
            Assert.AreEqual("A sample description.", test.Metadata.GetValue(MetadataKeys.Description));
        }

        [Test]
        public void MetadataImport_TestDescription()
        {
            PopulateTestTree();

            MbUnit2Test fixture = (MbUnit2Test)GetDescendantByName(rootTest, typeof(TestDescriptionSample).Name);
            MbUnit2Test test = (MbUnit2Test)fixture.Children[0];
            Assert.AreEqual("A sample description.", test.Metadata.GetValue(MetadataKeys.Description));
        }

        [Test]
        public void MetadataImport_AssemblyAttributes()
        {
            PopulateTemplateTree();

            BaseTemplate frameworkTemplate = (BaseTemplate)rootTemplate.Children[0];
            MbUnit2AssemblyTemplate assemblyTemplate = (MbUnit2AssemblyTemplate)frameworkTemplate.Children[0];

            Assert.AreEqual("MbUnit", assemblyTemplate.Metadata.GetValue(MetadataKeys.Company));
            Assert.AreEqual("Test", assemblyTemplate.Metadata.GetValue(MetadataKeys.Configuration));
            StringAssert.Contains(assemblyTemplate.Metadata.GetValue(MetadataKeys.Copyright), "MbUnit Project");
            Assert.AreEqual("A sample test assembly.", assemblyTemplate.Metadata.GetValue(MetadataKeys.Description));
            Assert.AreEqual("MbUnit.TestResources.MbUnit2", assemblyTemplate.Metadata.GetValue(MetadataKeys.Product));
            Assert.AreEqual("MbUnit.TestResources.MbUnit2", assemblyTemplate.Metadata.GetValue(MetadataKeys.Title));
            Assert.AreEqual("MbUnit", assemblyTemplate.Metadata.GetValue(MetadataKeys.Trademark));

            Assert.AreEqual("1.2.3.4", assemblyTemplate.Metadata.GetValue(MetadataKeys.InformationalVersion));
            StringAssert.IsNonEmpty(assemblyTemplate.Metadata.GetValue(MetadataKeys.FileVersion));
            StringAssert.IsNonEmpty(assemblyTemplate.Metadata.GetValue(MetadataKeys.Version));
        }
    }
}
