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
    [TestsOn(typeof(Substring))]
    public class SubstringTest
    {
        [ContractVerifier]
        public readonly IContractVerifier EqualityTests = new VerifyEqualityContract<Substring>()
        {
            ImplementsOperatorOverloads = false,
            EquivalenceClasses = new EquivalenceClassCollection<Substring>(
                new EquivalenceClass<Substring>(new Substring("bcd"), new Substring("abcde", new Range(1, 3))),
                new EquivalenceClass<Substring>(new Substring(""), new Substring("abcde", new Range(3, 0))),
                new EquivalenceClass<Substring>(new Substring("12345"), new Substring("9912345", new Range(2, 5))))
        };

        [Test, ExpectedArgumentNullException]
        public void ConstructorWithContentOnlyThrowsIfContentIsNull()
        {
            new Substring(null);
        }

        [Test, ExpectedArgumentNullException]
        public void ConstructorWithContentAndRangeThrowsIfContentIsNull()
        {
            new Substring(null, new Range(0, 0));
        }

        [Test, ExpectedArgumentException]
        public void ConstructorWithContentAndRangeThrowsIfRangeExceedsBounds()
        {
            new Substring("abcde", new Range(3, 3));
        }

        [Test]
        public void ConstructorWithContentOnlyInitializesProperties()
        {
            Substring substring = new Substring("abcde");
            Assert.AreEqual("abcde", substring.Content);
            Assert.AreEqual(new Range(0, 5), substring.Range);
            Assert.AreEqual(5, substring.Length);
        }

        [Test]
        public void ConstructorWithContentAndRangeInitializesProperties()
        {
            Substring substring = new Substring("abcde", new Range(2, 3));
            Assert.AreEqual("abcde", substring.Content);
            Assert.AreEqual(new Range(2, 3), substring.Range);
            Assert.AreEqual(3, substring.Length);
        }

        [Test]
        public void ToStringExtractsSubstring()
        {
            Substring substring = new Substring("abcde", new Range(2, 3));
            Assert.AreEqual("cde", substring.ToString());
        }

        [Test]
        public void IndexerThrowsIfIndexTooLow()
        {
            Substring substring = new Substring("abcde", new Range(1, 3));
            char c;
            Assert.Throws<IndexOutOfRangeException>(() => c = substring[-1]);
        }

        [Test]
        public void IndexerThrowsIfIndexTooHigh()
        {
            Substring substring = new Substring("abcde", new Range(1, 3));
            char c;
            Assert.Throws<IndexOutOfRangeException>(() => c = substring[3]);
        }

        [Test]
        public void IndexerReturnsIndexedCharWithAppropriateOffsets()
        {
            Substring substring = new Substring("abcde", new Range(1, 3));
            Assert.AreEqual('b', substring[0]);
            Assert.AreEqual('c', substring[1]);
            Assert.AreEqual('d', substring[2]);
        }

        [Test]
        [Row(-1, null, ExpectedException=typeof(ArgumentOutOfRangeException))]
        [Row(4, null, ExpectedException = typeof(ArgumentOutOfRangeException))]
        [Row(0, "bcd")]
        [Row(1, "cd")]
        [Row(3, "")]
        public void Extract(int index, string expectedToString)
        {
            Substring substring = new Substring("abcde", new Range(1, 3));
            Assert.AreEqual(expectedToString, substring.Extract(index).ToString());
        }

        [Test]
        [Row(-1, 2, null, ExpectedException = typeof(ArgumentOutOfRangeException))]
        [Row(1, -1, null, ExpectedException = typeof(ArgumentOutOfRangeException))]
        [Row(1, 3, null, ExpectedException = typeof(ArgumentOutOfRangeException))]
        [Row(0, 3, "bcd")]
        [Row(1, 1, "c")]
        [Row(1, 2, "cd")]
        [Row(3, 0, "")]
        public void Extract2(int index, int length, string expectedToString)
        {
            Substring substring = new Substring("abcde", new Range(1, 3));
            Assert.AreEqual(expectedToString, substring.Extract(index, length).ToString());
        }

        [Test]
        [Row("abcdef", 0, 4, "abcdef", 1, 4, 0)]
        [Row("abcdef", 1, 4, "abcdef", 0, 4, 0)]
        [Row("abcdef", 0, 4, "abcdef", 0, 4, 4)]
        [Row("abcdef", 1, 4, "abcef", 1, 4, 2)]
        [Row("abcdef", 1, 4, "abc", 1, 2, 2)]
        [Row("abc", 1, 2, "abcdef", 1, 4, 2)]
        public void FindCommonPrefixLength(string content1, int startIndex1, int length1,
            string content2, int startIndex2, int length2, int expectedResult)
        {
            Substring substring1 = new Substring(content1, new Range(startIndex1, length1));
            Substring substring2 = new Substring(content2, new Range(startIndex2, length2));

            Assert.AreEqual(expectedResult, substring1.FindCommonPrefixLength(substring2));
        }

        [Test]
        [Row("abcdef", 2, 4, "abcdef", 1, 4, 0)]
        [Row("abcdef", 1, 4, "abcdef", 0, 4, 0)]
        [Row("abcdef", 2, 4, "abcdef", 2, 4, 4)]
        [Row("abcdef", 2, 4, "abcefg", 2, 3, 2)]
        [Row("def", 0, 3, "abcdef", 3, 3, 3)]
        [Row("abcef", 1, 4, "def", 0, 3, 2)]
        public void FindCommonSuffixLength(string content1, int startIndex1, int length1,
            string content2, int startIndex2, int length2, int expectedResult)
        {
            Substring substring1 = new Substring(content1, new Range(startIndex1, length1));
            Substring substring2 = new Substring(content2, new Range(startIndex2, length2));

            Assert.AreEqual(expectedResult, substring1.FindCommonSuffixLength(substring2));
        }
    }
}
