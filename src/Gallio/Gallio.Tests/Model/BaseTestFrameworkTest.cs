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

using System.Reflection;
using Gallio.Contexts;
using Gallio.Model.Reflection;
using MbUnit.Framework;
using Gallio.Runner;
using Gallio.Runner.Harness;
using Gallio.Hosting;
using Gallio.Model.Execution;
using Gallio.Core.ProgressMonitoring;
using Gallio.Model;

namespace Gallio.Tests.Model
{
    public abstract class BaseTestFrameworkTest
    {
        protected Assembly sampleAssembly;
        protected ITestFramework framework;
        protected RootTemplate rootTemplate;
        protected RootTest rootTest;

        private ITestHarness harness;

        protected abstract Assembly GetSampleAssembly();

        protected abstract ITestFramework CreateFramework();

        [SetUp]
        public void SetUp()
        {
            sampleAssembly = GetSampleAssembly();

            harness = new DefaultTestHarness(new DependencyTestPlanFactory(Context.ContextManager));

            framework = CreateFramework();
            harness.AddTestFramework(framework);
        }

        [TearDown]
        public void TearDown()
        {
            if (harness != null)
            {
                harness.Dispose();
                harness = null;
                framework = null;
                sampleAssembly = null;
            }
        }

        protected void PopulateTemplateTree()
        {
            harness.LoadPackage(new TestPackage(), new NullProgressMonitor());
            harness.AddAssembly(Reflector.Wrap(sampleAssembly));
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