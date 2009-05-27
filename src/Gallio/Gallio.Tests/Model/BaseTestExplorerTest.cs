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
using Gallio.Model;
using Gallio.Model.Execution;
using Gallio.Model.Messages;
using Gallio.Common.Reflection;
using Gallio.Runner.Harness;
using Gallio.Runtime;
using Gallio.Runtime.Extensibility;
using Gallio.Runtime.Loader;
using Gallio.Runtime.ProgressMonitoring;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.Tests.Model
{
    public abstract class BaseTestExplorerTest<TSampleFixture>
    {
        protected Assembly sampleAssembly;
        protected ComponentHandle<ITestFramework, TestFrameworkTraits> frameworkHandle;
        protected TestModel testModel;
        private ITestHarness harness;
        private string adapterAssemblyName;
        private string testResourcesNamespace;

        private Assembly GetSampleAssembly()
        {
            return typeof(TSampleFixture).Assembly;
        }

        protected abstract ComponentHandle<ITestFramework, TestFrameworkTraits> GetFrameworkHandle();

        protected virtual string FrameworkKind
        {
            get { return TestKinds.Framework; }
        }

        protected virtual string PassTestName
        {
            get { return "Pass"; }
        }

        protected virtual string FailTestName
        {
            get { return "Fail"; }
        }

        [SetUp]
        public void SetUp()
        {
            sampleAssembly = GetSampleAssembly();

            frameworkHandle = GetFrameworkHandle();
            DefaultTestFrameworkManager frameworkManager = new DefaultTestFrameworkManager(
                new[] { frameworkHandle });

            harness = new DefaultTestHarness(TestContextTrackerAccessor.Instance,
                RuntimeAccessor.ServiceLocator.Resolve<ILoader>(), frameworkManager);

            adapterAssemblyName = frameworkHandle.GetComponent().GetType().Assembly.GetName().Name;
            testResourcesNamespace = sampleAssembly.GetName().Name;
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

        [Test]
        public void PopulateTestTree_WhenAssemblyDoesNotReferenceFramework_IsEmpty()
        {
            sampleAssembly = typeof(int).Assembly;
            PopulateTestTree();

            Assert.AreEqual(0, testModel.RootTest.Children.Count);
        }

        [Test]
        public void PopulateTestTree_CapturesTestStructureAndBasicMetadata()
        {
            PopulateTestTree();

            RootTest rootTest = testModel.RootTest;
            Assert.IsNull(rootTest.Parent);
            Assert.AreEqual(TestKinds.Root, rootTest.Kind);
            Assert.IsNull(rootTest.CodeElement);
            Assert.IsFalse(rootTest.IsTestCase);
            Assert.AreEqual(1, rootTest.Children.Count);

            BaseTest frameworkTest = (BaseTest)rootTest.Children[0];
            Assert.AreSame(testModel.RootTest, frameworkTest.Parent);
            Assert.AreEqual(FrameworkKind, frameworkTest.Kind);
            Assert.AreEqual(frameworkHandle.GetTraits().Name, frameworkTest.Metadata.GetValue(MetadataKeys.Framework));
            Assert.IsNull(frameworkTest.CodeElement);
            //Assert.AreEqual("MSTest v9.0.0.0", frameworkTest.Name);
            //Assert.AreEqual("MbUnit v" + expectedVersion, frameworkTest.Name);
            //Assert.AreEqual("xUnit.net v" + expectedVersion, frameworkTest.Name);
            //Assert.AreEqual("NUnit v" + expectedVersion, frameworkTest.Name);
            //Assert.AreEqual("MbUnit v" + expectedVersion, frameworkTest.Name);
            Assert.IsFalse(frameworkTest.IsTestCase);
            Assert.AreEqual(1, frameworkTest.Children.Count);

            BaseTest assemblyTest = (BaseTest)frameworkTest.Children[0];
            Assert.AreSame(frameworkTest, assemblyTest.Parent);
            Assert.AreEqual(TestKinds.Assembly, assemblyTest.Kind);
            Assert.AreEqual(CodeReference.CreateFromAssembly(sampleAssembly), assemblyTest.CodeElement.CodeReference);
            Assert.AreEqual(sampleAssembly.GetName().Name, assemblyTest.Name);
            Assert.IsFalse(assemblyTest.IsTestCase);
            Assert.GreaterThanOrEqualTo(assemblyTest.Children.Count, 2);

            BaseTest fixtureTest = (BaseTest)GetDescendantByName(assemblyTest, "SimpleTest");
            Assert.AreSame(assemblyTest, fixtureTest.Parent);
            Assert.AreEqual(TestKinds.Fixture, fixtureTest.Kind);
            Assert.AreEqual(new CodeReference(sampleAssembly.FullName, testResourcesNamespace, testResourcesNamespace + ".SimpleTest", null, null),
                fixtureTest.CodeElement.CodeReference);
            Assert.AreEqual("SimpleTest", fixtureTest.Name);
            Assert.IsFalse(fixtureTest.IsTestCase);
            Assert.AreEqual(2, fixtureTest.Children.Count);

            BaseTest passTest = (BaseTest)GetDescendantByName(fixtureTest, PassTestName);
            BaseTest failTest = (BaseTest)GetDescendantByName(fixtureTest, FailTestName);

            Assert.IsNotNull(passTest, "Cannot find test case '{0}'", PassTestName);
            Assert.IsNotNull(failTest, "Cannot find test case '{0}'", FailTestName);

            Assert.AreSame(fixtureTest, passTest.Parent);
            Assert.AreEqual(TestKinds.Test, passTest.Kind);
            Assert.AreEqual(new CodeReference(sampleAssembly.FullName, testResourcesNamespace, testResourcesNamespace + ".SimpleTest", "Pass", null),
                passTest.CodeElement.CodeReference);
            Assert.AreEqual(PassTestName, passTest.Name);
            Assert.IsTrue(passTest.IsTestCase);
            Assert.AreEqual(0, passTest.Children.Count);

            Assert.AreSame(fixtureTest, failTest.Parent);
            Assert.AreEqual(TestKinds.Test, failTest.Kind);
            Assert.AreEqual(new CodeReference(sampleAssembly.FullName, testResourcesNamespace, testResourcesNamespace + ".SimpleTest", "Fail", null),
                failTest.CodeElement.CodeReference);
            Assert.AreEqual(FailTestName, failTest.Name);
            Assert.IsTrue(failTest.IsTestCase);
            Assert.AreEqual(0, failTest.Children.Count);
        }

        [Test]
        public void MetadataImport_XmlDocumentation()
        {
            PopulateTestTree();

            BaseTest test = (BaseTest)GetDescendantByName(testModel.RootTest, typeof(TSampleFixture).Name);
            BaseTest passTest = (BaseTest)GetDescendantByName(test, PassTestName);
            BaseTest failTest = (BaseTest)GetDescendantByName(test, FailTestName);

            Assert.AreEqual("<summary>\nA simple test fixture.\n</summary>", test.Metadata.GetValue(MetadataKeys.XmlDocumentation));
            Assert.AreEqual("<summary>\nA passing test.\n</summary>", passTest.Metadata.GetValue(MetadataKeys.XmlDocumentation));
            Assert.AreEqual("<summary>\nA failing test.\n</summary>", failTest.Metadata.GetValue(MetadataKeys.XmlDocumentation));
        }

        [Test]
        public void MetadataImport_AssemblyAttributes()
        {
            PopulateTestTree();

            ITest frameworkTest = testModel.RootTest.Children[0];
            ITest assemblyTest = frameworkTest.Children[0];

            Assert.AreEqual("MbUnit Project", assemblyTest.Metadata.GetValue(MetadataKeys.Company));
            Assert.AreEqual("Test", assemblyTest.Metadata.GetValue(MetadataKeys.Configuration));
            Assert.Contains(assemblyTest.Metadata.GetValue(MetadataKeys.Copyright), "Gallio Project");
            Assert.AreEqual("A sample test assembly for " + frameworkHandle.GetTraits().Name + ".", assemblyTest.Metadata.GetValue(MetadataKeys.Description));
            Assert.AreEqual("Gallio", assemblyTest.Metadata.GetValue(MetadataKeys.Product));
            Assert.AreEqual(testResourcesNamespace, assemblyTest.Metadata.GetValue(MetadataKeys.Title));
            Assert.AreEqual("Gallio", assemblyTest.Metadata.GetValue(MetadataKeys.Trademark));

            Assert.AreEqual("1.2.3.4", assemblyTest.Metadata.GetValue(MetadataKeys.InformationalVersion));
            Assert.IsNotEmpty(assemblyTest.Metadata.GetValue(MetadataKeys.FileVersion));
            Assert.IsNotEmpty(assemblyTest.Metadata.GetValue(MetadataKeys.Version));
        }

        protected void PopulateTestTree()
        {
            TestPackageConfig config = new TestPackageConfig();
            config.AssemblyFiles.Add(AssemblyUtils.GetFriendlyAssemblyCodeBase(sampleAssembly));

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
    }
}
