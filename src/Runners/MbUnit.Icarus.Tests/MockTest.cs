using MbUnit.Framework;

using Rhino.Mocks;

namespace MbUnit.Icarus.Tests
{
    [TestFixture]
    public class MockTest
    {
        protected MockRepository mocks;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            mocks = new MockRepository();
        }

        [TearDown]
        public void TearDown()
        {
            mocks.VerifyAll();
        }
    }
}
