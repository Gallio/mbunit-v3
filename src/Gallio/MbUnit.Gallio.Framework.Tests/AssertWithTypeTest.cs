using System;

namespace MbUnit.Framework.Tests
{
    // FIXME: May contain NUnit derived code but is missing proper attribution!
    //        Need to follow-up with the original contributor.
    //NUnit Unit Tests
    [TestFixture]
    [TestsOn(typeof(Assert))]
    public class AssertWithTypeTest
    {
        [Test]
        public void IsInstanceOfType()
        {
            Assert.IsInstanceOfType(typeof(Exception), new ApplicationException());
        }

        [Test]
        public void IsNotInstanceOfType()
        {
            Assert.IsNotInstanceOfType(typeof(Int32), "abc123");
        }

        [Test()]
        public void IsAssignableFrom()
        {
            int[] array10 = new int[10];
            int[] array2 = new int[2];

            Assert.IsAssignableFrom(array2.GetType(), array10);
            Assert.IsAssignableFrom(array2.GetType(), array10, "Type Failure Message");
            Assert.IsAssignableFrom(array2.GetType(), array10, "Type Failure Message", null);
        }

        [Test()]
        public void IsNotAssignableFrom()
        {
            int[] array10 = new int[10];
            int[,] array2 = new int[2, 2];

            Assert.IsNotAssignableFrom(array2.GetType(), array10);
            Assert.IsNotAssignableFrom(array2.GetType(), array10, "Type Failure Message");
            Assert.IsNotAssignableFrom(array2.GetType(), array10, "Type Failure Message", null);
        }
    }
}
