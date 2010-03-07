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

using System;
using MbUnit.Framework;

#pragma warning disable 0618

namespace MbUnit.Compatibility.Tests.Framework
{
    // FIXME: May contain NUnit derived code but is missing proper attribution!
    //        Need to follow-up with the original contributor.
    //NUnit Unit Tests
    [TestFixture]
    [TestsOn(typeof(OldAssert))]
    public class OldAssertWithTypeTest
    {
        [Test]
        public void IsInstanceOfType()
        {
            OldAssert.IsInstanceOfType(typeof(Exception), new ApplicationException());
        }

        [Test]
        public void IsNotInstanceOfType()
        {
            OldAssert.IsNotInstanceOfType(typeof(Int32), "abc123");
        }

        [Test()]
        public void IsAssignableFrom()
        {
            int[] array10 = new int[10];
            int[] array2 = new int[2];

            OldAssert.IsAssignableFrom(array2.GetType(), array10);
            OldAssert.IsAssignableFrom(array2.GetType(), array10, "Type Failure Message");
            OldAssert.IsAssignableFrom(array2.GetType(), array10, "Type Failure Message", null);
        }

        [Test()]
        public void IsNotAssignableFrom()
        {
            int[] array10 = new int[10];
            int[,] array2 = new int[2, 2];

            OldAssert.IsNotAssignableFrom(array2.GetType(), array10);
            OldAssert.IsNotAssignableFrom(array2.GetType(), array10, "Type Failure Message");
            OldAssert.IsNotAssignableFrom(array2.GetType(), array10, "Type Failure Message", null);
        }
    }
}
