using System.Collections.Generic;
using Gallio.Data;
using MbUnit.Framework;

namespace Gallio.Tests.Data
{
    [TestFixture]
    [TestsOn(typeof(NullDataRow))]
    public class NullDataRowTest
    {
        [Test]
        public void HasNoMetadata()
        {
            List<KeyValuePair<string, string>> metadata = new List<KeyValuePair<string, string>>(NullDataRow.Instance.GetMetadata());
            Assert.AreEqual(0, metadata.Count);
        }
        
        [Test, ExpectedArgumentNullException]
        public void GetValueThrowsIfBindingIsNull()
        {
            NullDataRow.Instance.GetValue(null);
        }

        [Test]
        public void GetValueReturnsDefaultValueForType()
        {
            Assert.AreEqual(0, NullDataRow.Instance.GetValue(new SimpleDataBinding(typeof(int))));
            Assert.AreEqual(0.0, NullDataRow.Instance.GetValue(new SimpleDataBinding(typeof(double))));
            Assert.AreEqual(null, NullDataRow.Instance.GetValue(new SimpleDataBinding(typeof(object))));
        }
    }
}