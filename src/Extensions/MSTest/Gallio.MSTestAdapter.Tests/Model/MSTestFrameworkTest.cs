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
using Gallio.MSTestAdapter.Model;
using Gallio.MSTestAdapter.TestResources;
using Gallio.MSTestAdapter.TestResources.Metadata;
using Gallio.Runtime;
using Gallio.Runtime.Extensibility;
using Gallio.Tests.Model;
using MbUnit.Framework;
using Test = Gallio.Model.Tree.Test;

namespace Gallio.MSTestAdapter.Tests.Model
{
    [TestFixture]
    [TestsOn(typeof(MSTestFramework))]
    [Author("Julian", "julian.hidalgo@gallio.org")]
    public class MSTestFrameworkTest : BaseTestFrameworkTest<SimpleTest>
    {
        protected override ComponentHandle<ITestFramework, TestFrameworkTraits> FrameworkHandle
        {
            get
            {
                return (ComponentHandle<ITestFramework, TestFrameworkTraits>)
                    RuntimeAccessor.ServiceLocator.ResolveHandleByComponentId("MSTestAdapter.TestFramework");
            }
        }

        protected override string AssemblyKind
        {
            get { return MSTestExplorer.AssemblyKind; }
        }

        [Test]
        public void MetadataImport_IgnoreReason()
        {
            TestModel testModel = PopulateTestTree();

            Test fixture = GetDescendantByName(testModel.RootTest, typeof(MetadataSample).Name);
            Assert.AreEqual("The Ignore attribute was applied to this class.", fixture.Metadata.GetValue(MetadataKeys.IgnoreReason));

            Test test = fixture.Children[0];
            Assert.AreEqual("The Ignore attribute was applied to this test.", test.Metadata.GetValue(MetadataKeys.IgnoreReason));
        }

        [Test]
        public void MetadataImport_TestClass()
        {
            TestModel testModel = PopulateTestTree();

            Test fixture = GetDescendantByName(testModel.RootTest, typeof(MetadataSample).Name);
            Assert.IsNotNull(fixture, "Cannot find fixture 'MetadataSample'.");

            Assert.AreEqual(@"Path=file1.xml, OutputDirectory=c:\SomePath\", fixture.Metadata.GetValue(MSTestMetadataKeys.DeploymentItem));
        }

        [Test]
        public void MetadataImport_TestMethod()
        {
            TestModel testModel = PopulateTestTree();

            Test fixture = GetDescendantByName(testModel.RootTest, typeof(MetadataSample).Name);
            Assert.IsNotNull(fixture, "Cannot find fixture 'MetadataSample'.");

            Test test = fixture.Children[0];
            Assert.IsNotNull(fixture, "Cannot find test method 'Test' in fixture 'MetadataSample'.");

            Assert.AreEqual(@"Name=WebSite1, PathToWebApp=C:\WebSites\WebSite1, WebAppRoot=/WebSite1", test.Metadata.GetValue(MSTestMetadataKeys.AspNetDevelopmentServer));
            Assert.AreEqual(@"PathToWebApp=C:\WebSites\WebSite1, WebAppRoot=/WebSite1", test.Metadata.GetValue(MSTestMetadataKeys.AspNetDevelopmentServerHost));
            Assert.AreEqual("UserName=Julian, Password=secret, Domain=gallio.org", test.Metadata.GetValue(MSTestMetadataKeys.Credential));
            Assert.AreEqual("vstfs:///Classification/Node/3fe569cd-f84e-4375-a0f3-760ccb143bb7", test.Metadata.GetValue(MSTestMetadataKeys.CssIteration));
            Assert.AreEqual("Gallio", test.Metadata.GetValue(MSTestMetadataKeys.CssProjectStructure));
            Assert.AreEqual(@"ConnectionString=Server=.;Database=SomeDatabase;Trusted_Connection=Yes;, " +
                "DataAccessMethod=Sequential, ProviderInvariantName=System.Data.SqlClient, TableName=Products", test.Metadata.GetValue(MSTestMetadataKeys.DataSource));
            Assert.AreEqual(@"Path=file1.xml, OutputDirectory=c:\SomePath\", test.Metadata.GetValue(MSTestMetadataKeys.DeploymentItem));
            Assert.AreEqual("A sample description.", test.Metadata.GetValue(MetadataKeys.Description));
            Assert.AreEqual("HostType=ASP.NET, HostData=data", test.Metadata.GetValue(MSTestMetadataKeys.HostType));
            Assert.AreEqual("Julian", test.Metadata.GetValue(MSTestMetadataKeys.Owner));
            Assert.AreEqual("1", test.Metadata.GetValue(MSTestMetadataKeys.Priority));
            Assert.AreEqual("100", test.Metadata.GetValue(MSTestMetadataKeys.Timeout));
            Assert.AreEqual("http://www.gallio.org", test.Metadata.GetValue(MSTestMetadataKeys.UrlToTest));
            Assert.AreEqual("1", test.Metadata.GetValue(MSTestMetadataKeys.WorkItem));
            Assert.AreEqual("value1", test.Metadata.GetValue("key1"));
            Assert.AreEqual("value2", test.Metadata.GetValue("key2"));
        }
    }
}
