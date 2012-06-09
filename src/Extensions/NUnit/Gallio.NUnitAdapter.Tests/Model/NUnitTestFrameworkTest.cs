// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
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

using Gallio.Model.Tree;
using Gallio.NUnitAdapter.Model;
using Gallio.Common.Reflection;
using Gallio.Runtime;
using Gallio.Runtime.Extensibility;
using MbUnit.Framework;
using Gallio.Model;
using Gallio.NUnitAdapter.TestResources;
using Gallio.NUnitAdapter.TestResources.Metadata;
using Gallio.Tests.Model;
using Test = Gallio.Model.Tree.Test;

namespace Gallio.NUnitAdapter.Tests.Model
{
    [TestFixture]
    [TestsOn(typeof(NUnitTestFramework))]
    [Author("Jeff", "jeff@ingenio.com")]
    public class NUnitTestFrameworkTest : BaseTestFrameworkTest<SimpleTest>
    {
        protected override ComponentHandle<ITestFramework, TestFrameworkTraits> TestFrameworkHandle
        {
            get
            {
                return (ComponentHandle<ITestFramework, TestFrameworkTraits>)
#if NUNIT248
                RuntimeAccessor.ServiceLocator.ResolveHandleByComponentId("NUnitAdapter248.TestFramework");
#elif NUNIT253
                RuntimeAccessor.ServiceLocator.ResolveHandleByComponentId("NUnitAdapter253.TestFramework");
#elif NUNIT254TO10
                RuntimeAccessor.ServiceLocator.ResolveHandleByComponentId("NUnitAdapter254-10.TestFramework");
#elif NUNITLATEST
                RuntimeAccessor.ServiceLocator.ResolveHandleByComponentId("NUnitAdapterLatest.TestFramework");
#else
#error "Unrecognized NUnit framework version."
#endif
            }
        }

        protected override string AssemblyKind
        {
            get { return NUnitTestExplorer.AssemblyKind; }
        }
        
        [Test]
        public void MetadataImport_Description()
        {
            TestModel testModel = PopulateTestTree();

            Test test = GetDescendantByName(testModel.RootTest, typeof(DescriptionSample).Name);
            Assert.AreEqual("A sample description.", test.Metadata.GetValue(MetadataKeys.Description));
        }

        [Test]
        public void MetadataImport_Category()
        {
            TestModel testModel = PopulateTestTree();

            Test test = GetDescendantByName(testModel.RootTest, typeof(CategorySample).Name);
            Assert.AreEqual("samples", test.Metadata.GetValue(MetadataKeys.Category));
        }

        [Test]
        public void MetadataImport_IgnoreReason()
        {
            TestModel testModel = PopulateTestTree();

            Test fixture = GetDescendantByName(testModel.RootTest, typeof(IgnoreReasonSample).Name);
            Test test = fixture.Children[0];
            Assert.AreEqual("For testing purposes.", test.Metadata.GetValue(MetadataKeys.IgnoreReason));
        }

        [Test]
        public void MetadataImport_Property()
        {
            TestModel testModel = PopulateTestTree();

            Test test = GetDescendantByName(testModel.RootTest, typeof(PropertySample).Name);
            Assert.AreEqual("customvalue-1", test.Metadata.GetValue("customkey-1"));
            Assert.AreEqual("customvalue-2", test.Metadata.GetValue("customkey-2"));
        }
        
        [Test]
        public void DoesNotChokeOnAmbiguousMatch()
        {
            TestModel testModel = PopulateTestTree();

            Test fixture = GetDescendantByName(testModel.RootTest, typeof(AmbiguousMatchSample).Name);
            Assert.Count(1, fixture.Children);

            Test test =  fixture.Children[0];
            Assert.AreEqual("Test", test.Name);
            Assert.IsNull(test.CodeElement);
        }

#if NUNITLATEST
        [Test, Description(" Issue 677: NUnit TestCases and Theories display an incorrect hierarchy")]
        public void ParameterizedTestHasMethodAsCodeElement() 
        {
            TestModel testModel = PopulateTestTree();

            Test fixture = GetDescendantByName(testModel.RootTest, typeof(ParameterizedTest).Name);
            Assert.Count(1, fixture.Children);

            Test test = fixture.Children[0];
            Assert.AreEqual("Test", test.Name);
            Assert.AreEqual(CodeElementKind.Method, test.CodeElement.Kind);
            Assert.AreEqual("Test", test.CodeElement.Name);
        }

        [Test, Description(" Issue 801: NUnit Tests are nested inside their namespaces twice on the test tree, don't honour 'Flat'")]
        public void NamespacesHaveTestKindNamespace()
        {
            TestModel testModel = PopulateTestTree();

            Test assembly = testModel.RootTest.Children[0];
            Test namespaceTest = assembly.Children[0];

            Assert.AreEqual("Gallio", namespaceTest.Name);
            Assert.AreEqual(TestKinds.Namespace, namespaceTest.Kind);
        }
#endif
    }
}
