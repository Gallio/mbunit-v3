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
using Gallio.Model.Reflection;
using MbUnit.Framework;
using MbUnit.Model;
using Gallio.Model;
using Gallio.TestResources.MbUnit;
using Gallio.Tests.Model;

namespace MbUnit.Tests.Framework.Kernel.Model
{
    [TestFixture]
    [TestsOn(typeof(MbUnitTestFramework))]
    [Author("Jeff", "jeff@ingenio.com")]
    public class MbUnitTestFrameworkTest : BaseTestFrameworkTest
    {
        protected override Assembly GetSampleAssembly()
        {
            return typeof(SimpleTest).Assembly;
        }

        protected override ITestFramework CreateFramework()
        {
            return new MbUnitTestFramework();
        }

        [Test]
        public void NameIsMbUnitGallio()
        {
            Assert.AreEqual("MbUnit v3", framework.Name);
        }

        [Test]
        public void PopulateTestTree_WhenAssemblyDoesNotReferenceFramework_IsEmpty()
        {
            sampleAssembly = typeof(Int32).Assembly;
            PopulateTestTree();

            Assert.AreEqual(0, testModel.RootTest.Children.Count);
        }

        /// <summary>
        /// This is really just a quick sanity check to be sure that the framework
        /// seems to produce a sensible test tree.  More detailed checks on how particular
        /// attributes are handled belong elsewhere.
        /// </summary>
        [Test]
        public void PopulateTestTree_WhenAssemblyReferencesMbUnitGallio_ContainsSimpleTest()
        {
            Version expectedVersion = typeof(MbUnitTestFramework).Assembly.GetName().Version;
            PopulateTestTree();

            RootTest rootTest = testModel.RootTest;
            Assert.IsNull(rootTest.Parent);
            Assert.AreEqual(TestKinds.Root, rootTest.Kind);
            Assert.IsNull(rootTest.CodeElement);
            Assert.AreEqual(1, rootTest.Children.Count);

            MbUnitFrameworkTest frameworkTest = (MbUnitFrameworkTest)rootTest.Children[0];
            Assert.AreSame(rootTest, frameworkTest.Parent);
            Assert.AreEqual(TestKinds.Framework, frameworkTest.Kind);
            Assert.IsNull(frameworkTest.CodeElement);
            Assert.AreEqual(expectedVersion, frameworkTest.Version);
            Assert.AreEqual("MbUnit v" + expectedVersion, frameworkTest.Name);
            Assert.AreEqual(1, frameworkTest.Children.Count);

            MbUnitTest assemblyTest = (MbUnitTest) frameworkTest.Children[0];
            Assert.AreSame(frameworkTest, assemblyTest.Parent);
            Assert.AreEqual(TestKinds.Assembly, assemblyTest.Kind);
            Assert.AreEqual(CodeReference.CreateFromAssembly(sampleAssembly), assemblyTest.CodeElement.CodeReference);
            Assert.AreEqual(sampleAssembly, ((IAssemblyInfo) assemblyTest.CodeElement).Resolve());
            Assert.GreaterEqualThan(assemblyTest.Children.Count, 1);

            MbUnitTest typeTest = (MbUnitTest)GetDescendantByName(assemblyTest, "SimpleTest");
            Assert.IsNotNull(typeTest, "Could not find the SimpleTest fixture.");
            Assert.AreSame(assemblyTest, typeTest.Parent);
            Assert.AreEqual(TestKinds.Fixture, typeTest.Kind);
            Assert.AreEqual(CodeReference.CreateFromType(typeof(SimpleTest)), typeTest.CodeElement.CodeReference);
            Assert.AreEqual(typeof(SimpleTest), ((ITypeInfo) typeTest.CodeElement).Resolve());
            Assert.AreEqual("SimpleTest", typeTest.Name);
            Assert.AreEqual(2, typeTest.Children.Count);

            MbUnitTest passTest = (MbUnitTest)GetDescendantByName(typeTest, "Pass");
            Assert.IsNotNull(passTest, "Could not find the Pass test.");
            Assert.AreSame(typeTest, passTest.Parent);
            Assert.AreEqual(TestKinds.Test, passTest.Kind);
            Assert.AreEqual(CodeReference.CreateFromMember(typeof(SimpleTest).GetMethod("Pass")), passTest.CodeElement.CodeReference);
            Assert.AreEqual(typeof(SimpleTest).GetMethod("Pass"), ((IMethodInfo) passTest.CodeElement).Resolve());
            Assert.AreEqual("Pass", passTest.Name);

            MbUnitTest failTest = (MbUnitTest)GetDescendantByName(typeTest, "Fail");
            Assert.IsNotNull(failTest, "Could not find the Fail test.");
            Assert.AreSame(typeTest, failTest.Parent);
            Assert.AreEqual(TestKinds.Test, failTest.Kind);
            Assert.AreEqual(CodeReference.CreateFromMember(typeof(SimpleTest).GetMethod("Fail")), failTest.CodeElement.CodeReference);
            Assert.AreEqual(typeof(SimpleTest).GetMethod("Fail"), ((IMethodInfo)failTest.CodeElement).Resolve());
            Assert.AreEqual("Fail", failTest.Name);
        }

        [Test]
        public void MetadataImport_XmlDocumentation()
        {
            PopulateTestTree();

            MbUnitTest test = (MbUnitTest)GetDescendantByName(testModel.RootTest, typeof(SimpleTest).Name);
            MbUnitTest passTest = (MbUnitTest)GetDescendantByName(test, "Pass");
            MbUnitTest failTest = (MbUnitTest)GetDescendantByName(test, "Fail");

            Assert.AreEqual("<summary>\nA simple test fixture.\n</summary>", test.Metadata.GetValue(MetadataKeys.XmlDocumentation));
            Assert.AreEqual("<summary>\nA passing test.\n</summary>", passTest.Metadata.GetValue(MetadataKeys.XmlDocumentation));
            Assert.AreEqual("<summary>\nA failing test.\n</summary>", failTest.Metadata.GetValue(MetadataKeys.XmlDocumentation));
        }

        [Test]
        public void MetadataImport_XmlDocumentation_TestParameters()
        {
            PopulateTestTree();

            MbUnitTest test = (MbUnitTest)GetDescendantByName(testModel.RootTest, typeof(ParameterizedTest).Name);
            MbUnitTestParameter fieldParameter = (MbUnitTestParameter) GetParameterByName(test, "FieldParameter");
            MbUnitTestParameter propertyParameter = (MbUnitTestParameter) GetParameterByName(test, "PropertyParameter");

            Assert.AreEqual("<summary>\nA field parameter.\n</summary>", fieldParameter.Metadata.GetValue(MetadataKeys.XmlDocumentation));
            Assert.AreEqual("<summary>\nA property parameter.\n</summary>", propertyParameter.Metadata.GetValue(MetadataKeys.XmlDocumentation));
        }

        [Test]
        public void MetadataImport_AssemblyAttributes()
        {
            PopulateTestTree();

            MbUnitFrameworkTest frameworkTest = (MbUnitFrameworkTest)testModel.RootTest.Children[0];
            MbUnitTest assemblyTest = (MbUnitTest) frameworkTest.Children[0];

            Assert.AreEqual("MbUnit Project", assemblyTest.Metadata.GetValue(MetadataKeys.Company));
            Assert.AreEqual("Test", assemblyTest.Metadata.GetValue(MetadataKeys.Configuration));
            StringAssert.Contains(assemblyTest.Metadata.GetValue(MetadataKeys.Copyright), "MbUnit Project");
            Assert.AreEqual("A sample test assembly for MbUnit.", assemblyTest.Metadata.GetValue(MetadataKeys.Description));
            Assert.AreEqual("MbUnit", assemblyTest.Metadata.GetValue(MetadataKeys.Product));
            Assert.AreEqual("Gallio.TestResources.MbUnit", assemblyTest.Metadata.GetValue(MetadataKeys.Title));
            Assert.AreEqual("MbUnit", assemblyTest.Metadata.GetValue(MetadataKeys.Trademark));

            Assert.AreEqual("1.2.3.4", assemblyTest.Metadata.GetValue(MetadataKeys.InformationalVersion));
            StringAssert.IsNonEmpty(assemblyTest.Metadata.GetValue(MetadataKeys.FileVersion));
            StringAssert.IsNonEmpty(assemblyTest.Metadata.GetValue(MetadataKeys.Version));
        }
    }
}
