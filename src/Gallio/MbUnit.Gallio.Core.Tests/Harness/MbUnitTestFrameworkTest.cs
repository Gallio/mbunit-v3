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
using MbUnit.Core.Harness;
using MbUnit.Core.Runtime;
using MbUnit.Framework.Kernel.Events;
using MbUnit.Framework.Kernel.Model;
using MbUnit.Framework.Kernel.Metadata;
using MbUnit.Framework.Kernel.Runtime;
using MbUnit.Framework.Kernel.Utilities;
using MbUnit._Framework.Tests;
using MbUnit.Framework.Tests.Kernel.Runtime;
using MbUnit.TestResources.Gallio;
using MbUnit2::MbUnit.Framework;

using System;
using System.Reflection;

namespace MbUnit.Core.Tests.Harness
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
    }
}
