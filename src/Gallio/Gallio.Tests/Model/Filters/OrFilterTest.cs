// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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
using Gallio.Collections;
using Gallio.Tests;
using MbUnit.Framework;

using Gallio.Model.Filters;

namespace Gallio.Tests.Model.Filters
{
    [TestFixture]
    [TestsOn(typeof(OrFilter<object>))]
    public class OrFilterTest : BaseTestWithMocks
    {
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullArgument()
        {
            new OrFilter<object>(null);
        }

        [Test]
        [Row(true, new bool[] { })]
        [Row(true, new bool[] { true })]
        [Row(true, new bool[] { false, true })]
        [Row(true, new bool[] { true, true, false })]
        [Row(false, new bool[] { false, false, false })]
        public void IsMatchCombinations(bool expectedMatch, bool[] states)
        {
            Filter<object>[] filters = GenericUtils.ConvertAllToArray<bool, Filter<object>>(states, delegate(bool state)
            {
                return state ? (Filter<object>)new AnyFilter<object>() : new NoneFilter<object>();
            });

            OrFilter<object> combinator = new OrFilter<object>(filters);
            Assert.AreEqual(expectedMatch, combinator.IsMatch(null));
        }
    }
}