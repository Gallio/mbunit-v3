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

using csUnit;

namespace Gallio.CSUnitAdapter.TestResources.Metadata
{
    /// <summary>
    /// A simple class that make use of categories.
    /// </summary>
    [TestFixture(Categories = "DB")]
    public class DatabaseFixture
    {
        /// <summary>
        /// A test that make use of categories.
        /// </summary>
        [Test(Categories = "mySQL")]
        public void Test1()
        {
        }

        /// <summary>
        /// Another test that make use of categories.
        /// </summary>
        [Test(Categories = "MSFT")]
        public void Test2()
        {
            Assert.Fail("Boom!");
        }
    }

    /// <summary>
    /// Another class that make use of categories.
    /// </summary>
    [TestFixture(Categories = "DB, DataSet")]
    public class DatasetFixture
    {
        /// <summary>
        /// First test
        /// </summary>
        [Test(Categories = "mySQL")]
        public void TestA()
        {
        }

        /// <summary>
        /// Second test
        /// </summary>
        [Test(Categories = "MSFT")]
        [Ignore("not relevant")]
        public void TestB()
        {
        }

        /// <summary>
        /// Third test
        /// </summary>
        [Test(Categories = "Oracle")]
        public void TestC()
        {
        }
    }
}
