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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Gallio.Framework;
using Gallio.Runtime.Extensibility;
using MbUnit.Framework;
using System.IO;

namespace Gallio.Tests.Framework
{
    [TestFixture]
    [TestsOn(typeof(DefaultComparisonSemantics))]
    public class DefaultComparisonSemanticsTest
    {
        private DefaultComparisonSemantics comparisonSemantics;
        private IExtensionPoints extensionPoints;

        [SetUp]
        public void Setup()
        {
            extensionPoints = new DefaultExtensionPoints();
            comparisonSemantics = new DefaultComparisonSemantics(extensionPoints);
        }

        [Test]
        public void SameConsidersNullEqual()
        {
            Assert.IsTrue(comparisonSemantics.Same<object>(null, null));
        }

        [Test]
        public void SameConsidersNullNotEqualToNonNull()
        {
            Assert.IsFalse(comparisonSemantics.Same<object>(null, "a"));
            Assert.IsFalse(comparisonSemantics.Same<object>("a", null));
        }

        [Test]
        public void SameUsesReferentialIdentityForReferenceTypes()
        {
            var a = new object();
            var b = new object();
            Assert.IsTrue(comparisonSemantics.Same(a, a));
            Assert.IsFalse(comparisonSemantics.Same(a, b));
        }

        [Test]
        public void EqualsConsidersNullsEqual()
        {
            Assert.IsTrue(comparisonSemantics.Equals<object>(null, null));
        }

        [Test]
        public void EqualsConsidersReferentiallyIdenticalObjectstoBeEqual()
        {
            var a = new object();
            Assert.IsTrue(comparisonSemantics.Equals(a, a));
        }

        [Test]
        public void EqualsConsidersNullAndNonNullValuesDistinct()
        {
            Assert.IsFalse(comparisonSemantics.Equals<object>(null, "a"));
            Assert.IsFalse(comparisonSemantics.Equals<object>("a", null));
        }

        [Test]
        public void EqualsComparesValuesUsingObjectEquality()
        {
            Assert.IsTrue(comparisonSemantics.Equals("a", "a"));
            Assert.IsTrue(comparisonSemantics.Equals("a", "A".ToLowerInvariant()));
            Assert.IsTrue(comparisonSemantics.Equals(1, 1));
            Assert.IsFalse(comparisonSemantics.Equals("a", "b"));
            Assert.IsFalse(comparisonSemantics.Equals(1, 2));
        }

        [Test]
        public void EqualsComparesArraysByContent()
        {
            Assert.IsTrue(comparisonSemantics.Equals(new[] { 2, 3, 5, 7 }, new[] { 2, 3, 5, 7 }));
            Assert.IsFalse(comparisonSemantics.Equals(new[] { 2, 3, 5 }, new[] { 2, 3, 5, 7 }));
            Assert.IsFalse(comparisonSemantics.Equals(new[] { 2, 3, 5, 7 }, new[] { 2, 3, 5 }));
            Assert.IsFalse(comparisonSemantics.Equals(new int[] { }, new[] { 2, 3, 5 }));
            Assert.IsFalse(comparisonSemantics.Equals(new[] { 2, 3, 5 }, new int[] { }));
            Assert.IsTrue(comparisonSemantics.Equals(new int[] { }, new int[] { }));
        }

        [Test]
        public void EqualsComparesSimpleEnumerablesRecursivelyByContent()
        {
            Assert.IsTrue(comparisonSemantics.Equals(new List<int[]> { new[] { 1, 2 }, new[] { 9, 10 } },
                new List<int[]> { new[] { 1, 2 }, new[] { 9, 10 } }));
            Assert.IsFalse(comparisonSemantics.Equals(new List<int[]> { new[] { 1, 2 }, new[] { 9, 11 } },
                new List<int[]> { new[] { 1, 2 }, new[] { 9, 10 } }));
            Assert.IsFalse(comparisonSemantics.Equals(new List<int[]> { new[] { 1, 2 }, new[] { 9, 10 } },
                new List<int[]> { new[] { 1, 2 } }));
            Assert.IsFalse(comparisonSemantics.Equals(new List<int[]> { new[] { 1, 2 }, new[] { 9, 10 } },
                new List<int[]> { new[] { 1, 2 }, new[] { 9, 10, 11 } }));
        }

        [Test]
        public void EqualsComparesSimpleEnumerablesDistinctIfTheyHaveDifferentTypes()
        {
            Assert.IsFalse(comparisonSemantics.Equals(new List<int> { 1, 2 }, new[] { 1, 2 }));
        }

        [Test]
        public void Equals_WhenObjectsAreOfTypeAssemblyName_ComparesByFullName()
        {
            Assert.IsTrue(comparisonSemantics.Equals(
                new AssemblyName("Gallio, Version=0.0.0.0"),
                new AssemblyName("Gallio, Version=0.0.0.0")));

            Assert.IsFalse(comparisonSemantics.Equals(
                new AssemblyName("Gallio, Version=0.0.0.0"),
                new AssemblyName("Gallio")));
        }

        [Test]
        public void Equals_WhenObjectsAreOfTypeFileInfo_ComparesByToString()
        {
            Assert.IsTrue(comparisonSemantics.Equals(
                new FileInfo("a\\file.txt"),
                new FileInfo("a\\file.txt")));

            Assert.IsFalse(comparisonSemantics.Equals(
                new FileInfo("a\\file.txt"),
                new FileInfo("file.txt")));
        }

        [Test]
        public void Equals_WhenObjectsAreOfTypeDirectoryInfo_ComparesByToString()
        {
            Assert.IsTrue(comparisonSemantics.Equals(
                new DirectoryInfo("a\\directory"),
                new DirectoryInfo("a\\directory")));

            Assert.IsFalse(comparisonSemantics.Equals(
                new DirectoryInfo("a\\directory"),
                new DirectoryInfo("directory")));
        }

        [Test]
        [Row(true, typeof(int[]))]
        [Row(false, typeof(int[,]))]
        [Row(true, typeof(List<int>))]
        [Row(true, typeof(LinkedList<int>))]
        [Row(true, typeof(ArrayList))]
        [Row(true, typeof(Hashtable))]
        [Row(true, typeof(Dictionary<int, string>))]
        [Row(false, typeof(object))]
        [Row(false, typeof(string))]
        public void IsSimpleEnumerableType(bool expectedResult, Type t)
        {
            Assert.AreEqual(expectedResult, comparisonSemantics.IsSimpleEnumerableType(t));

            // do it again, to ensuring caching does not produce disastrous results
            Assert.AreEqual(expectedResult, comparisonSemantics.IsSimpleEnumerableType(t));
        }

        [Test]
        public void CompareConsidersReferentiallyIdenticalObjectstoBeEqual()
        {
            var a = new object();
            Assert.AreEqual(0, comparisonSemantics.Compare(a, a));
        }

        [Test]
        public void CompareConsidersNullsEqual()
        {
            Assert.AreEqual(0, comparisonSemantics.Compare<object>(null, null));
        }

        [Test]
        public void CompareConsidersNullSmallerThanCollections()
        {
            Assert.AreEqual(-1, comparisonSemantics.Compare(null, new int[] { }));
            Assert.AreEqual(1, comparisonSemantics.Compare(new int[] { }, null));
        }

        [Test]
        public void CompareComparesSimpleEnumerablesRecursivelyByContent()
        {
            Assert.AreEqual(0, comparisonSemantics.Compare(new List<int[]> { new[] { 1, 2 }, new[] { 9, 10 } },
                new List<int[]> { new[] { 1, 2 }, new[] { 9, 10 } }));
            Assert.AreEqual(1, comparisonSemantics.Compare(new List<int[]> { new[] { 1, 2 }, new[] { 9, 11 } },
                new List<int[]> { new[] { 1, 2 }, new[] { 9, 10 } }));
            Assert.AreEqual(1, comparisonSemantics.Compare(new List<int[]> { new[] { 1, 2 }, new[] { 9, 10 } },
                new List<int[]> { new[] { 1, 2 } }));
            Assert.AreEqual(-1, comparisonSemantics.Compare(new List<int[]> { new[] { 1, 2 }, new[] { 9, 10 } },
                new List<int[]> { new[] { 1, 2 }, new[] { 9, 10, 11 } }));
        }

        [Test]
        public void CompareUsesIComparableWhenAvailable()
        {
            Assert.AreEqual(0, comparisonSemantics.Compare(new ComparableStub(42), new ComparableStub(42)));
            Assert.AreEqual(1, comparisonSemantics.Compare(new ComparableStub(56), new ComparableStub(42)));
            Assert.AreEqual(-1, comparisonSemantics.Compare(new ComparableStub(32), new ComparableStub(42)));
            Assert.AreEqual(1, comparisonSemantics.Compare(new ComparableStub(42), null));
            Assert.AreEqual(-1, comparisonSemantics.Compare(new ComparableStub(42), new object()));
            Assert.AreEqual(-1, comparisonSemantics.Compare(null, new ComparableStub(42)));
            Assert.AreEqual(1, comparisonSemantics.Compare(new object(), new ComparableStub(42)));
        }

        [Test]
        public void CompareUsesGenericIComparableWhenAvailable()
        {
            Assert.AreEqual(0, comparisonSemantics.Compare(new GenericComparableStub(42), new GenericComparableStub(42)));
            Assert.AreEqual(1, comparisonSemantics.Compare(new GenericComparableStub(56), new GenericComparableStub(42)));
            Assert.AreEqual(-1, comparisonSemantics.Compare(new GenericComparableStub(32), new GenericComparableStub(42)));
            Assert.AreEqual(1, comparisonSemantics.Compare(new GenericComparableStub(42), null));
            Assert.AreEqual(-1, comparisonSemantics.Compare(null, new GenericComparableStub(42)));
        }

        [Test]
        public void CompareCanFindAppropriateUnificationOfenericIComparableWhenAvailable()
        {
            Assert.AreEqual(0, comparisonSemantics.Compare<object>(new GenericComparableStub(42), new GenericComparableStub(42)));
            Assert.AreEqual(1, comparisonSemantics.Compare<object>(new GenericComparableStub(56), new GenericComparableStub(42)));
            Assert.AreEqual(-1, comparisonSemantics.Compare<object>(new GenericComparableStub(32), new GenericComparableStub(42)));
            Assert.AreEqual(1, comparisonSemantics.Compare<object>(new GenericComparableStub(42), null));
            Assert.AreEqual(-1, comparisonSemantics.Compare<object>(null, new GenericComparableStub(42)));
        }

        [Test]
        public void CompareCanHandleCasesWhereIComparableIsNotImplementedReflexively()
        {
            Assert.AreEqual(0, comparisonSemantics.Compare(new NonReflexiveGenericComparableStub(42), new NonReflexiveGenericComparableStub(42)));
            Assert.AreEqual(1, comparisonSemantics.Compare(new NonReflexiveGenericComparableStub(56), new NonReflexiveGenericComparableStub(42)));
            Assert.AreEqual(-1, comparisonSemantics.Compare(new NonReflexiveGenericComparableStub(32), new NonReflexiveGenericComparableStub(42)));
            Assert.AreEqual(1, comparisonSemantics.Compare(new NonReflexiveGenericComparableStub(42), null));
            Assert.AreEqual(-1, comparisonSemantics.Compare<object>(new NonReflexiveGenericComparableStub(42), new Base()));
            Assert.AreEqual(-1, comparisonSemantics.Compare(null, new NonReflexiveGenericComparableStub(42)));
            Assert.AreEqual(1, comparisonSemantics.Compare<object>(new Base(), new NonReflexiveGenericComparableStub(42)));
        }

        [Test, ExpectedException(typeof(InvalidOperationException))]
        public void CompareThrowsIfNoComparisonIsAvailable()
        {
            comparisonSemantics.Compare(new object(), new object());
        }

        [Test]
        [Row(typeof(int), 2, 1, 0, false)]
        [Row(typeof(int), 1, 2, 0, false)]
        [Row(typeof(int), 2, 1, 1, true)]
        [Row(typeof(int), 1, 2, 1, true)]
        [Row(typeof(int), 2, 1, 2, true)]
        [Row(typeof(int), 1, 2, 2, true)]
        [Row(typeof(double), 1.2, 1.24, 0.01, false)]
        [Row(typeof(double), 1.24, 1.2, 0.01, false)]
        [Row(typeof(double), 1.2, 1.24, 0.05, true)]
        [Row(typeof(double), 1.24, 1.2, 0.05, true)]
        [Row(typeof(float), -1.1f, -1.05f, 0.01f, false)]
        [Row(typeof(float), -1.1f, -1.05f, 0.1f, true)]
        [Row(typeof(decimal), 1.1, 1.05, 0.01, false)]
        [Row(typeof(decimal), 1.1, 1.05, 0.1, true)]
        public void ApproximatelyEqual_NumericTypes<T>(T left, T right, T delta, bool expectedResult)
        {
            Assert.AreEqual(expectedResult, comparisonSemantics.ApproximatelyEqual(left, right, delta));
        }

        [Test]
        public void ApproximatelyEqual_DateTime()
        {
            Assert.AreEqual(true, comparisonSemantics.ApproximatelyEqual(
                new DateTime(2008, 03, 14), new DateTime(2008, 03, 15), new TimeSpan(1, 0, 0, 0)));
            Assert.AreEqual(true, comparisonSemantics.ApproximatelyEqual(
                new DateTime(2008, 03, 14), new DateTime(2008, 03, 15), new TimeSpan(2, 0, 0, 0)));
            Assert.AreEqual(false, comparisonSemantics.ApproximatelyEqual(
                new DateTime(2008, 03, 14), new DateTime(2008, 03, 15), new TimeSpan(0, 23, 0, 0)));
        }

        [Test]
        [Row(typeof(SByte), typeof(Int32))]
        [Row(typeof(Byte), typeof(Int32))]
        [Row(typeof(Int16), typeof(Int32))]
        [Row(typeof(UInt16), typeof(Int32))]
        [Row(typeof(Int32), typeof(Int32))]
        [Row(typeof(UInt32), typeof(UInt32))]
        [Row(typeof(Int64), typeof(Int64))]
        [Row(typeof(UInt64), typeof(UInt64))]
        [Row(typeof(Single), typeof(Single))]
        [Row(typeof(Double), typeof(Double))]
        [Row(typeof(Char), typeof(Int32))]
        public void GetSubtractionFunc_IsDefinedForPrimitiveTypes<T, D>()
        {
            Assert.DoesNotThrow(() => comparisonSemantics.GetSubtractionFunc<T, D>());
        }

        [Test]
        public void GetSubtractionFunc_Double()
        {
            SubtractionFunc<double, double> differencer = comparisonSemantics.GetSubtractionFunc<double, double>();
            Assert.AreEqual(1.5, differencer(3.5, 2.0));
        }

        [Test]
        public void GetSubtractionFunc_DateTime()
        {
            SubtractionFunc<DateTime, TimeSpan> differencer = comparisonSemantics.GetSubtractionFunc<DateTime, TimeSpan>();
            Assert.AreEqual(new TimeSpan(-1, 0, 0, 0), differencer(new DateTime(2008, 03, 14), new DateTime(2008, 03, 15)));
        }

        [Test]
        public void GetSubtractionFunc_ThrowsIfNoOperator()
        {
            Assert.Throws<InvalidOperationException>(() => comparisonSemantics.GetSubtractionFunc<Object, Object>());
        }

        [Test]
        public void GetSubtractionFunc_ThrowsIfWrongDifferenceTyp()
        {
            Assert.Throws<InvalidOperationException>(() => comparisonSemantics.GetSubtractionFunc<DateTime, Double>());
        }

        [Test]
        [Row(123, 456, false)]
        [Row(123, 123, true)]
        [Row(456, 123, false)]
        public void Equals_with_a_custom_comparer(int value1, int value2, bool expectedEqual)
        {
            var a = new NonComparableStub(value1);
            var b = new NonComparableStub(value2);
            extensionPoints.CustomEqualityComparers.Register<NonComparableStub>((x, y) => x.Value == y.Value);
            bool actualEqual = comparisonSemantics.Equals(a, b);
            Assert.AreEqual(expectedEqual, actualEqual);
            extensionPoints.CustomEqualityComparers.Unregister<NonComparableStub>();
        }

        [Test]
        [Row(123, 456, -1)]
        [Row(123, 123, 0)]
        [Row(456, 123, 1)]
        public void Compares_with_a_custom_comparer(int value1, int value2, int expectedSign)
        {
            var a = new NonComparableStub(value1);
            var b = new NonComparableStub(value2);
            extensionPoints.CustomComparers.Register<NonComparableStub>((x, y) => x.Value.CompareTo(y.Value));
            int actualSign = comparisonSemantics.Compare(a, b);
            Assert.AreEqual(expectedSign, actualSign, (x, y) => Math.Sign(x) == Math.Sign(y));
            extensionPoints.CustomComparers.Unregister<NonComparableStub>();
        }

        private sealed class ComparableStub : IComparable
        {
            private readonly int value;

            public ComparableStub(int value)
            {
                this.value = value;
            }

            public int CompareTo(object obj)
            {
                if (obj == null)
                    return 1;

                var other = obj as ComparableStub;
                if (other != null)
                    return value.CompareTo(other.value);

                return -1;
            }
        }

        private sealed class GenericComparableStub : IComparable<GenericComparableStub>
        {
            private readonly int value;

            public GenericComparableStub(int value)
            {
                this.value = value;
            }

            public int CompareTo(GenericComparableStub obj)
            {
                if (obj == null)
                    return 1;

                return value.CompareTo(obj.value);
            }
        }

        private class Base
        {
        }

        private sealed class NonReflexiveGenericComparableStub : Base, IComparable<Base>
        {
            private readonly int value;

            public NonReflexiveGenericComparableStub(int value)
            {
                this.value = value;
            }

            public int CompareTo(Base obj)
            {
                if (obj == null)
                    return 1;

                var other = obj as NonReflexiveGenericComparableStub;
                if (other != null)
                    return value.CompareTo(other.value);

                return -1;
            }
        }

        internal class NonComparableStub
        {
            private readonly int value;

            public int Value
            {
                get
                {
                    return value;
                }
            }

            public NonComparableStub(int value)
            {
                this.value = value;
            }
        }

    }
}
