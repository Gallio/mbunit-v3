// Copyright 2005-2009 Gallio Project - http://www.gallio.org/
// Portions Copyright 2000-2004 Jonathan de Halleux
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
using Gallio.Model.Messages;
using Gallio.Runtime.Extensibility;
using Gallio.Runtime.FileTypes;
using Gallio.Runtime.Loader;
using Gallio.Runtime;
using Gallio.Framework;
using Gallio.Common.Reflection;
using Gallio.Model;
using MbUnit.Framework;
using Gallio.Runner.Harness;
using Gallio.Model.Execution;
using Gallio.Runtime.ProgressMonitoring;
using Rhino.Mocks;

namespace Gallio.Tests.Model
{
    public abstract class BaseTestFrameworkTest
    {
        protected Assembly sampleAssembly;
        protected ComponentHandle<ITestFramework, TestFrameworkTraits> frameworkHandle;
        protected TestModel testModel;

        private ITestHarness harness;

        protected abstract Assembly GetSampleAssembly();

        protected abstract ComponentHandle<ITestFramework, TestFrameworkTraits> GetFrameworkHandle();

        [SetUp]
        public void SetUp()
        {
            sampleAssembly = GetSampleAssembly();

            frameworkHandle = GetFrameworkHandle();
            DefaultTestFrameworkManager frameworkManager = new DefaultTestFrameworkManager(
                new[] { frameworkHandle }, RuntimeAccessor.ServiceLocator.Resolve<IFileTypeManager>());
            ITestEnvironmentManager environmentManager = RuntimeAccessor.ServiceLocator.Resolve<ITestEnvironmentManager>();

            harness = new DefaultTestHarness(TestContextTrackerAccessor.Instance,
                RuntimeAccessor.ServiceLocator.Resolve<ILoader>(),
                environmentManager, frameworkManager);
        }

        [TearDown]
        public void TearDown()
        {
            if (harness != null)
            {
                harness.Dispose();
                harness = null;
                frameworkHandle = null;
                sampleAssembly = null;
            }
        }

        protected void PopulateTestTree()
        {
            TestPackageConfig config = new TestPackageConfig();
            config.Files.Add(AssemblyUtils.GetFriendlyAssemblyCodeBase(sampleAssembly));

            harness.Load(config, NullProgressMonitor.CreateInstance());
            harness.Explore(new TestExplorationOptions(),
                MockRepository.GenerateStub<ITestExplorationListener>(),
                NullProgressMonitor.CreateInstance());

            testModel = harness.TestModel;
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

        protected ITestParameter GetParameterByName(ITest test, string name)
        {
            foreach (ITestParameter testParameter in test.Parameters)
            {
                if (testParameter.Name == name)
                    return testParameter;
            }

            return null;
        }
    }
}