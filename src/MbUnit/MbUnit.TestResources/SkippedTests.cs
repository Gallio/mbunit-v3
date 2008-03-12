using MbUnit.Framework;

namespace MbUnit.TestResources
{
    [TestFixture]
    public class SkippedTests
    {
        [Test]
        [Ignore("Won't run")]
        public void IgnoredTest()
        {
            Assert.Fail("Should never get here");   
        }

        [Test]
        [Pending("Won't run")]
        public void PendingTest()
        {
            Assert.Fail("Should never get here");
        }

        [Test]
        [Explicit("Won't run")]
        public void ExplicitTest()
        {
        }

        [TestFixture]
        [Explicit("Should only run if explicitly selected or if one of its test cases is explicitly selected.")]
        public class ExplicitFixture
        {
            [Test]
            public void Test()
            {
            }
        }
    }
}