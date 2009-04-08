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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gallio.Framework.Assertions;
using MbUnit.Framework;

namespace MbUnit.Tests.Framework
{
    [TestsOn(typeof(Assert))]
	public class AssertTest_Sorting : BaseAssertTest
	{
        public enum ExpectedResult
        {
            Sorted,
            Unsorted,
            Uncomparable,
        }

        internal class Foo1 : IComparable
        {
            private readonly int value;

            public Foo1(int value)
            {
                this.value = value;
            }

            public int CompareTo(object obj)
            {
                return value.CompareTo(((Foo1)obj).value);
            }
        }

        internal class Foo2 : IComparable<Foo2>
        {
            private readonly int value;

            public Foo2(int value)
            {
                this.value = value;
            }

            public int CompareTo(Foo2 other)
            {
                return value.CompareTo(other.value);
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

        public static IEnumerable<object[]> ProvideSortedTestData()
        {
            // With a simple value typea (Int32 and string)
            yield return new object[] { typeof(int), new int[] { 1, 2, 2, 3 }, SortOrder.Increasing, ExpectedResult.Sorted };
            yield return new object[] { typeof(int), new int[] { 1, 2, 3, 4 }, SortOrder.Increasing, ExpectedResult.Sorted };
            yield return new object[] { typeof(int), new int[] { 4, 3, 2, 1 }, SortOrder.Increasing, ExpectedResult.Unsorted };
            yield return new object[] { typeof(int), new int[] { 1, 2, 2, 3 }, SortOrder.StrictlyIncreasing, ExpectedResult.Unsorted };
            yield return new object[] { typeof(int), new int[] { 1, 2, 3, 4 }, SortOrder.StrictlyIncreasing, ExpectedResult.Sorted };
            yield return new object[] { typeof(int), new int[] { 3, 2, 2, 1 }, SortOrder.Decreasing, ExpectedResult.Sorted };
            yield return new object[] { typeof(int), new int[] { 4, 3, 2, 1 }, SortOrder.Decreasing, ExpectedResult.Sorted };
            yield return new object[] { typeof(int), new int[] { 1, 2, 3, 4 }, SortOrder.Decreasing, ExpectedResult.Unsorted };
            yield return new object[] { typeof(int), new int[] { 3, 2, 2, 1 }, SortOrder.StrictlyDecreasing, ExpectedResult.Unsorted };
            yield return new object[] { typeof(int), new int[] { 4, 3, 2, 1 }, SortOrder.StrictlyDecreasing, ExpectedResult.Sorted };
            yield return new object[] { typeof(string), new string[] { "cat", "dog", "dog", "pig" }, SortOrder.Increasing, ExpectedResult.Sorted };
            yield return new object[] { typeof(string), new string[] { "cat", "dog", "pig", "rat" }, SortOrder.Increasing, ExpectedResult.Sorted };
            yield return new object[] { typeof(string), new string[] { "rat", "pig", "dog", "cat" }, SortOrder.Increasing, ExpectedResult.Unsorted };
            yield return new object[] { typeof(string), new string[] { "cat", "dog", "dog", "pig" }, SortOrder.StrictlyIncreasing, ExpectedResult.Unsorted };
            yield return new object[] { typeof(string), new string[] { "cat", "dog", "pig", "rat" }, SortOrder.StrictlyIncreasing, ExpectedResult.Sorted };
            yield return new object[] { typeof(string), new string[] { "pig", "dog", "dog", "cat" }, SortOrder.Decreasing, ExpectedResult.Sorted };
            yield return new object[] { typeof(string), new string[] { "rat", "pig", "dog", "cat" }, SortOrder.Decreasing, ExpectedResult.Sorted };
            yield return new object[] { typeof(string), new string[] { "cat", "dog", "pig", "rat" }, SortOrder.Decreasing, ExpectedResult.Unsorted };
            yield return new object[] { typeof(string), new string[] { "pig", "dog", "dog", "cat" }, SortOrder.StrictlyDecreasing, ExpectedResult.Unsorted };
            yield return new object[] { typeof(string), new string[] { "rat", "pig", "dog", "cat" }, SortOrder.StrictlyDecreasing, ExpectedResult.Sorted };
            
            // With a custom type implementing System.IComparable.
            yield return new object[] { typeof(Foo1), new Foo1[] { new Foo1(1), new Foo1(2), new Foo1(2), new Foo1(3) }, SortOrder.Increasing, ExpectedResult.Sorted };
            yield return new object[] { typeof(Foo1), new Foo1[] { new Foo1(1), new Foo1(2), new Foo1(3), new Foo1(4) }, SortOrder.Increasing, ExpectedResult.Sorted };
            yield return new object[] { typeof(Foo1), new Foo1[] { new Foo1(4), new Foo1(3), new Foo1(2), new Foo1(1) }, SortOrder.Increasing, ExpectedResult.Unsorted };
            yield return new object[] { typeof(Foo1), new Foo1[] { new Foo1(1), new Foo1(2), new Foo1(2), new Foo1(3) }, SortOrder.StrictlyIncreasing, ExpectedResult.Unsorted };
            yield return new object[] { typeof(Foo1), new Foo1[] { new Foo1(1), new Foo1(2), new Foo1(3), new Foo1(4) }, SortOrder.StrictlyIncreasing, ExpectedResult.Sorted };
            yield return new object[] { typeof(Foo1), new Foo1[] { new Foo1(3), new Foo1(2), new Foo1(2), new Foo1(1) }, SortOrder.Decreasing, ExpectedResult.Sorted };
            yield return new object[] { typeof(Foo1), new Foo1[] { new Foo1(4), new Foo1(3), new Foo1(2), new Foo1(1) }, SortOrder.Decreasing, ExpectedResult.Sorted };
            yield return new object[] { typeof(Foo1), new Foo1[] { new Foo1(1), new Foo1(2), new Foo1(3), new Foo1(4) }, SortOrder.Decreasing, ExpectedResult.Unsorted };
            yield return new object[] { typeof(Foo1), new Foo1[] { new Foo1(3), new Foo1(2), new Foo1(2), new Foo1(1) }, SortOrder.StrictlyDecreasing, ExpectedResult.Unsorted };
            yield return new object[] { typeof(Foo1), new Foo1[] { new Foo1(4), new Foo1(3), new Foo1(2), new Foo1(1) }, SortOrder.StrictlyDecreasing, ExpectedResult.Sorted };

            // With a custom type implementing System.IComparable<T>.
            yield return new object[] { typeof(Foo2), new Foo2[] { new Foo2(1), new Foo2(2), new Foo2(2), new Foo2(3) }, SortOrder.Increasing, ExpectedResult.Sorted };
            yield return new object[] { typeof(Foo2), new Foo2[] { new Foo2(1), new Foo2(2), new Foo2(3), new Foo2(4) }, SortOrder.Increasing, ExpectedResult.Sorted };
            yield return new object[] { typeof(Foo2), new Foo2[] { new Foo2(4), new Foo2(3), new Foo2(2), new Foo2(1) }, SortOrder.Increasing, ExpectedResult.Unsorted };
            yield return new object[] { typeof(Foo2), new Foo2[] { new Foo2(1), new Foo2(2), new Foo2(2), new Foo2(3) }, SortOrder.StrictlyIncreasing, ExpectedResult.Unsorted };
            yield return new object[] { typeof(Foo2), new Foo2[] { new Foo2(1), new Foo2(2), new Foo2(3), new Foo2(4) }, SortOrder.StrictlyIncreasing, ExpectedResult.Sorted };
            yield return new object[] { typeof(Foo2), new Foo2[] { new Foo2(3), new Foo2(2), new Foo2(2), new Foo2(1) }, SortOrder.Decreasing, ExpectedResult.Sorted };
            yield return new object[] { typeof(Foo2), new Foo2[] { new Foo2(4), new Foo2(3), new Foo2(2), new Foo2(1) }, SortOrder.Decreasing, ExpectedResult.Sorted };
            yield return new object[] { typeof(Foo2), new Foo2[] { new Foo2(1), new Foo2(2), new Foo2(3), new Foo2(4) }, SortOrder.Decreasing, ExpectedResult.Unsorted };
            yield return new object[] { typeof(Foo2), new Foo2[] { new Foo2(3), new Foo2(2), new Foo2(2), new Foo2(1) }, SortOrder.StrictlyDecreasing, ExpectedResult.Unsorted };
            yield return new object[] { typeof(Foo2), new Foo2[] { new Foo2(4), new Foo2(3), new Foo2(2), new Foo2(1) }, SortOrder.StrictlyDecreasing, ExpectedResult.Sorted };

            // With a custom type without implicit comparison feature.
            yield return new object[] { typeof(Foo3), new Foo3[] { new Foo3(1), new Foo3(2), new Foo3(2), new Foo3(3) }, SortOrder.Increasing, ExpectedResult.Uncomparable };
        }

        [Test]
        [Factory("ProvideSortedTestData")]
        public void Sorted_with_implicit_comparer<T>(T[] values, SortOrder sortOrder, ExpectedResult expectedResult)
        {
            AssertionFailure[] failures = Capture(() => Assert.Sorted<T>(values, sortOrder));

            switch (expectedResult)
            {
                case ExpectedResult.Sorted:
                    Assert.IsEmpty(failures);
                    break;
                
                case ExpectedResult.Unsorted:
                    Assert.AreEqual(1, failures.Length);
                    Assert.AreEqual("Expected the elements to be sorted in a specific order but the sequence of values mismatches at one position at least.", failures[0].Description);
                    break;
                
                case ExpectedResult.Uncomparable:
                    Assert.AreEqual(1, failures.Length);
                    Assert.AreEqual("Expected the elements to be sorted in a specific order but no implicit ordering comparison can be found for the subject type.", failures[0].Description);
                    break;
            }
        }


        [Test]
        [Row(typeof(int), new int[] { 1, 2, 3, 4 }, SortOrder.Decreasing, ExpectedResult.Sorted)]
        [Row(typeof(int), new int[] { 1, 2, 3, 4 }, SortOrder.Increasing, ExpectedResult.Unsorted)]
        [Row(typeof(int), new int[] { 4, 3, 2, 1 }, SortOrder.Decreasing, ExpectedResult.Unsorted)]
        [Row(typeof(int), new int[] { 4, 3, 2, 1 }, SortOrder.Increasing, ExpectedResult.Sorted)]
        public void Sorted_with_explicit_comparer<T>(T[] values, SortOrder sortOrder, ExpectedResult expectedResult)
            where T : IComparable<T>
        {
            // Provide a comparison function that inverts the comparison result!
            AssertionFailure[] failures = Capture(() => Assert.Sorted<T>(values, sortOrder, (x, y) => -x.CompareTo(y)));

            switch (expectedResult)
            {
                case ExpectedResult.Sorted:
                    Assert.IsEmpty(failures);
                    break;

                case ExpectedResult.Unsorted:
                    Assert.AreEqual(1, failures.Length);
                    Assert.AreEqual("Expected the elements to be sorted in a specific order but the sequence of values mismatches at one position at least.", failures[0].Description);
                    break;
            }
        }
    }
}
