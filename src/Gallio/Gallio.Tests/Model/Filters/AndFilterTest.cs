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

using Gallio.Common.Collections;
using Gallio.Tests;
using MbUnit.Framework;
using Gallio.Model.Filters;
using System;

namespace Gallio.Tests.Model.Filters
{
    [TestFixture]
    [TestsOn(typeof(AndFilter<object>))]
    public class AndFilterTest : BaseTestWithMocks
    {
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_WhenFilterCollectionIsNull_Throws()
        {
            new AndFilter<object>(null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_WhenFilterCollectionContainsNull_Throws()
        {
            new AndFilter<object>(new Filter<object>[] { null });
        }

        [Test]
        [Row(true, new bool[] { })]
        [Row(true, new bool[] { true })]
        [Row(false, new bool[] { false, true })]
        [Row(false, new bool[] { true, true, false })]
        [Row(true, new bool[] { true, true, true })]
        public void IsMatchCombinations(bool expectedMatch, bool[] states)
        {
            Filter<object>[] filters = GenericCollectionUtils.ConvertAllToArray<bool, Filter<object>>(states, delegate(bool state)
            {
                return state ? (Filter<object>)new AnyFilter<object>() : new NoneFilter<object>();
            });

            AndFilter<object> combinator = new AndFilter<object>(filters);
            Assert.AreEqual(expectedMatch, combinator.IsMatch(0));
        }
    }
}
