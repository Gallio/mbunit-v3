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

using System;
using System.IO;
using System.Reflection;
using Gallio.Common.Messaging;
using Gallio.Model.Contexts;
using Gallio.Model.Isolation;
using Gallio.Model.Messages;
using Gallio.Model.Messages.Exploration;
using Gallio.Model.Schema;
using Gallio.Model.Tree;
using Gallio.Runtime.Extensibility;
using Gallio.Runtime.FileTypes;
using Gallio.Runtime.Hosting;
using Gallio.Runtime.Loader;
using Gallio.Runtime;
using Gallio.Framework;
using Gallio.Common.Reflection;
using Gallio.Model;
using Gallio.Runtime.Logging;
using MbUnit.Framework;
using Gallio.Model.Environments;
using Gallio.Runtime.ProgressMonitoring;
using Rhino.Mocks;
using Test=Gallio.Model.Tree.Test;

namespace Gallio.Tests.Model
{
    public abstract class BaseTestFrameworkTest<TSampleFixture>
    {
        protected Assembly SimpleFixtureAssembly
        {
            get { return SimpleFixtureType.Assembly; }
        }

        protected Type SimpleFixtureType
        {
            get { return typeof (TSampleFixture); }
        }

        protected string SimpleFixtureNamespace
        {
            get { return SimpleFixtureType.Namespace; }
        }

        protected abstract ComponentHandle<ITestFramework, TestFrameworkTraits> FrameworkHandle { get; }

        protected virtual string AssemblyKind
        {
            get { return TestKinds.Assembly; }
        }

        protected virtual string PassTestName
        {
            get { return "Pass"; }
        }

        protected virtual string FailTestName
        {
            get { return "Fail"; }
        }

        protected TestModel PopulateTestTree()
        {
            return PopulateTestTree(SimpleFixtureAssembly);
        }

        protected TestModel PopulateTestTree(Assembly assembly)
        {
            TestModel testModel = new TestModel();

            var testFrameworkManager = RuntimeAccessor.ServiceLocator.Resolve<ITestFrameworkManager>();
            var logger = new MarkupStreamLogger(TestLog.Default);
            ITestDriver testDriver = testFrameworkManager.GetTestDriver(frameworkId => frameworkId == FrameworkHandle.Id, logger);

            var testIsolationProvider = (ITestIsolationProvider) RuntimeAccessor.ServiceLocator.ResolveByComponentId("Gallio.LocalTestIsolationProvider");
            var testIsolationOptions = new TestIsolationOptions();
            using (ITestIsolationContext testIsolationContext = testIsolationProvider.CreateContext(testIsolationOptions, logger))
            {
                var testPackage = new TestPackage();
                testPackage.AddFile(new FileInfo(AssemblyUtils.GetFriendlyAssemblyCodeBase(assembly)));
                var testExplorationOptions = new TestExplorationOptions();

                var messageSink = TestModelSerializer.CreateMessageSinkToPopulateTestModel(testModel);

                new LogProgressMonitorProvider(logger).Run(progressMonitor =>
                {
                    testDriver.Explore(testIsolationContext, testPackage, testExplorationOptions,
                        messageSink, progressMonitor);
                });
            }

            return testModel;
        }

        protected Test GetDescendantByName(Test parent, string name)
        {
            foreach (Test test in parent.Children)
            {
                if (test.Name == name)
                    return test;

                Test descendant = GetDescendantByName(test, name);
                if (descendant != null)
                    return descendant;
            }

            return null;
        }

        protected TestParameter GetParameterByName(Test test, string name)
        {
            foreach (TestParameter testParameter in test.Parameters)
            {
                if (testParameter.Name == name)
                    return testParameter;
            }

            return null;
        }

        [Test]
        public void PopulateTestTree_WhenAssemblyDoesNotReferenceFramework_IsEmpty()
        {
            TestModel testModel = PopulateTestTree(typeof(int).Assembly);

            Assert.AreEqual(0, testModel.RootTest.Children.Count);
        }

        [Test]
        public void PopulateTestTree_CapturesTestStructureAndBasicMetadata()
        {
            TestModel testModel = PopulateTestTree();

            Test rootTest = testModel.RootTest;
            Assert.IsNull(rootTest.Parent);
            Assert.AreEqual(TestKinds.Root, rootTest.Kind);
            Assert.IsNull(rootTest.CodeElement);
            Assert.IsFalse(rootTest.IsTestCase);
            Assert.AreEqual(1, rootTest.Children.Count);

            Test assemblyTest = rootTest.Children[0];
            Assert.AreSame(rootTest, assemblyTest.Parent);
            Assert.AreEqual(AssemblyKind, assemblyTest.Kind);
            Assert.AreEqual(SimpleFixtureAssembly.Location, assemblyTest.Metadata.GetValue(MetadataKeys.File), StringComparison.OrdinalIgnoreCase);
            Assert.AreEqual(CodeReference.CreateFromAssembly(SimpleFixtureAssembly), assemblyTest.CodeElement.CodeReference);
            Assert.AreEqual(SimpleFixtureAssembly.GetName().Name, assemblyTest.Name);
            Assert.IsFalse(assemblyTest.IsTestCase);
            Assert.GreaterThanOrEqualTo(assemblyTest.Children.Count, 1);

            Test fixtureTest = GetDescendantByName(assemblyTest, "SimpleTest");
            Assert.AreEqual(TestKinds.Fixture, fixtureTest.Kind);
            Assert.AreEqual(new CodeReference(SimpleFixtureAssembly.FullName, SimpleFixtureNamespace, SimpleFixtureNamespace + ".SimpleTest", null, null),
                fixtureTest.CodeElement.CodeReference);
            Assert.AreEqual("SimpleTest", fixtureTest.Name);
            Assert.IsFalse(fixtureTest.IsTestCase);
            Assert.AreEqual(2, fixtureTest.Children.Count);

            Test passTest = GetDescendantByName(fixtureTest, PassTestName);
            Test failTest = GetDescendantByName(fixtureTest, FailTestName);

            Assert.IsNotNull(passTest, "Cannot find test case '{0}'", PassTestName);
            Assert.IsNotNull(failTest, "Cannot find test case '{0}'", FailTestName);

            Assert.AreSame(fixtureTest, passTest.Parent);
            Assert.AreEqual(TestKinds.Test, passTest.Kind);
            Assert.AreEqual(new CodeReference(SimpleFixtureAssembly.FullName, SimpleFixtureNamespace, SimpleFixtureNamespace + ".SimpleTest", "Pass", null),
                passTest.CodeElement.CodeReference);
            Assert.AreEqual(PassTestName, passTest.Name);
            Assert.IsTrue(passTest.IsTestCase);
            Assert.AreEqual(0, passTest.Children.Count);

            Assert.AreSame(fixtureTest, failTest.Parent);
            Assert.AreEqual(TestKinds.Test, failTest.Kind);
            Assert.AreEqual(new CodeReference(SimpleFixtureAssembly.FullName, SimpleFixtureNamespace, SimpleFixtureNamespace + ".SimpleTest", "Fail", null),
                failTest.CodeElement.CodeReference);
            Assert.AreEqual(FailTestName, failTest.Name);
            Assert.IsTrue(failTest.IsTestCase);
            Assert.AreEqual(0, failTest.Children.Count);
        }

        [Test]
        public void MetadataImport_XmlDocumentation()
        {
            TestModel testModel = PopulateTestTree();

            Test test = GetDescendantByName(testModel.RootTest, typeof(TSampleFixture).Name);
            Test passTest = GetDescendantByName(test, PassTestName);
            Test failTest = GetDescendantByName(test, FailTestName);

            Assert.AreEqual("<summary>\nA simple test fixture.\n</summary>", test.Metadata.GetValue(MetadataKeys.XmlDocumentation));
            Assert.AreEqual("<summary>\nA passing test.\n</summary>", passTest.Metadata.GetValue(MetadataKeys.XmlDocumentation));
            Assert.AreEqual("<summary>\nA failing test.\n</summary>", failTest.Metadata.GetValue(MetadataKeys.XmlDocumentation));
        }

        [Test]
        public void MetadataImport_AssemblyAttributes()
        {
            TestModel testModel = PopulateTestTree();

            Test assemblyTest = testModel.RootTest.Children[0];

            Assert.AreEqual("MbUnit Project", assemblyTest.Metadata.GetValue(MetadataKeys.Company));
            Assert.AreEqual("Test", assemblyTest.Metadata.GetValue(MetadataKeys.Configuration));
            Assert.Contains(assemblyTest.Metadata.GetValue(MetadataKeys.Copyright), "Gallio Project");
            Assert.AreEqual("A sample test assembly for " + FrameworkHandle.GetTraits().Name + ".", assemblyTest.Metadata.GetValue(MetadataKeys.Description));
            Assert.AreEqual("Gallio", assemblyTest.Metadata.GetValue(MetadataKeys.Product));
            Assert.AreEqual(SimpleFixtureAssembly.GetName().Name, assemblyTest.Metadata.GetValue(MetadataKeys.Title));
            Assert.AreEqual("Gallio", assemblyTest.Metadata.GetValue(MetadataKeys.Trademark));

            Assert.AreEqual("1.2.3.4", assemblyTest.Metadata.GetValue(MetadataKeys.InformationalVersion));
            Assert.IsNotEmpty(assemblyTest.Metadata.GetValue(MetadataKeys.FileVersion));
            Assert.IsNotEmpty(assemblyTest.Metadata.GetValue(MetadataKeys.Version));
        }
    }
}