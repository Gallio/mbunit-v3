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

using System;
using System.Reflection;
using Gallio.Model;
using Gallio.Plugin.XunitAdapter.Model;
using Gallio.TestResources.Xunit;
using Gallio.TestResources.Xunit.Metadata;
using Gallio.Tests.Model;
using MbUnit.Framework;
using Assert=Xunit.Assert;

namespace Gallio.Plugin.XunitAdapter.Tests.Model
{
    [TestFixture]
    [TestsOn(typeof(XunitTestFramework))]
    [Author("Jeff", "jeff@ingenio.com")]
    public class XunitTestFrameworkTest : BaseTestFrameworkTest
    {
        protected override Assembly GetSampleAssembly()
        {
            return typeof(SimpleTest).Assembly;
        }

        protected override ITestFramework CreateFramework()
        {
            return new XunitTestFramework();
        }

        [Test]
        public void NameIsXunit()
        {
            MbUnit.Framework.Assert.AreEqual("xUnit.Net", framework.Name);
        }

        [Test]
        public void PopulateTemplateTree_WhenAssemblyDoesNotReferenceFramework_IsEmpty()
        {
            sampleAssembly = typeof(Int32).Assembly;
            PopulateTemplateTree();

            MbUnit.Framework.Assert.AreEqual(0, rootTemplate.Children.Count);
        }

        [Test]
        public void PopulateTemplateTree_WhenAssemblyReferencesXunit_ContainsJustTheFrameworkTemplate()
        {
            Version expectedVersion = typeof(Assert).Assembly.GetName().Version;
            PopulateTemplateTree();

            MbUnit.Framework.Assert.IsNull(rootTemplate.Parent);
            MbUnit.Framework.Assert.AreEqual(ComponentKind.Root, rootTemplate.Kind);
            MbUnit.Framework.Assert.AreEqual(CodeReference.Unknown, rootTemplate.CodeReference);
            MbUnit.Framework.Assert.IsTrue(rootTemplate.IsGenerator);
            MbUnit.Framework.Assert.AreEqual(1, rootTemplate.Children.Count);

            BaseTemplate frameworkTemplate = (BaseTemplate)rootTemplate.Children[0];
            MbUnit.Framework.Assert.AreSame(rootTemplate, frameworkTemplate.Parent);
            MbUnit.Framework.Assert.AreEqual(ComponentKind.Framework, frameworkTemplate.Kind);
            MbUnit.Framework.Assert.AreEqual(CodeReference.Unknown, frameworkTemplate.CodeReference);
            MbUnit.Framework.Assert.AreEqual("xUnit.Net v" + expectedVersion, frameworkTemplate.Name);
            MbUnit.Framework.Assert.IsTrue(frameworkTemplate.IsGenerator);
            MbUnit.Framework.Assert.AreEqual(0, frameworkTemplate.Children.Count);
        }

        [Test]
        public void PopulateTestTree_CapturesTestStructureAndBasicMetadata()
        {
            Version expectedVersion = typeof(Assert).Assembly.GetName().Version;
            PopulateTestTree();

            MbUnit.Framework.Assert.IsNull(rootTest.Parent);
            MbUnit.Framework.Assert.AreEqual(ComponentKind.Root, rootTest.Kind);
            MbUnit.Framework.Assert.AreEqual(CodeReference.Unknown, rootTest.CodeReference);
            MbUnit.Framework.Assert.IsFalse(rootTest.IsTestCase);
            MbUnit.Framework.Assert.AreEqual(1, rootTest.Children.Count);

            BaseTest frameworkTest = (BaseTest)rootTest.Children[0];
            MbUnit.Framework.Assert.AreSame(rootTest, frameworkTest.Parent);
            MbUnit.Framework.Assert.AreEqual(ComponentKind.Framework, frameworkTest.Kind);
            MbUnit.Framework.Assert.AreEqual(CodeReference.Unknown, frameworkTest.CodeReference);
            MbUnit.Framework.Assert.AreEqual("xUnit.Net v" + expectedVersion, frameworkTest.Name);
            MbUnit.Framework.Assert.IsFalse(frameworkTest.IsTestCase);
            MbUnit.Framework.Assert.AreEqual(1, frameworkTest.Children.Count);

            BaseTest assemblyTest = (BaseTest)frameworkTest.Children[0];
            MbUnit.Framework.Assert.AreSame(frameworkTest, assemblyTest.Parent);
            MbUnit.Framework.Assert.AreEqual(ComponentKind.Assembly, assemblyTest.Kind);
            MbUnit.Framework.Assert.AreEqual(CodeReference.CreateFromAssembly(sampleAssembly), assemblyTest.CodeReference);
            MbUnit.Framework.Assert.AreEqual(sampleAssembly.GetName().Name, assemblyTest.Name);
            MbUnit.Framework.Assert.IsFalse(assemblyTest.IsTestCase);
            MbUnit.Framework.Assert.GreaterEqualThan(assemblyTest.Children.Count, 1);

            XunitTest fixtureTest = (XunitTest)GetDescendantByName(assemblyTest, "SimpleTest");
            MbUnit.Framework.Assert.AreSame(assemblyTest, fixtureTest.Parent);
            MbUnit.Framework.Assert.AreEqual(ComponentKind.Fixture, fixtureTest.Kind);
            MbUnit.Framework.Assert.AreEqual(new CodeReference(sampleAssembly.FullName, "Gallio.TestResources.Xunit", "Gallio.TestResources.Xunit.SimpleTest", null, null),
                fixtureTest.CodeReference);
            MbUnit.Framework.Assert.AreEqual("SimpleTest", fixtureTest.Name);
            MbUnit.Framework.Assert.IsFalse(fixtureTest.IsTestCase);
            MbUnit.Framework.Assert.AreEqual(2, fixtureTest.Children.Count);

            XunitTest passTest = (XunitTest)GetDescendantByName(fixtureTest, "Pass");
            XunitTest failTest = (XunitTest)GetDescendantByName(fixtureTest, "Fail");
 
            MbUnit.Framework.Assert.AreSame(fixtureTest, passTest.Parent);
            MbUnit.Framework.Assert.AreEqual(ComponentKind.Test, passTest.Kind);
            MbUnit.Framework.Assert.AreEqual(new CodeReference(sampleAssembly.FullName, "Gallio.TestResources.Xunit", "Gallio.TestResources.Xunit.SimpleTest", "Pass", null), passTest.CodeReference);
            MbUnit.Framework.Assert.AreEqual("Pass", passTest.Name);
            MbUnit.Framework.Assert.IsTrue(passTest.IsTestCase);
            MbUnit.Framework.Assert.AreEqual(0, passTest.Children.Count);

            MbUnit.Framework.Assert.AreSame(fixtureTest, failTest.Parent);
            MbUnit.Framework.Assert.AreEqual(ComponentKind.Test, failTest.Kind);
            MbUnit.Framework.Assert.AreEqual(new CodeReference(sampleAssembly.FullName, "Gallio.TestResources.Xunit", "Gallio.TestResources.Xunit.SimpleTest", "Fail", null), failTest.CodeReference);
            MbUnit.Framework.Assert.AreEqual("Fail", failTest.Name);
            MbUnit.Framework.Assert.IsTrue(failTest.IsTestCase);
            MbUnit.Framework.Assert.AreEqual(0, failTest.Children.Count);
        }

        [Test]
        public void MetadataImport_XmlDocumentation()
        {
            PopulateTestTree();

            XunitTest test = (XunitTest)GetDescendantByName(rootTest, typeof(SimpleTest).Name);
            XunitTest passTest = (XunitTest)GetDescendantByName(test, "Pass");
            XunitTest failTest = (XunitTest)GetDescendantByName(test, "Fail");

            MbUnit.Framework.Assert.AreEqual("<summary>\nA simple test fixture.\n</summary>", test.Metadata.GetValue(MetadataKeys.XmlDocumentation));
            MbUnit.Framework.Assert.AreEqual("<summary>\nA passing test.\n</summary>", passTest.Metadata.GetValue(MetadataKeys.XmlDocumentation));
            MbUnit.Framework.Assert.AreEqual("<summary>\nA failing test.\n</summary>", failTest.Metadata.GetValue(MetadataKeys.XmlDocumentation));
        }

        [Test]
        public void MetadataImport_SkipReason()
        {
            PopulateTestTree();

            XunitTest fixture = (XunitTest)GetDescendantByName(rootTest, typeof(MetadataSample).Name);
            XunitTest test = (XunitTest)fixture.Children[0];
            MbUnit.Framework.Assert.AreEqual("For testing purposes.", test.Metadata.GetValue(MetadataKeys.IgnoreReason));
        }

        [Test]
        public void MetadataImport_Property()
        {
            PopulateTestTree();

            XunitTest fixture = (XunitTest)GetDescendantByName(rootTest, typeof(MetadataSample).Name);
            XunitTest test = (XunitTest)fixture.Children[0];
            MbUnit.Framework.Assert.AreEqual("customvalue-1", test.Metadata.GetValue("customkey-1"));
            MbUnit.Framework.Assert.AreEqual("customvalue-2", test.Metadata.GetValue("customkey-2"));
        }

        [Test]
        public void MetadataImport_AssemblyAttributes()
        {
            PopulateTestTree();

            ITest frameworkTest = rootTest.Children[0];
            ITest assemblyTest = frameworkTest.Children[0];

            MbUnit.Framework.Assert.AreEqual("MbUnit Project", assemblyTest.Metadata.GetValue(MetadataKeys.Company));
            MbUnit.Framework.Assert.AreEqual("Test", assemblyTest.Metadata.GetValue(MetadataKeys.Configuration));
            StringAssert.Contains(assemblyTest.Metadata.GetValue(MetadataKeys.Copyright), "MbUnit Project");
            MbUnit.Framework.Assert.AreEqual("A sample test assembly for xUnit.Net.", assemblyTest.Metadata.GetValue(MetadataKeys.Description));
            MbUnit.Framework.Assert.AreEqual("Gallio", assemblyTest.Metadata.GetValue(MetadataKeys.Product));
            MbUnit.Framework.Assert.AreEqual("Gallio.TestResources.Xunit", assemblyTest.Metadata.GetValue(MetadataKeys.Title));
            MbUnit.Framework.Assert.AreEqual("Gallio", assemblyTest.Metadata.GetValue(MetadataKeys.Trademark));

            MbUnit.Framework.Assert.AreEqual("1.2.3.4", assemblyTest.Metadata.GetValue(MetadataKeys.InformationalVersion));
            StringAssert.IsNonEmpty(assemblyTest.Metadata.GetValue(MetadataKeys.FileVersion));
            StringAssert.IsNonEmpty(assemblyTest.Metadata.GetValue(MetadataKeys.Version));
        }
    }
}
