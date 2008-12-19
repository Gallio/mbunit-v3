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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gallio.Framework.Text;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;

namespace Gallio.Tests.Framework.Text
{
    [TestsOn(typeof(Range))]
    public class RangeTest
    {
        [ContractVerifier]
        public readonly IContractVerifier EqualityTests = new VerifyEqualityContract<Range>()
        {
            ImplementsOperatorOverloads = false,
            EquivalenceClasses = EquivalenceClassCollection<Range>.FromDistinctInstances(
                new Range(0, 0),
                new Range(1, 1),
                new Range(0, 1),
                new Range(1, 0))
        };

        [Test]
        public void ConstructorInitializesProperties()
        {
            Range range = new Range(3, 4);
            Assert.AreEqual(3, range.StartIndex);
            Assert.AreEqual(4, range.Length);
            Assert.AreEqual(7, range.EndIndex);
        }

        [Test]
        public void BetweenInitializesProperties()
        {
            Range range = Range.Between(3, 4);
            Assert.AreEqual(3, range.StartIndex);
            Assert.AreEqual(1, range.Length);
            Assert.AreEqual(4, range.EndIndex);
        }

        [Test]
        [Row(0, -1, ExpectedException=typeof(ArgumentOutOfRangeException))]
        [Row(-1, 0, ExpectedException = typeof(ArgumentOutOfRangeException))]
        public void ConstructorThrowsIfRangeIsInvalid(int startIndex, int length)
        {
            new Range(startIndex, length);
        }

        [Test]
        [Row(0, -1, ExpectedException = typeof(ArgumentOutOfRangeException))]
        [Row(-1, 0, ExpectedException = typeof(ArgumentOutOfRangeException))]
        [Row(2, 1, ExpectedException = typeof(ArgumentOutOfRangeException))]
        public void BetweenThrowsIfRangeIsInvalid(int startIndex, int endIndex)
        {
            Range.Between(startIndex, endIndex);
        }

        [Test]
        public void SubstringOfThrowsIfStringIsNull()
        {
            Range range = new Range(0, 4);
            Assert.Throws<ArgumentNullException>(() => range.SubstringOf(null));
        }

        [Test]
        public void SubstringOfThrowsIfRangeIsOutOfBounds()
        {
            Range range = new Range(3, 4);
            Assert.Throws<ArgumentOutOfRangeException>(() => range.SubstringOf("abcde"));
        }

        [Test]
        public void SubstringOfReturnsStringWhenRangeIsCorrect()
        {
            Assert.AreEqual("cdef", new Range(2, 4).SubstringOf("abcdefg"));
        }

        [Test]
        public void ExtendWithThrowsIfRangeIsDisjoint()
        {
            Range a = new Range(2, 3);
            Range b = new Range(4, 2);
            Assert.Throws<ArgumentException>(() => a.ExtendWith(b));
        }

        [Test]
        public void ExtendWithThrowsIfRangeOverlaps()
        {
            Range a = new Range(2, 3);
            Range b = new Range(3, 1);
            Assert.Throws<ArgumentException>(() => a.ExtendWith(b));
        }

        [Test]
        public void ExtendWithFollowingRange()
        {
            Assert.AreEqual(new Range(2, 3), new Range(2, 1).ExtendWith(new Range(3, 2)));
        }

        [Test]
        public void ExtendWithPrecedingRange()
        {
            Assert.AreEqual(new Range(2, 3), new Range(3, 2).ExtendWith(new Range(2, 1)));
        }

        [Test]
        public void ToStringFormatting()
        {
            Assert.AreEqual("[2 .. 5)", new Range(2, 3).ToString());
        }
    }
}
