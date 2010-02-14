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
using Gallio.Common.Collections;
using MbUnit.Framework;

namespace Gallio.Tests.Common.Collections
{
    [TestFixture]
    [TestsOn(typeof(GenericCollectionUtils))]
    public class GenericCollectionUtilsTest
    {
        [Test]
        [Row(new[] { 1, 2, 3 }, 3, new[] { "1", "2", "3" })]
        public void ConvertAndCopyAll(int[] input, int outputLength, string[] expectedOutput)
        {
            string[] output = new string[outputLength];

            GenericCollectionUtils.ConvertAndCopyAll(input, output, value => value.ToString());

            Assert.AreEqual(expectedOutput, output);
        }

        [Test]
        [Row(new[] { 1, 2, 3 }, new[] { "1", "2", "3" })]
        public void ConvertAllToArray(int[] input, string[] expectedOutput)
        {
            string[] output = GenericCollectionUtils.ConvertAllToArray(input, value => value.ToString());

            Assert.AreEqual(expectedOutput, output);
        }

        [Test]
        public void AddAllIfNotAlreadyPresent()
        {
            string[] input = new[] { "one", "one", "two" };
            List<string> output = new List<string>();
            GenericCollectionUtils.AddAllIfNotAlreadyPresent(input, output);
            Assert.AreEqual(2, output.Count);
            Assert.AreElementsEqual(new[] { "one" ,"two" }, output);
        }

        [Test]
        [ExpectedArgumentNullException]
        public void Selects_from_null_enumeration_should_throw_exception()
        {
            var enumerator = GenericCollectionUtils.Select<string, int>(null, x => 0).GetEnumerator();
            enumerator.MoveNext();
        }

        [Test]
        [ExpectedArgumentNullException]
        public void Selects_from_null_filter_should_throw_exception()
        {
            var enumerator = GenericCollectionUtils.Select<string, int>(EmptyArray<string>.Instance, null).GetEnumerator();
            enumerator.MoveNext();
        }

        [Test]
        public void Selects_ok()
        {
            var input = new[] { "1", "2", "3" };
            var output = GenericCollectionUtils.Select(input, x => Int32.Parse(x));
            Assert.AreElementsEqual(new[] { 1, 2, 3 }, output);
        }
    }
}
