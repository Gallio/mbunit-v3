// Copyright 2007 MbUnit Project - http://www.mbunit.com/
// Portions Copyright 2000-2004 Jonathan De Halleux, Jamie Cansdale
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

namespace MbUnit.Tests.Framework
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
