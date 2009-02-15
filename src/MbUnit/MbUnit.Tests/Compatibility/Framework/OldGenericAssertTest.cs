// Copyright 2005-2009 Gallio Project - http://www.gallio.org/
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

using System.Collections.Generic;
using Gallio.Framework.Assertions;
using MbUnit.Framework;

#pragma warning disable 0618

namespace MbUnit.Tests.Compatibility.Framework
{
    // FIXME: May contain NUnit derived code but is missing proper attribution!
    //        Need to follow-up with the original contributor.
    [TestFixture]
    [TestsOn(typeof(OldGenericAssert))]
    public class OldGenericAssertTest
    {
        #region IsEmpty
        //NUnit Code
        [Test]
        public void IsEmpty()
        {
            OldGenericAssert.IsEmpty(new List<string>());
        }

        [Test, ExpectedException(typeof(AssertionException), "List expected to be empty")]
        public void IsEmptyFail()
        {
            List<string> arr = new List<string>();
            arr.Add("Testing");

            OldGenericAssert.IsEmpty(arr, "List");
        }

        #endregion

        #region IsNotEmpty

        [Test]
        public void IsNotEmpty()
        {
            List<string> arr = new List<string>();
            arr.Add("Testing");

            OldGenericAssert.IsNotEmpty(arr);
        }

        [Test, ExpectedException(typeof(AssertionException), "List expected not to be empty")]
        public void IsNotEmptyFail()
        {
            OldGenericAssert.IsNotEmpty(new List<string>(), "List");
        }

        #endregion
    }
}
