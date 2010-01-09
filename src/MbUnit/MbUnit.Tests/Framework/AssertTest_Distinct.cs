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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gallio.Framework.Assertions;
using MbUnit.Framework;

namespace MbUnit.Tests.Framework
{
    [TestsOn(typeof(Assert))]
	public class AssertTest_Distinct : BaseAssertTest
	{
        internal class Foo1 : IEquatable<Foo1>
        {
            private readonly int value;

            public Foo1(int value)
            {
                this.value = value;
            }

            public bool Equals(Foo1 other)
            {
                return (other != null) && (value == other.value);
            }

            public override bool Equals(object obj)
            {
                return Equals(obj as Foo1);
            }

            public override int GetHashCode()
            {
                return value;
            }
        }

        internal class Foo2
        {
            private readonly int value;

            public Foo2(int value)
            {
                this.value = value;
            }

            public override bool Equals(object obj)
            {
                var other = obj as Foo2;
                return (other != null) && (value == other.value);
            }

            public override int GetHashCode()
            {
                return value;
            }
        }

        internal class Foo3
        {
            private readonly int value;

            public Foo3(int value)
            {
                this.value = value;
            }
        }

        public static IEnumerable<object[]> ProvideTestData()
        {
            yield return new object[] { typeof(int), new int[] { 1, 2, 3 }, 0 };
            yield return new object[] { typeof(int), new int[] { 1, 2, 2 }, 1 };
            yield return new object[] { typeof(int), new int[] { 2, 1, 2, 2, 2, 2, 2, 2 }, 1 };
            yield return new object[] { typeof(int), new int[] { 2, 1, 4, 2, 3, 2, 2, 3 }, 2 };
            yield return new object[] { typeof(string), new string[] { "1", "2", "3" }, 0 };
            yield return new object[] { typeof(string), new string[] { "1", "2", "2" }, 1 };
            yield return new object[] { typeof(string), new string[] { "1", "2", "2", "3", "2", "3" }, 2 };
            yield return new object[] { typeof(string), new string[] { "1", "2", "4", "2", "3", "2", "3", "4" }, 3 };
            yield return new object[] { typeof(Foo1), new Foo1[] { new Foo1(1), new Foo1(2), new Foo1(3) }, 0 };
            yield return new object[] { typeof(Foo1), new Foo1[] { new Foo1(1), new Foo1(2), new Foo1(2) }, 1 };
            yield return new object[] { typeof(Foo1), new Foo1[] { new Foo1(1), new Foo1(2), new Foo1(2), new Foo1(1) }, 2 };
            yield return new object[] { typeof(Foo2), new Foo2[] { new Foo2(1), new Foo2(2), new Foo2(3) }, 0 };
            yield return new object[] { typeof(Foo2), new Foo2[] { new Foo2(1), new Foo2(2), new Foo2(2) }, 1 };
            yield return new object[] { typeof(Foo2), new Foo2[] { new Foo2(1), new Foo2(3), new Foo2(2), new Foo2(2), new Foo2(2), new Foo2(3) }, 2 };
            yield return new object[] { typeof(Foo3), new Foo3[] { new Foo3(1), new Foo3(2), new Foo3(3) }, 0 };
            yield return new object[] { typeof(Foo3), new Foo3[] { new Foo3(1), new Foo3(2), new Foo3(2) }, 0 };
        }

        [Test]
        [Factory("ProvideTestData")]
        public void Distinct_ok<T>(IEnumerable<T> values, int expectedDuplicates)
        {
            AssertionFailure[] failures = Capture(() => Assert.Distinct<T>(values));

            if (expectedDuplicates == 0)
            {
                Assert.IsEmpty(failures);
            }
            else
            {
                Assert.AreEqual(1, failures.Length);
                Assert.AreEqual("Expected the elements to be distinct but several instances represents the same value.", failures[0].Description);
                Assert.AreEqual(expectedDuplicates, failures[0].LabeledValues.Count(x => x.Label == "Duplicated Value"));
            }
        }
    }
}
