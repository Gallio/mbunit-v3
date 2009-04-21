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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Gallio.Framework;
using MbUnit.Framework;
using System.IO;

namespace Gallio.Tests.Framework
{
    [TestsOn(typeof(ComparisonSemantics))]
    public class ComparisonSemanticsTest
    {
        [Test]
        public void SameConsidersNullEqual()
        {
            Assert.IsTrue(ComparisonSemantics.Same<object>(null, null));
        }

        [Test]
        public void SameConsidersNullNotEqualToNonNull()
        {
            Assert.IsFalse(ComparisonSemantics.Same<object>(null, "a"));
            Assert.IsFalse(ComparisonSemantics.Same<object>("a", null));
        }

        [Test]
        public void SameUsesReferentialIdentityForReferenceTypes()
        {
            object a = new object();
            object b = new object();

            Assert.IsTrue(ComparisonSemantics.Same(a, a));
            Assert.IsFalse(ComparisonSemantics.Same(a, b));
        }

        [Test]
        public void EqualsConsidersNullsEqual()
        {
            Assert.IsTrue(ComparisonSemantics.Equals<object>(null, null));
        }

        [Test]
        public void EqualsConsidersReferentiallyIdenticalObjectstoBeEqual()
        {
            object a = new object();
            Assert.IsTrue(ComparisonSemantics.Equals(a, a));
        }

        [Test]
        public void EqualsConsidersNullAndNonNullValuesDistinct()
        {
            Assert.IsFalse(ComparisonSemantics.Equals<object>(null, "a"));
            Assert.IsFalse(ComparisonSemantics.Equals<object>("a", null));
        }

        [Test]
        public void EqualsComparesValuesUsingObjectEquality()
        {
            Assert.IsTrue(ComparisonSemantics.Equals("a", "a"));
            Assert.IsTrue(ComparisonSemantics.Equals("a", "A".ToLowerInvariant()));
            Assert.IsTrue(ComparisonSemantics.Equals(1, 1));
            Assert.IsFalse(ComparisonSemantics.Equals("a", "b"));
            Assert.IsFalse(ComparisonSemantics.Equals(1, 2));
        }

        [Test]
        public void EqualsComparesArraysByContent()
        {
            Assert.IsTrue(ComparisonSemantics.Equals(new[] { 2, 3, 5, 7 }, new[] { 2, 3, 5, 7 }));
            Assert.IsFalse(ComparisonSemantics.Equals(new[] { 2, 3, 5 }, new[] { 2, 3, 5, 7 }));
            Assert.IsFalse(ComparisonSemantics.Equals(new[] { 2, 3, 5, 7 }, new[] { 2, 3, 5 }));
            Assert.IsFalse(ComparisonSemantics.Equals(new int[] { }, new[] { 2, 3, 5 }));
            Assert.IsFalse(ComparisonSemantics.Equals(new[] { 2, 3, 5 }, new int[] { }));
            Assert.IsTrue(ComparisonSemantics.Equals(new int[] { }, new int[] { }));
        }

        [Test]
        public void EqualsComparesSimpleEnumerablesRecursivelyByContent()
        {
            Assert.IsTrue(ComparisonSemantics.Equals(new List<int[]> { new[] { 1, 2 }, new[] { 9, 10 } },
                new List<int[]> { new[] { 1, 2 }, new[] { 9, 10 } }));
            Assert.IsFalse(ComparisonSemantics.Equals(new List<int[]> { new[] { 1, 2 }, new[] { 9, 11 } },
                new List<int[]> { new[] { 1, 2 }, new[] { 9, 10 } }));
            Assert.IsFalse(ComparisonSemantics.Equals(new List<int[]> { new[] { 1, 2 }, new[] { 9, 10 } },
                new List<int[]> { new[] { 1, 2 } }));
            Assert.IsFalse(ComparisonSemantics.Equals(new List<int[]> { new[] { 1, 2 }, new[] { 9, 10 } },
                new List<int[]> { new[] { 1, 2 }, new[] { 9, 10, 11 } }));
        }

        [Test]
        public void EqualsComparesSimpleEnumerablesDistinctIfTheyHaveDifferentTypes()
        {
            Assert.IsFalse(ComparisonSemantics.Equals(new List<int> { 1, 2 }, new[] { 1, 2}));
        }

        [Test]
        public void Equals_WhenObjectsAreOfTypeAssemblyName_ComparesByFullName()
        {
            Assert.IsTrue(ComparisonSemantics.Equals(
                new AssemblyName("Gallio, Version=0.0.0.0"),
                new AssemblyName("Gallio, Version=0.0.0.0")));

            Assert.IsFalse(ComparisonSemantics.Equals(
                new AssemblyName("Gallio, Version=0.0.0.0"),
                new AssemblyName("Gallio")));
        }

        [Test]
        public void Equals_WhenObjectsAreOfTypeFileInfo_ComparesByToString()
        {
            Assert.IsTrue(ComparisonSemantics.Equals(
                new FileInfo("a\\file.txt"),
                new FileInfo("a\\file.txt")));

            Assert.IsFalse(ComparisonSemantics.Equals(
                new FileInfo("a\\file.txt"),
                new FileInfo("file.txt")));
        }

        [Test]
        public void Equals_WhenObjectsAreOfTypeDirectoryInfo_ComparesByToString()
        {
            Assert.IsTrue(ComparisonSemantics.Equals(
                new DirectoryInfo("a\\directory"),
                new DirectoryInfo("a\\directory")));

            Assert.IsFalse(ComparisonSemantics.Equals(
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
            Assert.AreEqual(expectedResult, ComparisonSemantics.IsSimpleEnumerableType(t));

            // do it again, to ensuring caching does not produce disastrous results
            Assert.AreEqual(expectedResult, ComparisonSemantics.IsSimpleEnumerableType(t));
        }

        [Test]
        public void CompareConsidersReferentiallyIdenticalObjectstoBeEqual()
        {
            object a = new object();
            Assert.AreEqual(0, ComparisonSemantics.Compare(a, a));
        }

        [Test]
        public void CompareConsidersNullsEqual()
        {
            Assert.AreEqual(0, ComparisonSemantics.Compare<object>(null, null));
        }

        [Test]
        public void CompareConsidersNullSmallerThanCollections()
        {
            Assert.AreEqual(-1, ComparisonSemantics.Compare(null, new int[] { }));
            Assert.AreEqual(1, ComparisonSemantics.Compare(new int[] { }, null));
        }

        [Test]
        public void CompareComparesSimpleEnumerablesRecursivelyByContent()
        {
            Assert.AreEqual(0, ComparisonSemantics.Compare(new List<int[]> { new[] { 1, 2 }, new[] { 9, 10 } },
                new List<int[]> { new[] { 1, 2 }, new[] { 9, 10 } }));
            Assert.AreEqual(1, ComparisonSemantics.Compare(new List<int[]> { new[] { 1, 2 }, new[] { 9, 11 } },
                new List<int[]> { new[] { 1, 2 }, new[] { 9, 10 } }));
            Assert.AreEqual(1, ComparisonSemantics.Compare(new List<int[]> { new[] { 1, 2 }, new[] { 9, 10 } },
                new List<int[]> { new[] { 1, 2 } }));
            Assert.AreEqual(-1, ComparisonSemantics.Compare(new List<int[]> { new[] { 1, 2 }, new[] { 9, 10 } },
                new List<int[]> { new[] { 1, 2 }, new[] { 9, 10, 11 } }));
        }

        [Test]
        public void CompareUsesIComparableWhenAvailable()
        {
            Assert.AreEqual(0, ComparisonSemantics.Compare(new ComparableStub(42), new ComparableStub(42)));
            Assert.AreEqual(1, ComparisonSemantics.Compare(new ComparableStub(56), new ComparableStub(42)));
            Assert.AreEqual(-1, ComparisonSemantics.Compare(new ComparableStub(32), new ComparableStub(42)));
            Assert.AreEqual(1, ComparisonSemantics.Compare(new ComparableStub(42), null));
            Assert.AreEqual(-1, ComparisonSemantics.Compare(new ComparableStub(42), new object()));
            Assert.AreEqual(-1, ComparisonSemantics.Compare(null, new ComparableStub(42)));
            Assert.AreEqual(1, ComparisonSemantics.Compare(new object(), new ComparableStub(42)));
        }

        [Test]
        public void CompareUsesGenericIComparableWhenAvailable()
        {
            Assert.AreEqual(0, ComparisonSemantics.Compare(new GenericComparableStub(42), new GenericComparableStub(42)));
            Assert.AreEqual(1, ComparisonSemantics.Compare(new GenericComparableStub(56), new GenericComparableStub(42)));
            Assert.AreEqual(-1, ComparisonSemantics.Compare(new GenericComparableStub(32), new GenericComparableStub(42)));
            Assert.AreEqual(1, ComparisonSemantics.Compare(new GenericComparableStub(42), null));
            Assert.AreEqual(-1, ComparisonSemantics.Compare(null, new GenericComparableStub(42)));
        }

        [Test]
        public void CompareCanFindAppropriateUnificationOfenericIComparableWhenAvailable()
        {
            Assert.AreEqual(0, ComparisonSemantics.Compare<object>(new GenericComparableStub(42), new GenericComparableStub(42)));
            Assert.AreEqual(1, ComparisonSemantics.Compare<object>(new GenericComparableStub(56), new GenericComparableStub(42)));
            Assert.AreEqual(-1, ComparisonSemantics.Compare<object>(new GenericComparableStub(32), new GenericComparableStub(42)));
            Assert.AreEqual(1, ComparisonSemantics.Compare<object>(new GenericComparableStub(42), null));
            Assert.AreEqual(-1, ComparisonSemantics.Compare<object>(null, new GenericComparableStub(42)));
        }

        [Test]
        public void CompareCanHandleCasesWhereIComparableIsNotImplementedReflexively()
        {
            Assert.AreEqual(0, ComparisonSemantics.Compare(new NonReflexiveGenericComparableStub(42), new NonReflexiveGenericComparableStub(42)));
            Assert.AreEqual(1, ComparisonSemantics.Compare(new NonReflexiveGenericComparableStub(56), new NonReflexiveGenericComparableStub(42)));
            Assert.AreEqual(-1, ComparisonSemantics.Compare(new NonReflexiveGenericComparableStub(32), new NonReflexiveGenericComparableStub(42)));
            Assert.AreEqual(1, ComparisonSemantics.Compare(new NonReflexiveGenericComparableStub(42), null));
            Assert.AreEqual(-1, ComparisonSemantics.Compare<object>(new NonReflexiveGenericComparableStub(42), new Base()));
            Assert.AreEqual(-1, ComparisonSemantics.Compare(null, new NonReflexiveGenericComparableStub(42)));
            Assert.AreEqual(1, ComparisonSemantics.Compare<object>(new Base(), new NonReflexiveGenericComparableStub(42)));
        }

        [Test, ExpectedException(typeof(InvalidOperationException))]
        public void CompareThrowsIfNoComparisonIsAvailable()
        {
            ComparisonSemantics.Compare(new object(), new object());
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
            Assert.AreEqual(expectedResult, ComparisonSemantics.ApproximatelyEqual(left, right, delta));
        }

        [Test]
        public void ApproximatelyEqual_DateTime()
        {
            Assert.AreEqual(true, ComparisonSemantics.ApproximatelyEqual(
                new DateTime(2008, 03, 14), new DateTime(2008, 03, 15), new TimeSpan(1, 0, 0, 0)));
            Assert.AreEqual(true, ComparisonSemantics.ApproximatelyEqual(
                new DateTime(2008, 03, 14), new DateTime(2008, 03, 15), new TimeSpan(2, 0, 0, 0)));
            Assert.AreEqual(false, ComparisonSemantics.ApproximatelyEqual(
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
            Assert.DoesNotThrow(() => ComparisonSemantics.GetSubtractionFunc<T, D>());
        }

        [Test]
        public void GetSubtractionFunc_Double()
        {
            SubtractionFunc<double, double> differencer = ComparisonSemantics.GetSubtractionFunc<double, double>();
            Assert.AreEqual(1.5, differencer(3.5, 2.0));
        }

        [Test]
        public void GetSubtractionFunc_DateTime()
        {
            SubtractionFunc<DateTime, TimeSpan> differencer = ComparisonSemantics.GetSubtractionFunc<DateTime, TimeSpan>();
            Assert.AreEqual(new TimeSpan(-1, 0, 0, 0), differencer(new DateTime(2008, 03, 14), new DateTime(2008, 03, 15)));
        }

        [Test]
        public void GetSubtractionFunc_ThrowsIfNoOperator()
        {
            Assert.Throws<InvalidOperationException>(() => ComparisonSemantics.GetSubtractionFunc<Object, Object>());
        }

        [Test]
        public void GetSubtractionFunc_ThrowsIfWrongDifferenceTyp()
        {
            Assert.Throws<InvalidOperationException>(() => ComparisonSemantics.GetSubtractionFunc<DateTime, Double>());
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

                ComparableStub other = obj as ComparableStub;
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

                NonReflexiveGenericComparableStub other = obj as NonReflexiveGenericComparableStub;
                if (other != null)
                    return value.CompareTo(other.value);

                return -1;
            }
        }
    }
}
