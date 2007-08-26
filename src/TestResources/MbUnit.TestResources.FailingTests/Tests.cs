using MbUnit.Framework;

namespace MbUnit.TestResources.FailingTests
{
    [TestFixture]
    public class Tests
    {
        [Test]
        public void Test1()
        {
            Assert.AreEqual(1, 2);
        }

        [Test]
        public void Test2()
        {
            Assert.AreEqual("1", "2");
        }
    }
}
