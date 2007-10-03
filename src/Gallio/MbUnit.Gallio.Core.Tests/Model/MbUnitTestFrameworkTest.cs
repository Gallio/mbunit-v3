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
using MbUnit.Core.Model;
using MbUnit.Framework;
using MbUnit.Framework.Kernel.Model;
using MbUnit.Framework.Kernel.Utilities;
using MbUnit.TestResources.Gallio;

namespace MbUnit.Core.Tests.Model
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
            Assert.AreEqual("MbUnit Gallio", framework.Name);
        }

        [Test]
        public void PopulateTemplateTree_WhenAssemblyDoesNotReferenceFramework_IsEmpty()
        {
            sampleAssembly = typeof(Int32).Assembly;
            PopulateTemplateTree();

            Assert.AreEqual(0, rootTemplate.Children.Count);
        }

        /// <summary>
        /// This is really just a quick sanity check to be sure that the framework
        /// seems to produce a sensible test tree.  More detailed checks on how particular
        /// attributes are handled belong elsewhere.
        /// </summary>
        [Test]
        public void PopulateTemplateTree_WhenAssemblyReferencesMbUnitGallio_ContainsSimpleTest()
        {
            Version expectedVersion = typeof(MbUnitTestFramework).Assembly.GetName().Version;
            PopulateTemplateTree();

            Assert.IsNull(rootTemplate.Parent);
            Assert.AreEqual(ComponentKind.Root, rootTemplate.Kind);
            Assert.AreEqual(CodeReference.Unknown, rootTemplate.CodeReference);
            Assert.AreEqual(1, rootTemplate.Children.Count);

            MbUnitFrameworkTemplate frameworkTemplate = (MbUnitFrameworkTemplate)rootTemplate.Children[0];
            Assert.AreSame(rootTemplate, frameworkTemplate.Parent);
            Assert.AreEqual(ComponentKind.Framework, frameworkTemplate.Kind);
            Assert.AreEqual(CodeReference.Unknown, frameworkTemplate.CodeReference);
            Assert.AreEqual(expectedVersion, frameworkTemplate.Version);
            Assert.AreEqual("MbUnit Gallio v" + expectedVersion, frameworkTemplate.Name);
            Assert.AreEqual(1, frameworkTemplate.AssemblyTemplates.Count);

            MbUnitAssemblyTemplate assemblyTemplate = frameworkTemplate.AssemblyTemplates[0];
            Assert.AreSame(frameworkTemplate, assemblyTemplate.Parent);
            Assert.AreEqual(ComponentKind.Assembly, assemblyTemplate.Kind);
            Assert.AreEqual(CodeReference.CreateFromAssembly(sampleAssembly), assemblyTemplate.CodeReference);
            Assert.AreEqual(sampleAssembly, assemblyTemplate.Assembly);
            Assert.GreaterEqualThan(assemblyTemplate.TypeTemplates.Count, 1);

            MbUnitTypeTemplate typeTemplate = GenericUtils.Find(assemblyTemplate.TypeTemplates, delegate(MbUnitTypeTemplate template)
            {
                return template.Type == typeof(SimpleTest);
            });
            Assert.IsNotNull(typeTemplate, "Could not find the SimpleTest fixture.");
            Assert.AreSame(assemblyTemplate, typeTemplate.Parent);
            Assert.AreEqual(ComponentKind.Fixture, typeTemplate.Kind);
            Assert.AreEqual(CodeReference.CreateFromType(typeof(SimpleTest)), typeTemplate.CodeReference);
            Assert.AreEqual(typeof(SimpleTest), typeTemplate.Type);
            Assert.AreEqual("SimpleTest", typeTemplate.Name);
            Assert.AreEqual(2, typeTemplate.MethodTemplates.Count);

            MbUnitMethodTemplate passTemplate = GenericUtils.Find(typeTemplate.MethodTemplates, delegate(MbUnitMethodTemplate template)
            {
                return template.Name == "Pass";
            });
            Assert.IsNotNull(passTemplate, "Could not find the Pass test.");
            Assert.AreSame(typeTemplate, passTemplate.Parent);
            Assert.AreEqual(ComponentKind.Test, passTemplate.Kind);
            Assert.AreEqual(CodeReference.CreateFromMember(typeof(SimpleTest).GetMethod("Pass")), passTemplate.CodeReference);
            Assert.AreEqual(typeof(SimpleTest).GetMethod("Pass"), passTemplate.Method);
            Assert.AreEqual("Pass", passTemplate.Name);

            MbUnitMethodTemplate failTemplate = GenericUtils.Find(typeTemplate.MethodTemplates, delegate(MbUnitMethodTemplate template)
            {
                return template.Name == "Fail";
            });
            Assert.IsNotNull(failTemplate, "Could not find the Fail test.");
            Assert.AreSame(typeTemplate, failTemplate.Parent);
            Assert.AreEqual(ComponentKind.Test, failTemplate.Kind);
            Assert.AreEqual(CodeReference.CreateFromMember(typeof(SimpleTest).GetMethod("Fail")), failTemplate.CodeReference);
            Assert.AreEqual(typeof(SimpleTest).GetMethod("Fail"), failTemplate.Method);
            Assert.AreEqual("Fail", failTemplate.Name);
        }

        [Test]
        public void MetadataImport_XmlDocumentation()
        {
            PopulateTestTree();

            MbUnitTest test = (MbUnitTest)GetDescendantByName(rootTest, typeof(SimpleTest).Name);
            MbUnitTest passTest = (MbUnitTest)GetDescendantByName(test, "Pass");
            MbUnitTest failTest = (MbUnitTest)GetDescendantByName(test, "Fail");

            Assert.AreEqual("<summary>\nA simple test fixture.\n</summary>", test.Metadata.GetValue(MetadataKeys.XmlDocumentation));
            Assert.AreEqual("<summary>\nA passing test.\n</summary>", passTest.Metadata.GetValue(MetadataKeys.XmlDocumentation));
            Assert.AreEqual("<summary>\nA failing test.\n</summary>", failTest.Metadata.GetValue(MetadataKeys.XmlDocumentation));
        }

        [Test]
        public void MetadataImport_XmlDocumentation_TemplateParameters()
        {
            PopulateTemplateTree();

            MbUnitTemplate template = (MbUnitTemplate)GetDescendantByName(rootTemplate, typeof(ParameterizedTest).Name);
            MbUnitTemplateParameter fieldParameter = (MbUnitTemplateParameter) template.GetParameterByName("FieldParameter");
            MbUnitTemplateParameter propertyParameter = (MbUnitTemplateParameter) template.GetParameterByName("PropertyParameter");

            Assert.AreEqual("<summary>\nA field parameter.\n</summary>", fieldParameter.Metadata.GetValue(MetadataKeys.XmlDocumentation));
            Assert.AreEqual("<summary>\nA property parameter.\n</summary>", propertyParameter.Metadata.GetValue(MetadataKeys.XmlDocumentation));
        }

        [Test]
        public void MetadataImport_AssemblyAttributes()
        {
            PopulateTemplateTree();

            MbUnitFrameworkTemplate frameworkTemplate = (MbUnitFrameworkTemplate)rootTemplate.Children[0];
            MbUnitAssemblyTemplate assemblyTemplate = frameworkTemplate.AssemblyTemplates[0];

            Assert.AreEqual("MbUnit", assemblyTemplate.Metadata.GetValue(MetadataKeys.Company));
            Assert.AreEqual("Test", assemblyTemplate.Metadata.GetValue(MetadataKeys.Configuration));
            StringAssert.Contains(assemblyTemplate.Metadata.GetValue(MetadataKeys.Copyright), "MbUnit Project");
            Assert.AreEqual("A sample test assembly.", assemblyTemplate.Metadata.GetValue(MetadataKeys.Description));
            Assert.AreEqual("MbUnit.TestResources.Gallio", assemblyTemplate.Metadata.GetValue(MetadataKeys.Product));
            Assert.AreEqual("MbUnit.TestResources.Gallio", assemblyTemplate.Metadata.GetValue(MetadataKeys.Title));
            Assert.AreEqual("MbUnit", assemblyTemplate.Metadata.GetValue(MetadataKeys.Trademark));

            StringAssert.IsNonEmpty(assemblyTemplate.Metadata.GetValue(MetadataKeys.InformationalVersion));
            StringAssert.IsNonEmpty(assemblyTemplate.Metadata.GetValue(MetadataKeys.FileVersion));
            StringAssert.IsNonEmpty(assemblyTemplate.Metadata.GetValue(MetadataKeys.Version));
        }
    }
}
