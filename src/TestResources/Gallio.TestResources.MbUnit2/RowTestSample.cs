using MbUnit.Framework;

namespace Gallio.TestResources.MbUnit2
{
    [TestFixture]
    public class RowTestSample
    {
        [RowTest]
        [Row(3, 4, 5)]
        [Row(6, 8, 10)]
        [Row(1, 1, 1, Description="This one should fail.")]
        public void Pythagoras(int a, int b, int c)
        {
            Assert.AreEqual(c * c, a * a + b * b);
        }
    }
}
