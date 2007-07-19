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
using MbUnit._Framework.Tests;
using MbUnit.Core.Harness;
using MbUnit.Framework.Kernel.Harness;
using MbUnit.Framework.Services.Runtime;
using MbUnit.Framework.Tests.Kernel.Runtime;
using MbUnit2::MbUnit.Framework;

using System.Reflection;
using MbUnit.Framework.Kernel.Metadata;
using MbUnit.Framework.Kernel.Model;
using MbUnit.Plugin.NUnitAdapter.Core;

using System;

namespace MbUnit.Plugin.NUnitAdapter.Tests.Core
{
    [TestFixture]
    [TestsOn(typeof(NUnitTestFramework))]
    [Author("Jeff", "jeff@ingenio.com")]
    public class NUnitTestFrameworkTest : BaseUnitTest
    {
        private NUnitTestFramework framework;
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

            framework = new NUnitTestFramework();
            mockHarness.AddContributor(framework);
        }

        [Test]
        public void NameIsNUnit()
        {
            Assert.AreEqual("NUnit", framework.Name);
        }

        [Test]
        public void PopulateTemplateTree_WhenAssemblyDoesNotReferenceMbUnit_IsEmpty()
        {
            PopulateTemplateTree(typeof(Int32).Assembly);

            Assert.AreEqual(0, builder.Root.Children.Count);
        }

        [Test]
        public void PopulateTemplateTree_WhenAssemblyReferencesNUnit_ContainsJustTheFrameworkTemplate()
        {
            Type fixtureType = typeof(MbUnit.TestResources.NUnit.SimpleTest);
            Assembly assembly = fixtureType.Assembly;
            Version expectedVersion = typeof(NUnit.Framework.Assert).Assembly.GetName().Version;

            PopulateTemplateTree(assembly);
            Assert.IsNull(builder.Root.Parent);
            Assert.AreEqual(ComponentKind.Root, builder.Root.Kind);
            Assert.AreEqual(CodeReference.Unknown, builder.Root.CodeReference);
            Assert.AreEqual(1, builder.Root.Children.Count);

            BaseTemplate frameworkTemplate = (BaseTemplate)builder.Root.Children[0];
            Assert.AreSame(builder.Root, frameworkTemplate.Parent);
            Assert.AreEqual(ComponentKind.Framework, frameworkTemplate.Kind);
            Assert.AreEqual(CodeReference.Unknown, frameworkTemplate.CodeReference);
            Assert.AreEqual("NUnit v" + expectedVersion, frameworkTemplate.Name);
            Assert.AreEqual(1, frameworkTemplate.Children.Count);

            BaseTemplate assemblyTemplate = (BaseTemplate)frameworkTemplate.Children[0];
            Assert.AreSame(frameworkTemplate, assemblyTemplate.Parent);
            Assert.AreEqual(ComponentKind.Assembly, assemblyTemplate.Kind);
            Assert.AreEqual(CodeReference.CreateFromAssembly(assembly), assemblyTemplate.CodeReference);
            Assert.AreEqual(0, assemblyTemplate.Children.Count);
        }

        private void PopulateTemplateTree(Assembly assembly)
        {
            mockHarness.AddAssembly(assembly);
            mockHarness.Initialize();
            mockHarness.BuildTemplates();

            builder = mockHarness.TemplateTreeBuilder;
        }
    }
}
