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
using Castle.Core.Logging;
using MbUnit.Core.Model;
using MbUnit.Core.ProgressMonitoring;
using MbUnit.Core.Runtime;
using MbUnit.Framework.Kernel.Runtime;
using MbUnit2::MbUnit.Framework;

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using MbUnit._Framework.Tests;
using MbUnit.Core.Harness;
using MbUnit.Framework.Kernel.Model;
using MbUnit.Framework.Tests.Kernel.Runtime;

namespace MbUnit.Core.Tests.Model
{
    public abstract class BaseTestFrameworkTest : BaseUnitTest
    {
        protected Assembly sampleAssembly;
        protected ITestFramework framework;
        protected RootTemplate rootTemplate;
        protected RootTest rootTest;

        private MockRuntime mockRuntime;
        private IAssemblyResolverManager mockAssemblyResolverManager;
        private ITestHarness harness;

        protected abstract Assembly GetSampleAssembly();

        protected abstract ITestFramework CreateFramework();

        public override void SetUp()
        {
            base.SetUp();

            sampleAssembly = GetSampleAssembly();

            mockRuntime = new MockRuntime();
            mockAssemblyResolverManager = Mocks.CreateMock<IAssemblyResolverManager>();
            mockRuntime.Components.Add(typeof(IAssemblyResolverManager), mockAssemblyResolverManager);

            DefaultXmlDocumentationResolver xmlDocumentationResolver = new DefaultXmlDocumentationResolver();
            xmlDocumentationResolver.Logger = new ConsoleLogger();
            mockRuntime.Components.Add(typeof(IXmlDocumentationResolver), xmlDocumentationResolver);

            DefaultContextManager contextManager = new DefaultContextManager();
            mockRuntime.Components.Add(typeof(IContextManager), contextManager);

            DependencyTestPlanFactory testPlanFactory = new DependencyTestPlanFactory(contextManager);

            MbUnit.Framework.Runtime.Instance = mockRuntime;

            harness = new DefaultTestHarness(mockRuntime, testPlanFactory);

            framework = CreateFramework();
            harness.AddContributor(framework);
        }

        public override void TearDown()
        {
            base.TearDown();

            MbUnit.Framework.Runtime.Instance = null;
        }

        protected void PopulateTemplateTree()
        {
            harness.LoadPackage(new TestPackage(), new NullProgressMonitor());
            harness.AddAssembly(sampleAssembly);
            harness.BuildTemplates(new TemplateEnumerationOptions(), new NullProgressMonitor());

            rootTemplate = harness.TemplateTreeBuilder.Root;
        }

        protected void PopulateTestTree()
        {
            PopulateTemplateTree();
            harness.BuildTests(new TestEnumerationOptions(), new NullProgressMonitor());

            rootTest = harness.TestTreeBuilder.Root;
        }

        protected ITemplate GetDescendantByName(ITemplate parent, string name)
        {
            foreach (ITemplate test in parent.Children)
            {
                if (test.Name == name)
                    return test;

                ITemplate descendant = GetDescendantByName(test, name);
                if (descendant != null)
                    return descendant;
            }

            return null;
        }

        protected ITest GetDescendantByName(ITest parent, string name)
        {
            foreach (ITest test in parent.Children)
            {
                if (test.Name == name)
                    return test;

                ITest descendant = GetDescendantByName(test, name);
                if (descendant != null)
                    return descendant;
            }

            return null;
        }
    }
}