using System;
using Gallio.Framework.Data;
using Gallio.Framework.Data.Binders;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.Tests.Framework.Data.Binders
{
    [TestFixture]
    [TestsOn(typeof(ConstantDataBindingAccessor))]
    public class ConstantDataBindingAccessorTest : BaseUnitTest
    {
        [Test, ExpectedArgumentNullException]
        public void GetValueThrowsIfItemIsNull()
        {
            new ConstantDataBindingAccessor(42).GetValue(null);
        }

        [Test]
        public void GetValueReturnsSameConstantAndSuppliedInTheConstructor()
        {
            DataBindingItem item = new DataBindingItem(Mocks.Stub<IDataRow>());
            Assert.AreEqual(42, new ConstantDataBindingAccessor(42).GetValue(item));
        }
    }
}