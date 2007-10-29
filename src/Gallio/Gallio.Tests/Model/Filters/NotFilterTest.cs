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

extern alias MbUnit2;
using Gallio.Tests;
using MbUnit2::MbUnit.Framework;

using Gallio.Model.Filters;

namespace Gallio.Tests.Model.Filters
{
    [TestFixture]
    [TestsOn(typeof(NotFilter<object>))]
    public class NotFilterTest : BaseUnitTest
    {
        [RowTest]
        [Row(true, false)]
        [Row(false, true)]
        public void IsMatchCombinations(bool expectedMatch, bool state)
        {
            Filter<object> filter = state ? (Filter<object>)new AnyFilter<object>() : new NoneFilter<object>();
            NotFilter<object> combinator = new NotFilter<object>(filter);
            Assert.AreEqual(expectedMatch, combinator.IsMatch(null));
        }
    }
}