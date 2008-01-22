extern alias MbUnit2;

using System.Collections.Generic;
using Gallio.Collections;
using Gallio.Data;
using MbUnit2::MbUnit.Framework;

namespace Gallio.Tests.Data
{
    [TestFixture]
    [TestsOn(typeof(ListDataRow<>))]
    public class ListDataRowTest
    {
        [Test, ExpectedArgumentNullException]
        public void ConstructorThrowsIfListIsNull()
        {
            new ListDataRow<object>(null, EmptyArray<KeyValuePair<string, string>>.Instance);
        }

        [Test]
        public void HasNoMetadataIfNullSpecifiedInConstructor()
        {
            ListDataRow<object> row = new ListDataRow<object>(EmptyArray<object>.Instance, null);
            List<KeyValuePair<string, string>> metadata = new List<KeyValuePair<string, string>>(row.GetMetadata());
            Assert.AreEqual(0, metadata.Count);
        }

        [Test]
        public void ContainSameMetadataAsSpecifiedInConstructor()
        {
            List<KeyValuePair<string, string>> metadata = new List<KeyValuePair<string, string>>();
            ListDataRow<object> row = new ListDataRow<object>(EmptyArray<object>.Instance, metadata);

            Assert.AreSame(metadata, row.GetMetadata());
        }

        [Test, ExpectedArgumentNullException]
        public void GetValueThrowsIfBindingIsNull()
        {
            ListDataRow<object> row = new ListDataRow<object>(EmptyArray<object>.Instance, null);
            row.GetValue(null);
        }

        [RowTest]
        [Row(null, null, ExpectedException=typeof(DataBindingException))]
        [Row(null, 3, ExpectedException=typeof(DataBindingException))]
        [Row(null, -1, ExpectedException=typeof(DataBindingException))]
        [Row("abc", null, ExpectedException=typeof(DataBindingException))]
        [Row("abc", 3, ExpectedException=typeof(DataBindingException))]
        [Row("abc", -1, ExpectedException=typeof(DataBindingException))]
        [Row(null, 0)]
        [Row("abc", 0)]
        [Row(null, 1)]
        [Row(null, 2)]
        public void GetValueReturnsValueOnlyIfTheBindingIndexIsWithinTheListCount(string path, object index)
        {
            object[] values = new object[] { "abc", "def", 42 };
            ListDataRow<object> row = new ListDataRow<object>(values, null);
            object value = row.GetValue(new SimpleDataBinding(typeof(string), path, (int?)index));

            Assert.AreEqual(values[(int)index], value);
        }
    }
}