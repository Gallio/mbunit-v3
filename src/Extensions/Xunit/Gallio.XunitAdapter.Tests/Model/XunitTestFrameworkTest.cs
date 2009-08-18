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
using Gallio.Model;
using Gallio.Common.Reflection;
using Gallio.Model.Tree;
using Gallio.Runtime;
using Gallio.Runtime.Extensibility;
using Gallio.XunitAdapter.Model;
using Gallio.XunitAdapter.TestResources;
using Gallio.XunitAdapter.TestResources.Metadata;
using Gallio.Tests.Model;
using MbUnit.Framework;
using XunitAssert=Xunit.Assert;
using Test = Gallio.Model.Tree.Test;

namespace Gallio.XunitAdapter.Tests.Model
{
    [TestFixture]
    [TestsOn(typeof(XunitTestFramework))]
    [Author("Jeff", "jeff@ingenio.com")]
    public class XunitTestFrameworkTest : BaseTestFrameworkTest<SimpleTest>
    {
        protected override ComponentHandle<ITestFramework, TestFrameworkTraits> TestFrameworkHandle
        {
            get
            {
                return (ComponentHandle<ITestFramework, TestFrameworkTraits>)
                    RuntimeAccessor.ServiceLocator.ResolveHandleByComponentId("XunitAdapter.TestFramework");
            }
        }

        protected override string AssemblyKind
        {
            get { return XunitTestExplorer.AssemblyKind; }
        }

        [Test]
        public void MetadataImport_SkipReason()
        {
            TestModel testModel = PopulateTestTree();

            Test fixture = GetDescendantByName(testModel.RootTest, typeof(MetadataSample).Name);
            Test test = fixture.Children[0];
            Assert.AreEqual("For testing purposes.", test.Metadata.GetValue(MetadataKeys.IgnoreReason));
        }

        [Test]
        public void MetadataImport_Property()
        {
            TestModel testModel = PopulateTestTree();

            Test fixture = GetDescendantByName(testModel.RootTest, typeof(MetadataSample).Name);
            Test test = fixture.Children[0];
            Assert.AreEqual("customvalue-1", test.Metadata.GetValue("customkey-1"));
            Assert.AreEqual("customvalue-2", test.Metadata.GetValue("customkey-2"));
        }
    }
}
