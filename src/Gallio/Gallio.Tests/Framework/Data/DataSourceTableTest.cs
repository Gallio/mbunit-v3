// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
// Portions Copyright 2000-2004 Jonathan De Halleux, Jamie Cansdale
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


using Gallio.Framework.Data;
using MbUnit.Framework;

namespace Gallio.Tests.Framework.Data
{
    [TestFixture]
    [TestsOn(typeof(DataSourceTable))]
    public class DataSourceTableTest
    {
        [Test]
        public void DefineDataSourceReturnsTheSameDataSourceIfAlreadyDefined()
        {
            DataSourceTable table = new DataSourceTable();
            DataSource source = table.DefineDataSource("source");

            Assert.AreSame(source, table.DefineDataSource("source"));
        }

        [Test]
        public void ResolveDataSourceReturnsNullIfTheDataSourceIsNotDefinedButOthersAre()
        {
            DataSourceTable table = new DataSourceTable();
            table.DefineDataSource("someOtherSource");

            Assert.IsNull(table.ResolveDataSource("test"));
        }

        [Test]
        public void ResolveDataSourceReturnsNullIfNoDataSourcesAreDefined()
        {
            DataSourceTable table = new DataSourceTable();
            Assert.IsNull(table.ResolveDataSource("test"));
        }

        [Test]
        public void ResolveDataSourceReturnsADataSource()
        {
            DataSourceTable table = new DataSourceTable();

            DataSource source1 = table.DefineDataSource("source1");
            Assert.IsNotNull(source1);

            DataSource source2 = table.DefineDataSource("source2");
            Assert.IsNotNull(source2);

            Assert.AreSame(source1, table.ResolveDataSource("source1"));
            Assert.AreSame(source2, table.ResolveDataSource("source2"));
        }

        [Test, ExpectedArgumentNullException]
        public void DefineDataSourceThrowsIfNameIsNull()
        {
            DataSourceTable table = new DataSourceTable();
            table.DefineDataSource(null);
        }

        [Test, ExpectedArgumentNullException]
        public void ResolveDataSourceThrowsIfNameIsNull()
        {
            DataSourceTable table = new DataSourceTable();
            table.ResolveDataSource(null);
        }
    }
}