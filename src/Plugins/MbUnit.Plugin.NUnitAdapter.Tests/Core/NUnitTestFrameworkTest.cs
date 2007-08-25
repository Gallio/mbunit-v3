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
using System.Threading;
using MbUnit._Framework.Tests;
using MbUnit.Core.Harness;
using MbUnit.Core.Tests.Harness;
using MbUnit.Framework.Kernel.Events;
using MbUnit.Framework.Kernel.Model;
using MbUnit.Framework.Kernel.Utilities;
using MbUnit.Framework.Kernel.Runtime;
using MbUnit.Framework.Tests.Kernel.Runtime;
using MbUnit.TestResources.NUnit;
using MbUnit.TestResources.NUnit.Metadata;
using MbUnit2::MbUnit.Framework;

using System.Reflection;
using MbUnit.Framework.Kernel.Metadata;
using MbUnit.Plugin.NUnitAdapter.Core;

using System;

namespace MbUnit.Plugin.NUnitAdapter.Tests.Core
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
        public void PopulateTemplateTree_WhenAssemblyDoesNotReferenceFramework_IsEmpty()
        {
            sampleAssembly = typeof(Int32).Assembly;
            PopulateTemplateTree();

            Assert.AreEqual(0, rootTemplate.Children.Count);
        }

        [Test]
        public void PopulateTemplateTree_WhenAssemblyReferencesNUnit_ContainsJustTheFrameworkTemplate()
        {
            Version expectedVersion = typeof(NUnit.Framework.Assert).Assembly.GetName().Version;
            PopulateTemplateTree();

            Assert.IsNull(rootTemplate.Parent);
            Assert.AreEqual(ComponentKind.Root, rootTemplate.Kind);
            Assert.AreEqual(CodeReference.Unknown, rootTemplate.CodeReference);
            Assert.AreEqual(1, rootTemplate.Children.Count);

            BaseTemplate frameworkTemplate = (BaseTemplate)rootTemplate.Children[0];
            Assert.AreSame(rootTemplate, frameworkTemplate.Parent);
            Assert.AreEqual(ComponentKind.Framework, frameworkTemplate.Kind);
            Assert.AreEqual(CodeReference.Unknown, frameworkTemplate.CodeReference);
            Assert.AreEqual("NUnit v" + expectedVersion, frameworkTemplate.Name);
            Assert.AreEqual(0, frameworkTemplate.Children.Count);
        }

        [Test]
        public void PopulateTestTree_CapturesTestStructureAndBasicMetadata()
        {
            Version expectedVersion = typeof(NUnit.Framework.Assert).Assembly.GetName().Version;
            PopulateTestTree();

            Assert.IsNull(rootTest.Parent);
            Assert.AreEqual(ComponentKind.Root, rootTest.Kind);
            Assert.AreEqual(CodeReference.Unknown, rootTest.CodeReference);
            Assert.IsFalse(rootTest.IsTestCase);
            Assert.IsNull(rootTest.Batch);
            Assert.AreEqual(1, rootTest.Children.Count);

            BaseTest frameworkTest = (BaseTest)rootTest.Children[0];
            Assert.AreSame(rootTest, frameworkTest.Parent);
            Assert.AreEqual(ComponentKind.Framework, frameworkTest.Kind);
            Assert.AreEqual(CodeReference.Unknown, frameworkTest.CodeReference);
            Assert.AreEqual("NUnit v" + expectedVersion, frameworkTest.Name);
            Assert.IsFalse(frameworkTest.IsTestCase);
            Assert.IsNotNull(frameworkTest.Batch);
            TestBatch nunitBatch = frameworkTest.Batch;
            Assert.AreEqual(1, frameworkTest.Children.Count);

            NUnitTest assemblyTest = (NUnitTest)frameworkTest.Children[0];
            Assert.AreSame(frameworkTest, assemblyTest.Parent);
            Assert.AreEqual(ComponentKind.Assembly, assemblyTest.Kind);
            Assert.AreEqual(CodeReference.CreateFromAssembly(sampleAssembly), assemblyTest.CodeReference);
            Assert.AreEqual(sampleAssembly.Location, assemblyTest.Name);
            Assert.IsFalse(assemblyTest.IsTestCase);
            Assert.AreSame(nunitBatch, assemblyTest.Batch);
            Assert.AreEqual(1, assemblyTest.Children.Count);

            NUnitTest namespaceTest = (NUnitTest)assemblyTest.Children[0];
            Assert.AreSame(assemblyTest, namespaceTest.Parent);
            Assert.AreEqual(ComponentKind.Namespace, namespaceTest.Kind);
            Assert.AreEqual(new CodeReference(sampleAssembly.FullName, "MbUnit", null, null, null), namespaceTest.CodeReference);
            Assert.AreEqual("MbUnit", namespaceTest.Name);
            Assert.IsFalse(namespaceTest.IsTestCase);
            Assert.AreSame(nunitBatch, namespaceTest.Batch);
            Assert.AreEqual(1, namespaceTest.Children.Count);

            NUnitTest namespaceTest2 = (NUnitTest)namespaceTest.Children[0];
            Assert.AreSame(namespaceTest, namespaceTest2.Parent);
            Assert.AreEqual(ComponentKind.Namespace, namespaceTest2.Kind);
            Assert.AreEqual(new CodeReference(sampleAssembly.FullName, "MbUnit.TestResources", null, null, null), namespaceTest2.CodeReference);
            Assert.AreEqual("MbUnit.TestResources", namespaceTest2.Name);
            Assert.IsFalse(namespaceTest2.IsTestCase);
            Assert.AreSame(nunitBatch, namespaceTest2.Batch);
            Assert.AreEqual(1, namespaceTest2.Children.Count);

            NUnitTest namespaceTest3 = (NUnitTest)namespaceTest2.Children[0];
            Assert.AreSame(namespaceTest2, namespaceTest3.Parent);
            Assert.AreEqual(ComponentKind.Namespace, namespaceTest3.Kind);
            Assert.AreEqual(new CodeReference(sampleAssembly.FullName, "MbUnit.TestResources.NUnit", null, null, null), namespaceTest3.CodeReference);
            Assert.AreEqual("MbUnit.TestResources.NUnit", namespaceTest3.Name);
            Assert.IsFalse(namespaceTest3.IsTestCase);
            Assert.AreSame(nunitBatch, namespaceTest3.Batch);
            Assert.GreaterEqualThan(namespaceTest3.Children.Count, 1);

            NUnitTest fixtureTest = (NUnitTest)GetDescendantByName(namespaceTest3, "MbUnit.TestResources.NUnit.SimpleTest");
            Assert.AreSame(namespaceTest3, fixtureTest.Parent);
            Assert.AreEqual(ComponentKind.Fixture, fixtureTest.Kind);
            Assert.AreEqual(new CodeReference(sampleAssembly.FullName, "MbUnit.TestResources.NUnit", "MbUnit.TestResources.NUnit.SimpleTest", null, null), fixtureTest.CodeReference);
            Assert.AreEqual("MbUnit.TestResources.NUnit.SimpleTest", fixtureTest.Name);
            Assert.IsFalse(fixtureTest.IsTestCase);
            Assert.AreSame(nunitBatch, fixtureTest.Batch);
            Assert.AreEqual(2, fixtureTest.Children.Count);

            NUnitTest passTest = (NUnitTest)GetDescendantByName(fixtureTest, "MbUnit.TestResources.NUnit.SimpleTest.Pass");
            NUnitTest failTest = (NUnitTest)GetDescendantByName(fixtureTest, "MbUnit.TestResources.NUnit.SimpleTest.Fail");
 
            Assert.AreSame(fixtureTest, passTest.Parent);
            Assert.AreEqual(ComponentKind.Test, passTest.Kind);
            Assert.AreEqual(new CodeReference(sampleAssembly.FullName, "MbUnit.TestResources.NUnit", "MbUnit.TestResources.NUnit.SimpleTest", "Pass", null), passTest.CodeReference);
            Assert.AreEqual("MbUnit.TestResources.NUnit.SimpleTest.Pass", passTest.Name);
            Assert.IsTrue(passTest.IsTestCase);
            Assert.AreSame(nunitBatch, passTest.Batch);
            Assert.AreEqual(0, passTest.Children.Count);

            Assert.AreSame(fixtureTest, failTest.Parent);
            Assert.AreEqual(ComponentKind.Test, failTest.Kind);
            Assert.AreEqual(new CodeReference(sampleAssembly.FullName, "MbUnit.TestResources.NUnit", "MbUnit.TestResources.NUnit.SimpleTest", "Fail", null), failTest.CodeReference);
            Assert.AreEqual("MbUnit.TestResources.NUnit.SimpleTest.Fail", failTest.Name);
            Assert.IsTrue(failTest.IsTestCase);
            Assert.AreSame(nunitBatch, failTest.Batch);
            Assert.AreEqual(0, failTest.Children.Count);
        }

        [Test]
        public void MetadataImport_Description()
        {
            PopulateTestTree();

            NUnitTest test = (NUnitTest)GetDescendantByName(rootTest, typeof(DescriptionSample).FullName);
            Assert.AreEqual("A sample description.", test.Metadata.GetValue(MetadataKey.Description));
        }

        [Test]
        public void MetadataImport_Category()
        {
            PopulateTestTree();

            NUnitTest test = (NUnitTest)GetDescendantByName(rootTest, typeof(CategorySample).FullName);
            Assert.AreEqual("samples", test.Metadata.GetValue(MetadataKey.CategoryName));
        }

        [Test]
        public void MetadataImport_IgnoreReason()
        {
            PopulateTestTree();

            NUnitTest fixture = (NUnitTest)GetDescendantByName(rootTest, typeof(IgnoreReasonSample).FullName);
            NUnitTest test = (NUnitTest)fixture.Children[0];
            Assert.AreEqual("For testing purposes.", test.Metadata.GetValue(MetadataKey.IgnoreReason));
        }

        [Test]
        public void MetadataImport_Property()
        {
            PopulateTestTree();

            NUnitTest test = (NUnitTest)GetDescendantByName(rootTest, typeof(PropertySample).FullName);
            Assert.AreEqual("customvalue-1", test.Metadata.GetValue("customkey-1"));
            Assert.AreEqual("customvalue-2", test.Metadata.GetValue("customkey-2"));
        }
    }
}
