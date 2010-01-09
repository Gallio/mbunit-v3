// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
// Portions Copyright 2000-2004 Jonathan de Halleux
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using MbUnit.Framework;

#pragma warning disable 0618

namespace MbUnit.Tests.Compatibility.Framework
{
    [TestFixture]
    [TestsOn(typeof(OldStringAssert))]
    public class OldStringAssertTest
    {
        #region AreEqual
        [Test]
        public void AreEqualIgnoreCase()
        {
            OldStringAssert.AreEqualIgnoreCase("hello", "HELLO");
        }

        #endregion

        #region Contains
        [Test]
        public void DoesNotContain()
        {
            OldStringAssert.DoesNotContain("hello", 'k');
        }

        [Test]
        public void StartWith()
        {
            string s = "frame work";
            string pattern = @"fra";
            OldStringAssert.StartsWith(s, pattern);
        }

        [Test]
        public void EndsWith()
        {
            string s = "framework";
            string pattern = @"ork";
            OldStringAssert.EndsWith(s, pattern);
        }

        [Test]
        public void Contains()
        {
            string s = "framework";
            string contain = "ork";
            OldStringAssert.Contains(s, contain);
        }

        #endregion

        #region Emptiness
        [Test]
        public void IsEmpty()
        {
            OldStringAssert.IsEmpty("");
        }

        [Test]
        public void IsNotEmpty()
        {
            OldStringAssert.IsNonEmpty(" ");
        }

        #endregion

        #region RegEx
        [Test]
        public void FullMatchString()
        {
            string s = "Testing";
            string pattern = @"^Testing$";

            OldStringAssert.FullMatch(s, pattern);
        }

        [Test]
        public void LikeString()
        {
            string s = "Testing";
            string regEx = @"(?<Test>\w{3})$";

            OldStringAssert.Like(s, regEx);
        }

        [Test]
        public void NotLikeString()
        {
            string s = "Testing";
            string regEx = @"notthere";

            OldStringAssert.NotLike(s, regEx);
        }


        #endregion
    }
}
