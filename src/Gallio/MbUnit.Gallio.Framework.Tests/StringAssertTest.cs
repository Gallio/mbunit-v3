namespace MbUnit.Framework.Tests
{
    [TestFixture]
    [TestsOn(typeof(StringAssert))]
    public class StringAssertTest
    {
        #region AreEqual
        [Test]
        public void AreEqualIgnoreCase()
        {
            StringAssert.AreEqualIgnoreCase("hello", "HELLO");
        }

        #endregion

        #region Contains
        [Test]
        public void DoesNotContain()
        {
            StringAssert.DoesNotContain("hello", 'k');
        }

        [Test]
        public void StartWith()
        {
            string s = "frame work";
            string pattern = @"fra";
            StringAssert.StartsWith(s, pattern);
        }

        [Test]
        public void EndsWith()
        {
            string s = "framework";
            string pattern = @"ork";
            StringAssert.EndsWith(s, pattern);
        }

        [Test]
        public void Contains()
        {
            string s = "framework";
            string contain = "ork";
            StringAssert.Contains(s, contain);
        }

        #endregion

        #region Emptiness
        [Test]
        public void IsEmpty()
        {
            StringAssert.IsEmpty("");
        }

        [Test]
        public void IsNotEmpty()
        {
            StringAssert.IsNonEmpty(" ");
        }

        #endregion

        #region RegEx
        [Test]
        public void FullMatchString()
        {
            string s = "Testing";
            string pattern = @"^Testing$";

            StringAssert.FullMatch(s, pattern);
        }

        [Test]
        public void LikeString()
        {
            string s = "Testing";
            string regEx = @"(?<Test>\w{3})$";

            StringAssert.Like(s, regEx);
        }

        [Test]
        [Ignore("Don't know RegEx for NotLike")]
        public void NotLikeString()
        {
            string s = "Testing";
            string regEx = @"";

            StringAssert.NotLike(s, regEx);
        }


        #endregion
    }
}
