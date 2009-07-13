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
using System.Reflection;
using Gallio.Common.Policies;
using Gallio.Framework.Pattern;
using Gallio.Common.Reflection;
using Gallio.Model.Tree;
using Gallio.Runtime;
using Gallio.Runtime.Extensibility;
using MbUnit.Core;
using MbUnit.Framework;
using Gallio.Model;
using MbUnit.TestResources;
using Gallio.Tests.Model;
using Test = Gallio.Model.Tree.Test;

namespace MbUnit.Tests.Model
{
    [TestFixture]
    [TestsOn(typeof(MbUnitTestFramework))]
    [Author("Jeff", "jeff@ingenio.com")]
    public class MbUnitTestFrameworkTest : BaseTestFrameworkTest<SimpleTest>
    {
        protected override ComponentHandle<ITestFramework, TestFrameworkTraits> FrameworkHandle
        {
            get
            {
                return (ComponentHandle<ITestFramework, TestFrameworkTraits>)
                    RuntimeAccessor.ServiceLocator.ResolveHandleByComponentId("MbUnit.TestFramework");
            }
        }

        protected override string FrameworkKind
        {
            get { return "MbUnit v3 Framework"; }
        }

        /// <summary>
        /// This is really just a quick sanity check to be sure that the framework
        /// seems to produce a sensible test tree.  More detailed checks on how particular
        /// attributes are handled belong elsewhere.
        /// </summary>
        [Test]
        public void PopulateTestTree_WhenAssemblyReferencesMbUnit_ContainsSimpleTest()
        {
            TestModel testModel = PopulateTestTree();

            Version expectedVersion = VersionPolicy.GetVersionNumber(typeof(Assert).Assembly);

            Test rootTest = testModel.RootTest;
            Assert.IsNull(rootTest.Parent);
            Assert.AreEqual(TestKinds.Root, rootTest.Kind);
            Assert.IsNull(rootTest.CodeElement);
            Assert.AreEqual(1, rootTest.Children.Count);

            Test frameworkTest = rootTest.Children[0];
            Assert.AreSame(rootTest, frameworkTest.Parent);
            Assert.AreEqual("MbUnit v3 Framework", frameworkTest.Kind);
            Assert.AreEqual(FrameworkHandle.GetTraits().Name, frameworkTest.Metadata.GetValue(MetadataKeys.Framework));
            Assert.IsNull(frameworkTest.CodeElement);
            Assert.AreEqual("MbUnit v" + expectedVersion, frameworkTest.Name);
            Assert.AreEqual(1, frameworkTest.Children.Count);

            Test assemblyTest = frameworkTest.Children[0];
            Assert.AreSame(frameworkTest, assemblyTest.Parent);
            Assert.AreEqual(TestKinds.Assembly, assemblyTest.Kind);
            Assert.AreEqual(CodeReference.CreateFromAssembly(SimpleFixtureAssembly), assemblyTest.CodeElement.CodeReference);
            Assert.AreEqual(SimpleFixtureAssembly, ((IAssemblyInfo)assemblyTest.CodeElement).Resolve(true));
            Assert.GreaterThanOrEqualTo(assemblyTest.Children.Count, 1);

            Test typeTest = GetDescendantByName(assemblyTest, "SimpleTest");
            Assert.IsNotNull(typeTest, "Could not find the SimpleTest fixture.");
            Assert.AreSame(assemblyTest, typeTest.Parent);
            Assert.AreEqual(TestKinds.Fixture, typeTest.Kind);
            Assert.AreEqual(CodeReference.CreateFromType(typeof(SimpleTest)), typeTest.CodeElement.CodeReference);
            Assert.AreEqual(typeof(SimpleTest), ((ITypeInfo) typeTest.CodeElement).Resolve(true));
            Assert.AreEqual("SimpleTest", typeTest.Name);
            Assert.AreEqual(2, typeTest.Children.Count);

            Test passTest = GetDescendantByName(typeTest, "Pass");
            Assert.IsNotNull(passTest, "Could not find the Pass test.");
            Assert.AreSame(typeTest, passTest.Parent);
            Assert.AreEqual(TestKinds.Test, passTest.Kind);
            Assert.AreEqual(CodeReference.CreateFromMember(typeof(SimpleTest).GetMethod("Pass")), passTest.CodeElement.CodeReference);
            Assert.AreEqual(typeof(SimpleTest).GetMethod("Pass"), ((IMethodInfo) passTest.CodeElement).Resolve(true));
            Assert.AreEqual("Pass", passTest.Name);

            Test failTest = GetDescendantByName(typeTest, "Fail");
            Assert.IsNotNull(failTest, "Could not find the Fail test.");
            Assert.AreSame(typeTest, failTest.Parent);
            Assert.AreEqual(TestKinds.Test, failTest.Kind);
            Assert.AreEqual(CodeReference.CreateFromMember(typeof(SimpleTest).GetMethod("Fail")), failTest.CodeElement.CodeReference);
            Assert.AreEqual(typeof(SimpleTest).GetMethod("Fail"), ((IMethodInfo)failTest.CodeElement).Resolve(true));
            Assert.AreEqual("Fail", failTest.Name);
        }

        [Test]
        public void MetadataImport_XmlDocumentation_TestParameters()
        {
            TestModel testModel = PopulateTestTree();

            Test test = GetDescendantByName(testModel.RootTest, typeof(ParameterizedTest).Name);
            TestParameter fieldParameter = GetParameterByName(test, "FieldParameter");
            TestParameter propertyParameter = GetParameterByName(test, "PropertyParameter");

            Assert.AreEqual("<summary>\nA field parameter.\n</summary>", fieldParameter.Metadata.GetValue(MetadataKeys.XmlDocumentation));
            Assert.AreEqual("<summary>\nA property parameter.\n</summary>", propertyParameter.Metadata.GetValue(MetadataKeys.XmlDocumentation));
        }        
    }
}
