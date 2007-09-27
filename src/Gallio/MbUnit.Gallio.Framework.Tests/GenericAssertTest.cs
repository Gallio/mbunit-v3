using System.Collections.Generic;

namespace MbUnit.Framework.Tests
{
    // FIXME: May contain NUnit derived code but is missing proper attribution!
    //        Need to follow-up with the original contributor.
    [TestFixture]
    [TestsOn(typeof(GenericAssert))]
    public class GenericAssertTest
    {
        #region IsEmpty
        //NUnit Code
        [Test]
        public void IsEmpty()
        {
            GenericAssert.IsEmpty(new List<string>());
        }

        [Test, ExpectedException(typeof(AssertionException), "List expected to be empty")]
        public void IsEmptyFail()
        {
            List<string> arr = new List<string>();
            arr.Add("Testing");

            GenericAssert.IsEmpty(arr, "List");
        }

        #endregion

        #region IsNotEmpty

        [Test]
        public void IsNotEmpty()
        {
            List<string> arr = new List<string>();
            arr.Add("Testing");

            GenericAssert.IsNotEmpty(arr);
        }

        [Test, ExpectedException(typeof(AssertionException), "List expected not to be empty")]
        public void IsNotEmptyFail()
        {
            GenericAssert.IsNotEmpty(new List<string>(), "List");
        }

        #endregion
    }
}
