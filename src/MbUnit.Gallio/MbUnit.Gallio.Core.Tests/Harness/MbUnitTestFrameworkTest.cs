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
using MbUnit.Core.Services.Runtime;
using MbUnit.Framework.Kernel.Model;
using MbUnit.Framework.Kernel.Metadata;
using MbUnit.Framework.Services.Runtime;
using MbUnit.Framework.Kernel.Utilities;
using MbUnit._Framework.Tests;
using MbUnit.Framework.Tests.Kernel.Runtime;
using MbUnit2::MbUnit.Framework;

using System;
using System.Reflection;

namespace MbUnit.Core.Tests.Harness
{
    [TestFixture]
    [TestsOn(typeof(MbUnitTestFramework))]
    [Author("Jeff", "jeff@ingenio.com")]
    public class MbUnitTestFrameworkTest : BaseUnitTest
    {
        private MbUnitTestFramework framework;
        private TemplateTreeBuilder builder;

        private MockRuntime mockRuntime;
        private IAssemblyResolverManager mockAssemblyResolverManager;
        private ITestHarness mockHarness;

        public override void SetUp()
        {
            base.SetUp();

            mockRuntime = new MockRuntime();
            mockAssemblyResolverManager = Mocks.CreateMock<IAssemblyResolverManager>();
            mockRuntime.Components.Add(typeof(IAssemblyResolverManager), mockAssemblyResolverManager);
            mockHarness = new DefaultTestHarness(mockRuntime);

            framework = new MbUnitTestFramework();
            mockHarness.AddContributor(framework);
        }

        [Test]
        public void NameIsMbUnitGallio()
        {
            Assert.AreEqual("MbUnit Gallio", framework.Name);
        }

        [Test]
        public void PopulateTemplateTree_WhenAssemblyDoesNotReferenceMbUnitGallio_IsEmpty()
        {
            PopulateTemplateTree(typeof(Int32).Assembly);

            Assert.AreEqual(0, builder.Root.Children.Count);
        }

        /// <summary>
        /// This is really just a quick sanity check to be sure that the framework
        /// seems to produce a sensible test tree.  More detailed checks on how particular
        /// attributes are handled belong elsewhere.
        /// </summary>
        [Test]
        public void PopulateTemplateTree_WhenAssemblyReferencesMbUnitGallio_ContainsSimpleTest()
        {
            Type fixtureType = typeof(MbUnit.TestResources.Gallio.SimpleTest);
            Assembly assembly = fixtureType.Assembly;
            Version expectedVersion = typeof(MbUnitTestFramework).Assembly.GetName().Version;

            PopulateTemplateTree(assembly);
            Assert.IsNull(builder.Root.Parent);
            Assert.AreEqual(ComponentKind.Root, builder.Root.Kind);
            Assert.AreEqual(CodeReference.Unknown, builder.Root.CodeReference);
            Assert.AreEqual(1, builder.Root.Children.Count);

            MbUnitFrameworkTemplate frameworkTemplate = (MbUnitFrameworkTemplate) builder.Root.Children[0];
            Assert.AreSame(builder.Root, frameworkTemplate.Parent);
            Assert.AreEqual(ComponentKind.Framework, frameworkTemplate.Kind);
            Assert.AreEqual(CodeReference.Unknown, frameworkTemplate.CodeReference);
            Assert.AreEqual(expectedVersion, frameworkTemplate.Version);
            Assert.AreEqual("MbUnit Gallio v" + expectedVersion, frameworkTemplate.Name);
            Assert.AreEqual(1, frameworkTemplate.AssemblyTemplates.Count);

            MbUnitAssemblyTemplate assemblyTemplate = frameworkTemplate.AssemblyTemplates[0];
            Assert.AreSame(frameworkTemplate, assemblyTemplate.Parent);
            Assert.AreEqual(ComponentKind.Assembly, assemblyTemplate.Kind);
            Assert.AreEqual(CodeReference.CreateFromAssembly(assembly), assemblyTemplate.CodeReference);
            Assert.AreEqual(assembly, assemblyTemplate.Assembly);
            Assert.GreaterEqualThan(1, assemblyTemplate.FixtureTemplates.Count);

            MbUnitFixtureTemplate fixtureTemplate = ListUtils.Find(assemblyTemplate.FixtureTemplates, delegate(MbUnitFixtureTemplate template)
            {
                return template.FixtureType == fixtureType;
            });
            Assert.IsNotNull(fixtureTemplate, "Could not find the SimpleTest fixture.");
            Assert.AreSame(assemblyTemplate, fixtureTemplate.Parent);
            Assert.AreEqual(ComponentKind.Fixture, fixtureTemplate.Kind);
            Assert.AreEqual(CodeReference.CreateFromType(fixtureType), fixtureTemplate.CodeReference);
            Assert.AreEqual(fixtureType, fixtureTemplate.FixtureType);
            Assert.AreEqual("SimpleTest", fixtureTemplate.Name);
            Assert.AreEqual(2, fixtureTemplate.MethodTemplates.Count);

            MbUnitMethodTemplate passTemplate = ListUtils.Find(fixtureTemplate.MethodTemplates, delegate(MbUnitMethodTemplate template)
            {
                return template.Name == "Pass";
            });
            Assert.IsNotNull(passTemplate, "Could not find the Pass test.");
            Assert.AreSame(fixtureTemplate, passTemplate.Parent);
            Assert.AreEqual(ComponentKind.Test, passTemplate.Kind);
            Assert.AreEqual(CodeReference.CreateFromMember(fixtureType.GetMethod("Pass")), passTemplate.CodeReference);
            Assert.AreEqual(fixtureType.GetMethod("Pass"), passTemplate.Method);
            Assert.AreEqual("Pass", passTemplate.Name);

            MbUnitMethodTemplate failTemplate = ListUtils.Find(fixtureTemplate.MethodTemplates, delegate(MbUnitMethodTemplate template)
            {
                return template.Name == "Fail";
            });
            Assert.IsNotNull(failTemplate, "Could not find the Fail test.");
            Assert.AreSame(fixtureTemplate, failTemplate.Parent);
            Assert.AreEqual(ComponentKind.Test, failTemplate.Kind);
            Assert.AreEqual(CodeReference.CreateFromMember(fixtureType.GetMethod("Fail")), failTemplate.CodeReference);
            Assert.AreEqual(fixtureType.GetMethod("Fail"), failTemplate.Method);
            Assert.AreEqual("Fail", failTemplate.Name);
        }

        private void PopulateTemplateTree(Assembly assembly)
        {
            mockHarness.AddAssembly(assembly);
            mockHarness.Initialize();
            mockHarness.BuildTemplates(new TemplateEnumerationOptions());

            builder = mockHarness.TemplateTreeBuilder;
        }
    }
}
