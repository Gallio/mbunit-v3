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

using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Gallio.MSTestAdapter.TestResources
{
    /// <summary>
    /// A simple data driven fixture.
    /// </summary>
    [TestClass]
    public class DataDrivenTest
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        [DeploymentItem(@"DataDrivenTest.xml")]
        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.XML",
            @"|DataDirectory|\DataDrivenTest.xml",
            "Row",
            DataAccessMethod.Sequential)]
        public void Pythagoras()
        {
            int a = int.Parse((string)TestContext.DataRow["A"]);
            int b = int.Parse((string)TestContext.DataRow["B"]);
            int c = int.Parse((string)TestContext.DataRow["C"]);

            Assert.AreEqual(c * c, a * a + b * b);
        }
    }
}
