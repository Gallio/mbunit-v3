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
using Gallio.NUnitAdapter.Model;
using Gallio.Common.Reflection;
using Gallio.Runtime;
using Gallio.Runtime.Extensibility;
using MbUnit.Framework;
using Gallio.Model;
using Gallio.NUnitAdapter.TestResources;
using Gallio.NUnitAdapter.TestResources.Metadata;
using Gallio.Tests.Model;

namespace Gallio.NUnitAdapter.Tests.Model
{
    [TestFixture]
    [TestsOn(typeof(NUnitTestFramework))]
    [Author("Jeff", "jeff@ingenio.com")]
    public class NUnitTestFrameworkTest : BaseTestFrameworkTest
    {
        protected override Assembly GetSampleAssembly()
        {
            return typeof(SimpleTest).Assembly;
        }

        protected override ComponentHandle<ITestFramework, TestFrameworkTraits> GetFrameworkHandle()
        {
            return (ComponentHandle<ITestFramework, TestFrameworkTraits>)
                RuntimeAccessor.ServiceLocator.ResolveHandleByComponentId("NUnitAdapter.TestFramework");
        }
        
        [Test]
        public void MetadataImport_Description()
        {
            PopulateTestTree();

            NUnitTest test = (NUnitTest)GetDescendantByName(testModel.RootTest, typeof(DescriptionSample).Name);
            Assert.AreEqual("A sample description.", test.Metadata.GetValue(MetadataKeys.Description));
        }

        [Test]
        public void MetadataImport_Category()
        {
            PopulateTestTree();

            NUnitTest test = (NUnitTest)GetDescendantByName(testModel.RootTest, typeof(CategorySample).Name);
            Assert.AreEqual("samples", test.Metadata.GetValue(MetadataKeys.Category));
        }

        [Test]
        public void MetadataImport_IgnoreReason()
        {
            PopulateTestTree();

            NUnitTest fixture = (NUnitTest)GetDescendantByName(testModel.RootTest, typeof(IgnoreReasonSample).Name);
            NUnitTest test = (NUnitTest)fixture.Children[0];
            Assert.AreEqual("For testing purposes.", test.Metadata.GetValue(MetadataKeys.IgnoreReason));
        }

        [Test]
        public void MetadataImport_Property()
        {
            PopulateTestTree();

            NUnitTest test = (NUnitTest)GetDescendantByName(testModel.RootTest, typeof(PropertySample).Name);
            Assert.AreEqual("customvalue-1", test.Metadata.GetValue("customkey-1"));
            Assert.AreEqual("customvalue-2", test.Metadata.GetValue("customkey-2"));
        }
        
        [Test]
        public void DoesNotChokeOnAmbiguousMatch()
        {
            PopulateTestTree();

            NUnitTest fixture = (NUnitTest)GetDescendantByName(testModel.RootTest, typeof(AmbiguousMatchSample).Name);
            Assert.AreEqual(1, fixture.Children.Count);

            NUnitTest test = (NUnitTest) fixture.Children[0];
            Assert.AreEqual("Test", test.Name);
            Assert.IsNull(test.CodeElement);
        }
    }
}
