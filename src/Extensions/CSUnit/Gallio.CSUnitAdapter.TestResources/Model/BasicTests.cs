// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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

using System;
using System.IO;
using csUnit;

namespace Gallio.CSUnitAdapter.TestResources.Metadata
{
    /// <summary>
    /// A simple test fixture.
    /// </summary>
    [TestFixture]
    public class SimpleTest
    {
        /// <summary>
        /// A passing test.
        /// </summary>
        [Test]
        public void TestThatPass()
        {
            Assert.True(1 == 1.0);
        }

        /// <summary>
        /// A failing test.
        /// </summary>
        [Test]
        public void TestThatFail()
        {
            Assert.Fail("Boom");
        }

        /// <summary>
        /// An ignored test.
        /// </summary>
        [Test]
        [Ignore("Blah bla")]
        public void ThatThatIsIgnored()
        {
        }

        /// <summary>
        /// A test that throws an exception.
        /// </summary>
        [Test]
        public void TestWithError()
        {
            throw new EndOfStreamException("Bla blah");
        }

        [Test]
        [ExpectedException(typeof(EndOfStreamException))]
        public void TestCatchException()
        {
            throw new EndOfStreamException("This should be caught");
        }



        [Test(Categories = "AssertFailures")]
        public void TestAssertFailure_EqualsDouble()
        {
            Assert.Equals(3.14, 3.145, "This is my message");    
        }

        [Test(Categories = "AssertFailures")]
        public void TestAssertFailure_GreaterInt()
        {
            Assert.Greater(1, 200);
        }

        [Test(Categories = "AssertFailures")]
        public void TestAssertFailure_Contains()
        {
            Assert.Contains("123", "abcdf", "What ever do you mean.");
        }

        [Test(Categories = "AssertFailures")]
        public void TestAssertFailure_Null()
        {
            Assert.Null(String.Empty);
        }
    }
}
