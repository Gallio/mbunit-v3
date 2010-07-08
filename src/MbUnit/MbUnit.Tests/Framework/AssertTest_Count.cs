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
using System.Text;
using Gallio.Framework.Assertions;
using MbUnit.Framework;

namespace MbUnit.Tests.Framework
{
    [TestsOn(typeof(Assert))]
	public class AssertTest_Count : BaseAssertTest
	{
        private static IEnumerable GetEnumeration()
        {
            yield return "Tintin in America";
            yield return "Cigars of the Pharaoh";
            yield return "Explorers on the Moon";
            yield return "The Castafiore Emerald";
        }

        private static IEnumerable GetArrayList()
        {
            var collection = new ArrayList();

            foreach (var o in GetEnumeration())
                collection.Add(o);

            return collection;
        }

        private static IEnumerable GetStronglyTypedList()
        {
            return new List<string>(GetEnumeration().Cast<string>());
        }

        private static IEnumerable GetArray()
        {
            return GetEnumeration().Cast<string>().ToArray();
        }

        private static IEnumerable GetHashSet() // Interesting case because it does not implement ICollection!
        {
            return new HashSet<string>(GetEnumeration().Cast<string>());
        }

        private static IEnumerable GetCountableEnumerable()
        {
            return new CountableEnumerable(GetEnumeration().Cast<string>());
        }

        private class CountableEnumerable : IEnumerable<string>
        {
            private readonly List<string> data;

            public CountableEnumerable(IEnumerable<string> values)
            {
                data = values.ToList();
            }

            public int Count
            {
                get
                {
                    return data.Count;
                }
            }

            public IEnumerator<string> GetEnumerator()
            {
                return data.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return data.GetEnumerator();
            }
        }

        private IEnumerable<object[]> DataProvider
        {
            get
            {
                yield return new object[] { GetEnumeration() };
                yield return new object[] { GetArrayList() };
                yield return new object[] { GetStronglyTypedList() };
                yield return new object[] { GetArray() };
                yield return new object[] { GetHashSet() };
                yield return new object[] { GetCountableEnumerable() };
            }
        }

        [Test, Factory("DataProvider")]
        public void Count_pass(IEnumerable values)
        {
            Assert.Count(4, values);
        }

        [Test, Factory("DataProvider")]
        public void Count_fail(IEnumerable values)
        {
            AssertionFailure[] failures = Capture(() => Assert.Count(666, GetEnumeration()));
            Assert.Count(1, failures);
            Assert.Like(failures[0].Description, @"^Expected the (sequence|collection|array) to contain a certain number of elements.$");
            Assert.AreEqual("Expected Value", failures[0].LabeledValues[0].Label);
            Assert.AreEqual("666", failures[0].LabeledValues[0].FormattedValue.ToString());
            Assert.Like(failures[0].LabeledValues[1].Label, @"^Actual Value( \((Count|Length)\))?$");
            Assert.AreEqual("4", failures[0].LabeledValues[1].FormattedValue.ToString());
        }
    }
}
