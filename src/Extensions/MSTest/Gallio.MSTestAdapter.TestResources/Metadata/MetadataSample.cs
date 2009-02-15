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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.Web;

namespace Gallio.MSTestAdapter.TestResources.Metadata
{
    [TestClass]
    [DeploymentItem("file1.xml", @"c:\SomePath\")]
    [Ignore]
    public class MetadataSample
    {
        [TestMethod]
        [Credential("Julian", "secret", "gallio.org")]
        [CssIteration("vstfs:///Classification/Node/3fe569cd-f84e-4375-a0f3-760ccb143bb7")]
        [CssProjectStructure("Gallio")]
        [DataSource("System.Data.SqlClient", "Server=.;Database=SomeDatabase;Trusted_Connection=Yes;",
            "Products", DataAccessMethod.Sequential)]
        [DeploymentItem("file1.xml", @"c:\SomePath\")]
        [Description("A sample description.")]
        [HostType("ASP.NET", "data")]
        [Ignore]
        [Owner("Julian")]
        [Priority(1)]
        [TestProperty("key1", "value1")]
        [TestProperty("key2", "value2")]
        [Timeout(100)]
        [UrlToTest("http://www.gallio.org")]
        [WorkItem(1)]
        [AspNetDevelopmentServer("WebSite1", @"C:\WebSites\WebSite1", "/WebSite1")]
        [AspNetDevelopmentServerHost(@"C:\WebSites\WebSite1", "/WebSite1")]
        public void Test()
        {
        }
    }
}
