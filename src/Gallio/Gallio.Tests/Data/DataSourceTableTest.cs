extern alias MbUnit2;

using System.Collections.Generic;
using Gallio.Data;
using MbUnit2::MbUnit.Framework;

namespace Gallio.Tests.Data
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