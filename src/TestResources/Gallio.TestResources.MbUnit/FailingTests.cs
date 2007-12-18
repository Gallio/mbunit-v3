using MbUnit.Framework;

namespace Gallio.TestResources.MbUnit
{
    /// <summary>
    /// This class is used by the MSBuild task tests. Please don't modify it.
    /// </summary>
    [TestFixture]
    public class FailingTests
    {
        [Test]
        public void Fail()
        {
            Assert.Fail();
        }

        [Test]
        public void FailAgain()
        {
            Assert.Fail();
        }
    }
}