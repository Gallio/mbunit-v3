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
using System.Text;
using MbUnit.Framework;

namespace MbUnit.Samples.CustomComparers
{
    public class NonComparable
    {
        private readonly int value;

        public int Value
        {
            get
            {
                return value;
            }
        }

        public NonComparable(int value)
        {
            this.value = value;
        }
    }

    [TestFixture]
    public class CustomComparerTest
    {
        [Comparer]
        public static int Compare(NonComparable x, NonComparable y)
        {
            // The inner comparison engine of Gallio handles with the cases
            // where x or y is null. Therefore, we can safely assume than x
            // and y are never null here.

            return x.Value.CompareTo(y.Value); // Custom comparison logic.
        }
        
        [Test]
        public void Test()
        {
            var item1 = new NonComparable(123);
            var item2 = new NonComparable(456);
            var item3 = new NonComparable(789);
            Assert.GreaterThan(item2, item1); // The assertions will use the custom comparer defined above.
            Assert.LessThan(item2, item3);
        }
    }
}
