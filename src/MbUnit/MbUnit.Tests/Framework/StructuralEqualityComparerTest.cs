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
using Gallio.Framework;
using Gallio.Framework.Assertions;
using Gallio.Model;
using Gallio.Common.Diagnostics;
using MbUnit.Framework;

namespace MbUnit.Tests.Framework
{
    [TestFixture]
    public class StructuralEqualityComparerTest
    {
        [Test]
        [Row(123, 123, true)]
        [Row(123, 456, false)]
        public void Compare_Int32_default(int value1, int value2, bool expected)
        {
            var comparer = StructuralEqualityComparer<int>.Default;
            bool result = comparer.Equals(value1, value2);
            Assert.AreEqual(expected, result);
        }

        [Test]
        [Row(1, 1, true)]
        [Row(1, 3, true)]
        [Row(123, 789, true)]
        [Row(2, 2, true)]
        [Row(2, 4, true)]
        [Row(256, 512, true)]
        [Row(1, 2, false)]
        [Row(101, 1000, false)]
        public void Compare_Int32_parity(int value1, int value2, bool expected)
        {
            var comparer = new StructuralEqualityComparer<int>
            {
                { x => x % 2, (x, y) => x == y }
            };

            bool result = comparer.Equals(value1, value2);
            Assert.AreEqual(expected, result);
        }

        [Test]
        [Row("abcXXX", "abcXXX", true)]
        [Row("abcXXX", "abcYYY", true)]
        [Row("abcXXX", "ABCXYZ", true)]
        [Row("aBcXXX", "abCXYZ", true)]
        [Row("abcXXX", "abcXY", false)]
        [Row("abcXXX", "abdXYZ", false)]
        public void Compare_String_length_and_first_characters(string value1, string value2, bool expected)
        {
            var comparer = new StructuralEqualityComparer<string>
            {
                { x => x.Length, (x, y) => x == y },
                { x => x.Substring(0, 3), (x, y) => String.Compare(x, y, true) == 0 }
            };

            bool result = comparer.Equals(value1, value2);
            Assert.AreEqual(expected, result);
        }

        public class Foo
        {
            public int Number;
            public string Text;
            public TimeSpan Duration;
            public int[] Values;
            public ChildFoo[] Children;
        }

        public class ChildFoo
        {
            public int Tag;
            public string Name;
        }

        public IEnumerable<object[]> ProvideTestData1()
        {
            yield return new object[] 
            {
                new Foo() { Number = 123, Text = "Hello", Duration = TimeSpan.FromMinutes(600) },
                new Foo() { Number = 123, Text = "Hello", Duration = TimeSpan.FromMinutes(600) },
                true
            };

            yield return new object[] 
            {
                new Foo() { Number = 123, Text = "Hello", Duration = TimeSpan.FromMinutes(600) },
                new Foo() { Number = 123, Text = "HELLO", Duration = TimeSpan.FromMinutes(600) },
                true
            };

            yield return new object[] 
            {
                new Foo() { Number = 123, Text = "Hello", Duration = TimeSpan.FromMinutes(600) },
                new Foo() { Number = 123, Text = "Hello", Duration = TimeSpan.FromMinutes(630) },
                true
            };

            yield return new object[] 
            {
                new Foo() { Number = 123, Text = "Hello", Duration = TimeSpan.FromMinutes(600) },
                new Foo() { Number = 456, Text = "Hello", Duration = TimeSpan.FromMinutes(600) },
                false
            };

            yield return new object[] 
            {
                new Foo() { Number = 123, Text = "Hello", Duration = TimeSpan.FromMinutes(600) },
                new Foo() { Number = 123, Text = "abcde", Duration = TimeSpan.FromMinutes(600) },
                false
            };

            yield return new object[] 
            {
                new Foo() { Number = 123, Text = "Hello", Duration = TimeSpan.FromMinutes(600) },
                new Foo() { Number = 123, Text = "Hello", Duration = TimeSpan.FromMinutes(661) },
                false
            };
        }

        [Test]
        [Factory("ProvideTestData1")]
        public void Nested_comparers_on_complex_type(Foo value1, Foo value2, bool expected)
        {
            var comparer = new StructuralEqualityComparer<Foo>
            {
                { x => x.Number },
                
                { (x, y) => String.Compare(x.Text, y.Text, true) == 0 },

                { x => x.Duration, 
                    new StructuralEqualityComparer<TimeSpan>
                    {
                        { x => x.TotalHours, (x, y) => Math.Abs(x - y) <= 1 }
                    }
                }
            };

            bool result = comparer.Equals(value1, value2);
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void Compare_null_references()
        {
            var comparer = new StructuralEqualityComparer<Foo>
            {
                { x => x.Number },
            };

            var foo = new Foo() { Number = 123, Text = "Hello", Duration = TimeSpan.FromMinutes(600) };

            Assert.IsFalse(comparer.Equals(foo, null));
            Assert.IsFalse(comparer.Equals(null, foo));
            Assert.IsTrue(comparer.Equals(null, null));
        }


        public IEnumerable<object[]> ProvideTestData2()
        {
            yield return new object[] 
            {
                new Foo() { Number = 123, Values = new int[] { 1, 2, 3, 4, 5} },
                new Foo() { Number = 123, Values = new int[] { 1, 2, 3, 4, 5} },
                true
            };

            yield return new object[] 
            {
                new Foo() { Number = 123, Values = new int[] { 1, 2, 3, 4, 5} },
                new Foo() { Number = 123, Values = new int[] { 1, 2, 99, 4, 5} },
                false
            };

            yield return new object[] 
            {
                new Foo() { Number = 123, Values = new int[] { 1, 2, 3, 4, 5} },
                new Foo() { Number = 123, Values = new int[] { 1, 2, 3, 4, 5, 6} },
                false
            };
        }

        [Test]
        [Factory("ProvideTestData2")]
        public void Strict_nested_comparer_on_child_enumeration(Foo value1, Foo value2, bool expected)
        {
            var comparer = new StructuralEqualityComparer<Foo>
            {
                { x => x.Number },
                { x => x.Values,  StructuralEqualityComparer<int>.Default }
            };

            bool result = comparer.Equals(value1, value2);
            Assert.AreEqual(expected, result);
        }

        public IEnumerable<object[]> ProvideTestData3()
        {
            yield return new object[] 
            {
                new Foo() { Number = 123, Values = new int[] { 1, 2, 3, 4, 5} },
                new Foo() { Number = 123, Values = new int[] { 1, 2, 3, 4, 5} },
                true
            };

            yield return new object[] 
            {
                new Foo() { Number = 123, Values = new int[] { 1, 2, 3, 4, 5} },
                new Foo() { Number = 123, Values = new int[] { 3, 2, 4, 5, 1} },
                true
            };

            yield return new object[] 
            {
                new Foo() { Number = 123, Values = new int[] { 1, 2, 3, 4, 5, 6} },
                new Foo() { Number = 123, Values = new int[] { 1, 2, 3, 4, 5} },
                false
            };
        }

        [Test]
        [Factory("ProvideTestData3")]
        public void Nested_comparer_on_child_enumeration_ignoring_order(Foo value1, Foo value2, bool expected)
        {
            var comparer = new StructuralEqualityComparer<Foo>
            {
                { x => x.Number },
                { x => x.Values,  StructuralEqualityComparer<int>.Default, StructuralEqualityComparerOptions.IgnoreEnumerableOrder }
            };

            bool result = comparer.Equals(value1, value2);
            Assert.AreEqual(expected, result);
        }

        public IEnumerable<object[]> ProvideTestData4()
        {
            yield return new object[] 
            {
                new Foo() 
                { 
                    Number = 123, 
                    Text = "Hello", 
                    Values = new int[] { 1, 2, 3, 4, 5 } ,
                    Children = new[] 
                    { 
                        new ChildFoo() { Tag = 1, Name = "ABC" },
                        new ChildFoo() { Tag = 2, Name = "DEF" },
                        new ChildFoo() { Tag = 3, Name = "GHI" }
                    }
                },
                
                new Foo() 
                { 
                    Number = 321, 
                    Text = "HELLO", 
                    Values = new int[] { 5, 4, 3, 2, 1 } ,
                    Children = new[] 
                    { 
                        new ChildFoo() { Tag = 3, Name = "JKL" },
                        new ChildFoo() { Tag = 1, Name = "MNO" },
                        new ChildFoo() { Tag = 2, Name = "PQR" }
                    }
                },

                true
            };

            yield return new object[] 
            {
                new Foo() 
                { 
                    Number = 123, 
                    Text = "Hello", 
                    Values = new int[] { 1, 2, 3, 4, 5 } ,
                    Children = new[] 
                    { 
                        new ChildFoo() { Tag = 1, Name = "ABC" },
                        new ChildFoo() { Tag = 2, Name = "DEF" },
                        new ChildFoo() { Tag = 3, Name = "GHI" }
                    }
                },
                
                new Foo() 
                { 
                    Number = 321, 
                    Text = "HELLO", 
                    Values = new int[] { 5, 4, 3, 2, 1 } ,
                    Children = new[] 
                    { 
                        new ChildFoo() { Tag = 3, Name = "JKL" },
                        new ChildFoo() { Tag = 1, Name = "MNO" },
                        new ChildFoo() { Tag = 4, Name = "PQR" }
                    }
                },

                false,
            };

            yield return new object[] 
            {
                new Foo() 
                { 
                    Number = 123, 
                    Text = "Hello", 
                    Values = new int[] { 1, 2, 3, 4, 5 } ,
                    Children = new[] 
                    { 
                        new ChildFoo() { Tag = 1, Name = "ABC" },
                        new ChildFoo() { Tag = 2, Name = "DEF" },
                        new ChildFoo() { Tag = 3, Name = "GHI" }
                    }
                },
                
                new Foo() 
                { 
                    Number = 252, 
                    Text = "HELLO", 
                    Values = new int[] { 5, 4, 3, 2, 1 } ,
                    Children = new[] 
                    { 
                        new ChildFoo() { Tag = 3, Name = "JKL" },
                        new ChildFoo() { Tag = 1, Name = "MNO" },
                        new ChildFoo() { Tag = 2, Name = "PQR" }
                    }
                },

                false,
            };

            yield return new object[] 
            {
                new Foo() 
                { 
                    Number = 123, 
                    Text = "Hello", 
                    Values = new int[] { 1, 2, 3, 4, 5 } ,
                    Children = new[] 
                    { 
                        new ChildFoo() { Tag = 1, Name = "ABC" },
                        new ChildFoo() { Tag = 2, Name = "DEF" },
                        new ChildFoo() { Tag = 3, Name = "GHI" }
                    }
                },
                
                new Foo() 
                { 
                    Number = 321, 
                    Text = "Hello!", 
                    Values = new int[] { 5, 4, 3, 2, 1 } ,
                    Children = new[] 
                    { 
                        new ChildFoo() { Tag = 3, Name = "JKL" },
                        new ChildFoo() { Tag = 1, Name = "MNO" },
                        new ChildFoo() { Tag = 4, Name = "PQR" }
                    }
                },

                false,
            };
        }

        [Test]
        [Factory("ProvideTestData4")]
        public void Complex_comparers_with_nested_enumerations(Foo value1, Foo value2, bool expected)
        {
            var comparer = new StructuralEqualityComparer<Foo>
            {
                { x => x.Number % 2 },
                { x => x.Text, (x, y) => String.Compare(x, y, true) == 0 },
                { x => x.Values, StructuralEqualityComparer<int>.Default, StructuralEqualityComparerOptions.IgnoreEnumerableOrder },
                { x => x.Children, new StructuralEqualityComparer<ChildFoo> { { x => x.Tag } }, StructuralEqualityComparerOptions.IgnoreEnumerableOrder }
            };

            bool result = comparer.Equals(value1, value2);
            Assert.AreEqual(expected, result);
        }
    }
}
